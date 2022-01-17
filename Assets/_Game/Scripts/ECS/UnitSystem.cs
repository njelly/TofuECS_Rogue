using System;
using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class UnitSystem : ISystem
    {
        public static event EventHandler<EntityEventArgs> UnitMoved;
        public static event EventHandler<EntityEventArgs> UnitViewIdChanged; 

        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var buffer = s.Buffer<Unit>();
            var i = 0;
            while (buffer.NextUnsafe(ref i, out _, out var unit))
            {
                ProcessUnitMovement(s, unit);
            }
        }

        private static unsafe void ProcessUnitMovement(Simulation s, Unit* unit)
        {
            if (unit->TimeToDestination <= 0)
                return;

            var playerInput = s.GetSingletonComponent<PlayerInput>();
            unit->TimeToDestination -= playerInput.DeltaTime;

            if (unit->TimeToDestination > 0) 
                return;

            unit->TimeToDestination = 0;
            unit->PrevX = unit->X;
            unit->PrevY = unit->Y;
        }

        public static unsafe void MoveUnit(Simulation s, int entity, Unit* unit, int newX, int newY, float timeToDestination)
        {
            unit->X = newX;
            unit->Y = newY;
            unit->TimeToDestination = timeToDestination;
            
            UnitMoved?.Invoke(s, new EntityEventArgs(entity));
        }

        public static int CreateUnit(Simulation s, in UnitConfig unitConfig)
        {
            var entity = s.CreateEntity();
            var unitBuffer = s.Buffer<Unit>();
            unitBuffer.Set(entity, new Unit
            {
                X = unitConfig.X,
                Y = unitConfig.Y,
                PrevX = unitConfig.X,
                PrevY = unitConfig.Y,
                TimeToDestination = 0f,
                CurrentViewId = unitConfig.ViewId,
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