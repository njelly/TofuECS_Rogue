using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.Utilities;
using Tofunaut.TofuECS.Utilities;
using Tofunaut.TofuECS_Rogue.Generation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using PlayerInput = Tofunaut.TofuECS_Rogue.ECS.PlayerInput;
using Tile = Tofunaut.TofuECS_Rogue.ECS.Tile;
using Vector2 = System.Numerics.Vector2;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class SimulationRunner : MonoBehaviour
    {
        public enum State
        {
            NotRunning,
            LoadingData,
            Paused,
            Running,
        }
        
        public Simulation Current { get; private set; }
        public State CurrentState => _stateMachine.CurrentState;

        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private TilemapManager _tilemapManager;
        [SerializeField] private UnitViewManager _unitViewManager;
        [SerializeField] private int _floorGenSeed;

        private PlayerInputManager _playerInputManager;
        private Tilemap _tilemap;
        private EnumStateMachine<State> _stateMachine;

        private void Awake()
        {
            enabled = false;
            _playerInputManager = new PlayerInputManager();

            _stateMachine = new EnumStateMachine<State>();
            _stateMachine.Enter(State.NotRunning);
        }

        private void Update()
        {
            switch (_stateMachine.CurrentState)
            {
                case State.Running:
                    TickSimulation();
                    break;
                case State.NotRunning:
                    break;
                case State.LoadingData:
                    break;
                case State.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async void BeginSimulation()
        {
            if (Current != null)
            {
                Debug.LogError("a Simulation is already running");
                return;
            }
            
            var floorGenResult = await FloorGen.RequestFloorAsync(new FloorGenParams
            {
                MaxRoomSize = 20,
                MinRoomSize = 10,
                Seed = _floorGenSeed,
            });

            enabled = true;
            _tilemapManager.SetEnabled(true);
            _playerInputManager.Enable(_inputActionAsset);

            Current = new Simulation(new UnityLogService(), new ISystem[]
            {
                new PlayerSystem(),
                new UnitSystem(),
                new FloorSystem(),
            });
            
            Current.RegisterSingletonComponent<PlayerInput>();
            Current.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(DateTime.Now.Ticks)));
            Current.RegisterSingletonComponent(new Player
            {
                UnitConfig = new UnitConfig
                {
                    Position = new Vector2(floorGenResult.PlayerSpawnX, floorGenResult.PlayerSpawnY),
                    ViewId = ViewId.Player,
                    InitFacing = CardinalDirection4.East,
                    MoveSpeed = 3,
                },
            });
            var tileBufferIndex = Current.RegisterAnonymousComponent<Tile>(Floor.FloorSize * Floor.FloorSize);
            Current.RegisterSingletonComponent(new Floor
            {
                Depth = 1,
                TileBufferIndex = tileBufferIndex,
            });
            Current.RegisterComponent<Unit>(16);

            Current.Buffer<Unit>().OnComponentAdded += UnitAdded;
            Current.Buffer<Unit>().OnComponentRemoved += UnitRemoved;
            UnitSystem.UnitViewIdChanged += UnitViewIdChanged;
            FloorSystem.TilesUpdated += TilesUpdated;
            
            Current.Initialize();
            
            _tilemapManager.enabled = true;
            Current.SystemEvent(new FloorTilesInputEvent
            {
                Tiles = floorGenResult.Tiles,
            });
            _stateMachine.Enter(State.Running);
        }

        public void EndSimulation()
        {
            enabled = false;
            
            Current.Buffer<Unit>().OnComponentAdded -= UnitAdded;
            Current.Buffer<Unit>().OnComponentRemoved -= UnitRemoved;
            UnitSystem.UnitViewIdChanged -= UnitViewIdChanged;
            FloorSystem.TilesUpdated -= TilesUpdated;
            
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

        private void TilesUpdated(object sender, TilesUpdatedEventArgs e)
        {
            var buffer = Current.AnonymousBuffer<Tile>(e.TileBufferIndex);
            _tilemapManager.UpdateTiles(buffer, e.Tiles, e.Indexes);
        }
    }
}