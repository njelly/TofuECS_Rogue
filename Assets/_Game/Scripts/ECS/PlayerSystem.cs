using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class PlayerSystem : ISystem, ISystemEventListener<PlayerInput>
    {
        public unsafe void Initialize(Simulation s)
        {
            var player = s.GetSingletonComponentUnsafe<Player>();
            player->UnitEntity = s.CreateEntity();
            s.Buffer<Unit>().Set(player->UnitEntity);
        }

        public void Process(Simulation s) { }

        public void OnSystemEvent(Simulation s, in PlayerInput eventData)
        {
            // save the player input to the state of a singleton component
            s.SetSingletonComponent(eventData);
        }
    }
}