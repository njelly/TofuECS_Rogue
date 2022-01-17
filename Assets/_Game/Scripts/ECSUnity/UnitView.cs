using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public sealed class UnitView : MonoBehaviour
    {
        public int Entity { get; private set; }
        
        private Simulation _s;
        private UnitViewManager _unitViewManager;
        
        public void Initialize(Simulation s, UnitViewManager unitViewManager, int entity)
        {
            _s = s;
            _unitViewManager = unitViewManager;
            Entity = entity;
        }

        private void Update()
        {
            if (!_s.Buffer<Unit>().Get(Entity, out var unit))
                return;

            transform.position = Vector2Utils.ToUnityVector2(unit.CurrentPos);
        }
    }
}