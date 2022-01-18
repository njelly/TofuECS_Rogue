using System;
using System.Numerics;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Tofunaut.TofuECS_Rogue.ECS.PlayerInput;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class SimulationRunner : MonoBehaviour
    {
        public Simulation Current { get; private set; }

        [SerializeField] private UnitViewManager _unitViewManager;

        private PlayerInputManager _playerInputManager;

        private void Awake()
        {
            enabled = false;
            _playerInputManager = new PlayerInputManager();
        }

        private void Update()
        {
            TickSimulation();
        }

        public void BeginSimulation(InputActionAsset inputActionAsset)
        {
            if (Current != null)
            {
                Debug.LogError("a Simulation is already running");
                return;
            }

            enabled = true;

            _playerInputManager.Enable(inputActionAsset);

            Current = new Simulation(new UnityLogService(), new ISystem[]
            {
                new PlayerSystem(),
                new UnitSystem(),
            });
            
            Current.RegisterSingletonComponent<PlayerInput>();
            Current.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(DateTime.Now.Ticks)));
            Current.RegisterSingletonComponent(new Player
            {
                UnitConfig = new UnitConfig
                {
                    ViewId = ViewId.Player,
                    InitFacing = CardinalDirection4.East,
                    MoveSpeed = 3,
                },
            });
            
            Current.RegisterComponent<Unit>(16);

            Current.Buffer<Unit>().OnComponentAdded += UnitAdded;
            Current.Buffer<Unit>().OnComponentRemoved += UnitRemoved;
            UnitSystem.UnitViewIdChanged += UnitViewIdChanged;
            
            Current.Initialize();
        }

        public void EndSimulation()
        {
            enabled = false;
            
            Current.Buffer<Unit>().OnComponentAdded -= UnitAdded;
            Current.Buffer<Unit>().OnComponentRemoved -= UnitRemoved;
            UnitSystem.UnitViewIdChanged -= UnitViewIdChanged;
            
            _unitViewManager.Clear();
            _playerInputManager.Disable();

            Current.Dispose();
            Current = null;
        }

        private void TickSimulation()
        {
            _playerInputManager.Poll(out var playerInput);
            Current.SystemEvent(playerInput);
            Current.Tick();
        }

        private void UnitViewIdChanged(object sender, UnitViewIdEventArgs e)
        {
            _unitViewManager.ReleaseUnitView(e.Entity, e.PrevViewId);
            _unitViewManager.CreateUnitView(Current, e.Entity, e.NewViewId);
        }

        private void UnitAdded(object sender, EntityEventArgs e)
        {
            if (!Current.Buffer<Unit>().Get(e.Entity, out var unit))
                return;
            
            _unitViewManager.CreateUnitView(Current, e.Entity, unit.CurrentViewId);
        }

        private void UnitRemoved(object sender, EntityEventArgs e)
        {
            if (!Current.Buffer<Unit>().Get(e.Entity, out var unit))
                return;
            
            _unitViewManager.ReleaseUnitView(e.Entity, unit.CurrentViewId);
        }
    }
}