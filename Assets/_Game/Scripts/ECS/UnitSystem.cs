using System;
using System.Numerics;
using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class UnitSystem : ISystem
    {
        public static event EventHandler<UnitViewIdEventArgs> UnitViewIdChanged; 

        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var playerInput = s.GetSingletonComponent<PlayerInput>();
            var buffer = s.Buffer<Unit>();
            var i = 0;
            while (buffer.NextUnsafe(ref i, out var entity, out var unit))
            {
                ProcessUnitInput(s, entity, unit, playerInput);
                ProcessUnitMovement(s, entity, unit, playerInput);
            }
        }

        private static unsafe void ProcessUnitInput(Simulation s, int entity, Unit* unit, in PlayerInput playerInput)
        {
            // if we're not moving, see if we can start
            if (!unit->IsMoving)
            {
                // do nothing if the dir magnitude is too low
                if (unit->Input.DirMagnitude < UnitInput.FaceThreshold) 
                    return;
                
                unit->Facing = unit->Input.Dir;
                
                if (unit->Input.DirMagnitude < UnitInput.MoveThreshold) 
                    return;
                
                // check that the unit can move there in the first place...
                var newTargetPos = unit->TargetPos + unit->Input.Dir.ToVector2();
                if(CanUnitOccupyCoord(s, entity, unit, (int)newTargetPos.X, (int)newTargetPos.Y))
                    unit->TargetPos += unit->Input.Dir.ToVector2();
            }
            else
            {
                if (!s.Buffer<Modifiable>().GetUnsafe(entity, out var modifiable)) 
                    return;

                switch (unit->Input.DirMagnitude)
                {
                    // use a modifier for sprinting if the threshold has been reached
                    case >= UnitInput.SprintThreshold when unit->SprintModifierEntity == Simulation.InvalidEntityId:
                        unit->SprintModifierEntity = ModifiableSystem.AssignModifierToModifiable(s, modifiable, new Modifier
                        {
                            ModifiableEntity = entity,
                            ModifierClass = ModifierClass.MoveSpeed,
                            StackedValue = 2f,
                            Value = 2f,
                            TimeLeft = -1f,
                        }, StackBehavior.Add);
                        break;
                    case < UnitInput.SprintThreshold when unit->SprintModifierEntity != Simulation.InvalidEntityId:
                        ModifiableSystem.RemoveModifierFromModifiable(s, modifiable, unit->SprintModifierEntity);
                        unit->SprintModifierEntity = Simulation.InvalidEntityId;
                        break;
                }
            }
        }

        private static unsafe void ProcessUnitMovement(Simulation s, int unitEntity, Unit* unit, in PlayerInput playerInput)
        {
            if (!unit->IsMoving)
                return;

            var moveSpeed = unit->MoveSpeed;
            if (s.Buffer<Modifiable>().Get(unitEntity, out var modifiable) &&
                ModifiableSystem.TryGetValueForModifierClass(s, modifiable, ModifierClass.MoveSpeed, StackBehavior.Add,
                    out var moveSpeedMultiplier))
                moveSpeed *= moveSpeedMultiplier;
            
            var step = Vector2.Normalize(unit->TargetPos - unit->CurrentPos) * moveSpeed * playerInput.DeltaTime;
            var toTargetDistSquared = Vector2.DistanceSquared(unit->TargetPos, unit->CurrentPos);
            // keep going if we haven't reached our target
            if (toTargetDistSquared > step.LengthSquared())
            {
                unit->CurrentPos += step;
            }
            // the unit is almost at its target, but it is still moving
            else if(unit->Input.DirMagnitude >= UnitInput.MoveThreshold)
            {
                var newTargetPos = unit->TargetPos + step.ToCardinalDirection4().ToVector2();
                var hasChangedDirection = step.ToCardinalDirection4() != unit->Input.Dir;
                var dirToVector = unit->Input.Dir.ToVector2();
                // if the unit can't keep moving, then stop
                if(!CanUnitOccupyCoord(s, unitEntity, unit, (int)newTargetPos.X, (int)newTargetPos.Y))
                {
                    unit->CurrentPos = unit->TargetPos;
                }
                // simply keep going if the unit hasn't changed direction
                else if (!hasChangedDirection)
                {
                    unit->CurrentPos += step;
                    unit->TargetPos += dirToVector;
                }
                // otherwise, we need to calculate how much we over-stepped, and apply that amount to our new direction
                else
                {
                    unit->CurrentPos = unit->TargetPos;
                    unit->TargetPos += dirToVector;
                    var overStepDist = (float) (step.Length() - Math.Sqrt(toTargetDistSquared));
                    unit->CurrentPos += overStepDist * dirToVector;
                    unit->Facing = unit->Input.Dir;
                }
            }
            // the unit has come to a stop
            else
            {
                unit->CurrentPos = unit->TargetPos;
            }
        }

        public unsafe static bool CanUnitOccupyCoord(Simulation s, int unitEntity, Unit* unit, int newX, int newY)
        {
            if (newX < 0 || newY < 0 || newX >= Floor.FloorSize || newY >= Floor.FloorSize)
                return false;
            
            var floor = s.GetSingletonComponent<Floor>();

            // check against the target tile's type first...
            var tiles = s.AnonymousBuffer<Tile>(floor.TileBufferIndex);
            var tile = tiles.GetAt(newX + newY * Floor.FloorSize);
            switch (tile.Type)
            {
                case TileType.Void:
                case TileType.Bedrock:
                    return false;
                case TileType.Stone:
                    if (tile.Height != Tile.FloorHeight)
                        return false;
                    break;
                case TileType.LadderUp:
                    if (floor.Depth <= 1)
                        return false;
                    break;
            }

            // make sure this unit is not moving to the same position as another unit
            var unitBuffer = s.Buffer<Unit>();
            var i = 0;
            while (unitBuffer.Next(ref i, out var otherEntity, out var otherUnit))
            {
                if (otherEntity == unitEntity)
                    continue;

                if (unit->TargetPos == otherUnit.TargetPos)
                    return false;
            }

            return true;
        }

        public static void CreateUnit(Simulation s, int entity, in UnitConfig unitConfig)
        {
            s.Buffer<Unit>().Set(entity, new Unit
            {
                TargetPos = unitConfig.Position,
                CurrentPos = unitConfig.Position,
                CurrentViewId = unitConfig.ViewId,
                Facing = unitConfig.InitFacing,
                MoveSpeed = unitConfig.MoveSpeed,
            });
            s.Buffer<Modifiable>().Set(entity);
        }

        public static void DestroyUnit(Simulation s, int entity)
        {
            s.Buffer<Unit>().Remove(entity);
            s.Buffer<Modifiable>().Remove(entity);
        }

        public static int CreateEntityWithUnit(Simulation s, in UnitConfig unitConfig)
        {
            var e = s.CreateEntity();
            CreateUnit(s, e, unitConfig);
            return e;
        }

        public static unsafe void ChangeViewId(Simulation s, int entity, Unit* unit, ViewId newViewId)
        {
            var prevViewId = unit->CurrentViewId;
            unit->CurrentViewId = newViewId;
            UnitViewIdChanged?.Invoke(s, new UnitViewIdEventArgs(entity, prevViewId, newViewId));
        }
    }

    public class UnitViewIdEventArgs : EntityEventArgs
    {
        public readonly ViewId PrevViewId;
        public readonly ViewId NewViewId;

        public UnitViewIdEventArgs(int entity, ViewId prevViewId, ViewId newViewId) : base(entity)
        {
            PrevViewId = prevViewId;
            NewViewId = newViewId;
        }
    }
}