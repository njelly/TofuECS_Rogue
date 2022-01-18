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
                ProcessUnitMovement(s, unit, playerInput);
            }
        }

        private static unsafe void ProcessUnitInput(Simulation s, int entity, Unit* unit, in PlayerInput playerInput)
        {
            // for now, do nothing while the unit is moving (although we'll probably want to allow attacking)
            if (unit->IsMoving) 
                return;
            
            // if we're not moving and holding a direction, depending on the dir magnitude either face that direction
            // or face that direction and start moving 
            switch (unit->Input.DirMagnitude)
            {
                case UnitInput.FaceThreshold:
                    unit->Facing = unit->Input.Dir;
                    break;
                case UnitInput.MoveThreshold:
                    unit->Facing = unit->Input.Dir;
                    unit->TargetPos += unit->Input.Dir.ToVector2();
                    break;
            }
        }

        private static unsafe void ProcessUnitMovement(Simulation s, Unit* unit, in PlayerInput playerInput)
        {
            if (!unit->IsMoving)
                return;

            var step = Vector2.Normalize(unit->TargetPos - unit->CurrentPos) * unit->MoveSpeed * playerInput.DeltaTime;
            var toTargetDistSquared = Vector2.DistanceSquared(unit->TargetPos, unit->CurrentPos);
            // keep going if we haven't reached our target
            if (toTargetDistSquared > step.LengthSquared())
            {
                unit->CurrentPos += step;
            }
            // the unit is almost at its target, but it is still moving
            else if(unit->Input.DirMagnitude >= UnitInput.MoveThreshold)
            {
                var hasChangedDirection = step.ToCardinalDirection4() != unit->Input.Dir;
                var dirToVector = unit->Input.Dir.ToVector2();
                // simply keep going if the unit hasn't changed direction
                if (!hasChangedDirection)
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

        public static int CreateUnit(Simulation s, in UnitConfig unitConfig)
        {
            var entity = s.CreateEntity();
            var unitBuffer = s.Buffer<Unit>();
            unitBuffer.Set(entity, new Unit
            {
                TargetPos = unitConfig.Position,
                CurrentPos = unitConfig.Position,
                CurrentViewId = unitConfig.ViewId,
                Facing = unitConfig.InitFacing,
                MoveSpeed = unitConfig.MoveSpeed,
            });

            return entity;
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