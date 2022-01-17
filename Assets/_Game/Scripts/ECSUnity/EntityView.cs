using Tofunaut.TofuECS;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public abstract class EntityView : MonoBehaviour
    {
        public int EntityId { get; private set; }

        public virtual void Initialize(Simulation s, int entityId)
        {
            EntityId = entityId;
        }
    }
}