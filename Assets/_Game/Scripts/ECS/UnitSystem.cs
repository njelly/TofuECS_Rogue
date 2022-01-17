using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class UnitSystem : ISystem
    {
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

        public static unsafe void MoveUnit(Unit* unit, int newX, int newY, float timeToDestination)
        {
            unit->X = newX;
            unit->Y = newY;
            unit->TimeToDestination = timeToDestination;
        }
    }
}