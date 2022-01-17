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
            
            s.RegisterSingletonComponent<PlayerInput>();
            s.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(DateTime.Now.Ticks)));
            s.RegisterSingletonComponent(new Player
            {
                UnitConfig = new UnitConfig
                {
                    ViewId = ViewId.Test,
                    X = 0,
                    Y = 0,
                },
            });
            
            s.RegisterComponent<Unit>(1024);

            s.Buffer<Unit>().OnComponentAdded += _instance.UnitAdded;
            s.Buffer<Unit>().OnComponentRemoved += _instance.UnitRemoved;
            
            s.Initialize();

            _instance._simulation = s;
        }

        private void UnitAdded(object sender, EntityEventArgs e)
        {
            Debug.Log("unit added");
        }

        private void UnitRemoved(object sender, EntityEventArgs e)
        {
            
        }

        public static void EndSimulation()
        {
            if(_instance == null)
                return;
            
            _instance._simulation.Buffer<Unit>().OnComponentAdded -= _instance.UnitAdded;
            _instance._simulation.Buffer<Unit>().OnComponentRemoved -= _instance.UnitRemoved;

            _instance._simulation.Dispose();
            _instance._simulation = null;
        }
    }
}