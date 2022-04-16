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
        private Simulation _simulation;
        private SimulationConfig _config;
        private FloorView _floorView;
        
        public override Task OnEnter(InGameStateRequest request)
        {
            _config = request.SimulationConfig;
            
            _simulation = new Simulation(new UnityLogService(), new ISystem[]
            {
                new UnitSystem(),
                new HealthSystem(),
            });
            
            var floorBufferIndex = _simulation.RegisterAnonymousComponent<Tile>(_config.WorldWidth * _config.WorldHeight);
            
            _simulation.RegisterSingletonComponent(new WorldData
            {
                Width = _config.WorldWidth,
                FloorBufferIndex = floorBufferIndex,
            });
            _simulation.RegisterSingletonComponent(new XorShiftRandom(Convert.ToUInt64(_config.Seed)));
            
            _simulation.RegisterComponent<Unit>(_config.MaxUnitComponents);
            _simulation.RegisterComponent<Health>(_config.MaxHealthComponents);
            
            _simulation.Initialize();
            
            return Task.CompletedTask;
        }

        public override Task OnExit()
        {
            _simulation.Dispose();
            
            return Task.CompletedTask;
        }
    }
}