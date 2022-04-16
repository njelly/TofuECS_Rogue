using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class HealthSystem : ISystem
    {
        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var healthBuffer = s.Buffer<Health>();
            var entitiesToDestroy = stackalloc int[healthBuffer.Size];
            var numEntitiesToDestroy = 0;
            var i = 0;
            while (healthBuffer.Next(ref i, out var entity, out var health))
            {
                // any entity with a health component less than or equal to zero will be destroyed
                if (health.HP <= 0)
                    entitiesToDestroy[numEntitiesToDestroy++] = entity;
            }
            
            for(i = 0; i < numEntitiesToDestroy; i++)
                s.Destroy(entitiesToDestroy[i]);
        }
    }
}