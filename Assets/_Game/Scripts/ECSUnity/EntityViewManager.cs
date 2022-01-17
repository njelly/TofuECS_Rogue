using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Pool;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class EntityViewManager
    {
        private static readonly ReadOnlyDictionary<int, string> _viewIdToAssetPath = new (
            new Dictionary<int, string>
            {
                {0, ""}
            });

        private ObjectPool<EntityView> _pool;
        
        
    }
}