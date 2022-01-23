using System;
using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class ModifierSystem : ISystem
    {
        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var gameState = s.GetSingletonComponent<GameState>();
            var playerInput = s.GetSingletonComponent<PlayerInput>();
            var buffer = s.AnonymousBuffer<Modifier>(gameState.ModifierBufferIndex);
            var i = 0;
            while (buffer.NextUnsafe(ref i, out var modifier))
            {
                if(modifier->TimeLeft <= 0)
                    return;

                modifier->TimeLeft -= playerInput.DeltaTime;
                
                // TODO: end the modifier, notify other systems, not implemented yet
                //if(modifier->TimeLeft < 0)
                //    s.SystemEvent(new ModifierEndedEvent
                //    {
                //        ModifierBufferIndex = gameState.ModifierBufferIndex,
                //        ModifierIndex = i,
                //        ModifiableEntity = modifier->ModifiableEntity,
                //    });
            }
        }
    }

    public struct ModifierEndedEvent
    {
        public int ModifierBufferIndex;
        public int ModifierIndex;
        public int ModifiableEntity;
    }
}