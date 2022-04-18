using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.ECSUnity;
using Tofunaut.TofuECS.Utilities;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue
{
    public class InGameStateRequest
    {
        public SimulationConfig SimulationConfig;
    }
    
    public class InGameState : AppState<InGameStateRequest>
    {
        private readonly RogueTile _rogueTile;
        
        private Simulation _simulation;
        private SimulationConfig _config;
        private FloorView _floorView;

        public InGameState(RogueTile rogueTile)
        {
            _rogueTile = rogueTile;
        }

        public override Task OnEnter(InGameStateRequest request)
        {
            _config = request.SimulationConfig;
            _floorView = new FloorView(_rogueTile);
            
            #region INIT_SIMULATION
            
            _simulation = new Simulation(new UnityLogService(), new ISystem[]
            {
                new TileSystem(),
                new UnitSystem(),
                new HealthSystem(),
            });
            
            TileSystem.TileUpdated += OnTileUpdated;
            UnitSystem.UnitCreated += OnUnitCreated;
            UnitSystem.UnitMoved += OnUnitMoved;
            
            // register anonymous components
            var floorBufferIndex = _simulation.RegisterAnonymousComponent<Tile>(_config.InitialTiles.Length);
            
            // register singleton components
            _simulation.RegisterSingletonComponent(new WorldData
            {
                Width = _config.WorldWidth,
                TileBufferIndex = floorBufferIndex,
            });
            _simulation.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(_config.Seed)));
            
            // register entity components
            _simulation.RegisterComponent<Unit>(32);
            _simulation.RegisterComponent<Health>(32);
            
            // Initialize!
            _simulation.Initialize();
            
            // Set up the simulation with some initial data
            _simulation.SystemEvent(new SetTilesInput
            {
                Tiles = _config.InitialTiles,
            });
            _simulation.SystemEvent(new CreateUnitInput
            {
                Unit = new Unit
                {
                    DoesBlockTile = true,
                    Facing = Facing.East,
                    TileIndex = 0,
                    ViewId = 1,
                },
                Health = new Health
                {
                    HP = 100,
                    MaxHP = 100,
                }
            });
            
            #endregion
            
            return Task.CompletedTask;
        }

        private void OnTileUpdated(int x, int y, in Tile tile)
        {
            _floorView.UpdateTile(x, y, tile);
        }

        private void OnUnitCreated(int entityId, in Unit unit)
        {
            
        }

        private void OnUnitMoved(int entityId, in Unit unit)
        {
            
        }

        public override Task OnExit()
        {
            _simulation.Dispose();
            
            return Task.CompletedTask;
        }
    }
}