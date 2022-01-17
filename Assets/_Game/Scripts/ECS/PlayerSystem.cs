using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class PlayerSystem : ISystem, ISystemEventListener<PlayerInput>
    {
        public unsafe void Initialize(Simulation s)
        {
            var player = s.GetSingletonComponentUnsafe<Player>();
            player->UnitEntity = UnitSystem.CreateUnit(s, player->UnitConfig);
        }

        public void Process(Simulation s) { }

        public unsafe void OnSystemEvent(Simulation s, in PlayerInput eventData)
        {
            // save the player input to the state of a singleton component
            s.SetSingletonComponent(eventData);
            var player = s.GetSingletonComponent<Player>();

            if (!s.Buffer<Unit>().GetUnsafe(player.UnitEntity, out var unit)) 
                return;
            
            // set the player unit's input
            unit->Input = eventData.UnitInput;
        }
    }
}