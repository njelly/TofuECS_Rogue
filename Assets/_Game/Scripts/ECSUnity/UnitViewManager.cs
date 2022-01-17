using System;
using System.Collections.Generic;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.Pool;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class UnitViewManager : MonoBehaviour
    {
        [SerializeField] private UnitView _testPrefab;

        private Dictionary<ViewId, ObjectPool<UnitView>> _viewIdToPool;
        private Dictionary<int, UnitView> _entityToUnitView;

        private void Awake()
        {
            _viewIdToPool = new Dictionary<ViewId, ObjectPool<UnitView>>();
            _entityToUnitView = new Dictionary<int, UnitView>();
        }

        public void CreateUnitView(Simulation s, int entity, ViewId viewId)
        {
            var prefab = viewId switch
            {
                ViewId.None => null,
                ViewId.Test => _testPrefab,
                _ => throw new ArgumentOutOfRangeException(nameof(viewId), viewId, null)
            };

            if (prefab == null)
                return;

            // create the pool if it doesn't exist
            if (!_viewIdToPool.TryGetValue(viewId, out var pool))
            {
                pool = new ObjectPool<UnitView>(
                    () => Instantiate(prefab),
                    unitView => unitView.gameObject.SetActive(true), 
                    unitView => unitView.gameObject.SetActive(false));
                //unitView => Destroy(unitView.gameObject)); Unity bug??? this never gets called!
                
                _viewIdToPool.Add(viewId, pool);
            }

            // get the unit view and initialize it
            var unitView = pool.Get();
            _entityToUnitView.Add(entity, unitView);
            unitView.Initialize(s, this, entity);
        }

        public void ReleaseUnitView(int entity, ViewId viewId)
        {
            if (viewId == ViewId.None)
                return;
            
            if (!_entityToUnitView.TryGetValue(entity, out var unitView))
            {
                Debug.LogError($"no UnitView exists for entity: {_entityToUnitView}");
                return;
            }
            
            if (!_viewIdToPool.TryGetValue(viewId, out var pool))
            {
                Debug.LogError($"no pool exists for ViewId: {viewId}");
                return;
            }
            
            pool.Release(unitView);
            _entityToUnitView.Remove(entity);
        }

        public bool TryGetUnitView(int entity, out UnitView unitView) =>
            _entityToUnitView.TryGetValue(entity, out unitView);

        public void Clear()
        {
            foreach (var kvp in _viewIdToPool)
                kvp.Value.Clear();

            // need to call this for now, Unity's object pool destroy callback is bugged, it never seems to get called
            foreach (var kvp in _entityToUnitView)
                Destroy(kvp.Value.gameObject);
            
            _entityToUnitView.Clear();
        }
    }
}