using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS.Utilities;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class SimulationRunner : MonoBehaviour
    {
        public static Simulation Current => _instance ? _instance._simulation : null;
        
        private static SimulationRunner _instance;
        
        private Simulation _simulation;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("An instance of SimulationRunner already exists.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Update()
        {
            _simulation.SystemEvent(new PlayerInput
            {
                DeltaTime = Time.deltaTime,
            });
            
            _simulation?.Tick();
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static void BeginSimulation()
        {
            if (_instance == null)
                return;
            
            var s = new Simulation(new UnityLogService(), new ISystem[]
            {
                new PlayerSystem(),
                new UnitSystem(),
            });
            
            s.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(DateTime.Now.Ticks)));
            s.RegisterSingletonComponent<PlayerInput>();
            s.RegisterSingletonComponent<Player>();
            
            s.RegisterComponent<Unit>(1024);
            
            s.Initialize();

            _instance._simulation = s;
        }

        public static void EndSimulation()
        {
            if(_instance == null)
                return;

            _instance._simulation.Dispose();
            _instance._simulation = null;
        }
    }
}