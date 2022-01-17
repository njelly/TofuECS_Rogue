using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.ECSUnity;
using Tofunaut.TofuECS_Rogue.ECSUnity.UI;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue
{
    public class InGameStateRequest
    {
        
    }

    public class InGameState : AppState<InGameStateRequest>
    {
        private readonly AppStateMachine _appStateMachine;
        private readonly ViewStack _viewStack;
        
        private GameObject _simulationRunnerGo;
        private SimulationDebugViewController _simulationDebugView;

        public InGameState(AppStateMachine appStateMachine, ViewStack viewStack)
        {
            _appStateMachine = appStateMachine;
            _viewStack = viewStack;
        }
        
        public override async Task OnEnter(InGameStateRequest request)
        {
            _simulationRunnerGo = new GameObject("SimulationRunner", typeof(SimulationRunner));
            SimulationRunner.BeginSimulation();

            _simulationDebugView = await _viewStack.Push<SimulationDebugViewController, SimulationDebugViewModel>(
                new SimulationDebugViewModel
                {
                    GetCurrentTick = () => SimulationRunner.Current.CurrentTick,
                });
        }

        public override async Task OnExit()
        {
            await _viewStack.PopAll();
            
            SimulationRunner.EndSimulation();
            Object.Destroy(_simulationRunnerGo);
            _simulationRunnerGo = null;
        }
    }
}