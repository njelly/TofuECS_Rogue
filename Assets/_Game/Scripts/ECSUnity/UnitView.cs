using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.Utilities;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public sealed class UnitView : MonoBehaviour
    {
        public int Entity { get; private set; }

        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private Simulation _s;
        private UnitViewManager _unitViewManager;
        private CardinalDirection4 _prevFacing;
        
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

            // flip the sprite depending on how the unit is facing
            if (_prevFacing != unit.Facing)
            {
                _spriteRenderer.flipX = unit.Facing switch
                {
                    CardinalDirection4.East => false,
                    CardinalDirection4.West => true,
                    _ => _spriteRenderer.flipX
                };
            }
        }
    }
}