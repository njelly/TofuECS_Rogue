using Tofunaut.TofuECS;
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
    }
}