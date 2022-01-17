using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.ECSUnity;
using Tofunaut.TofuECS_Rogue.ECSUnity.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tofunaut.TofuECS_Rogue
{
    public class InGameStateRequest
    {
        
    }

    public class InGameState : AppState<InGameStateRequest>
    {
        private readonly AppStateMachine _appStateMachine;
        private readonly ViewStack _viewStack;
        private readonly InputActionAsset _inputActionAsset;
        
        private GameObject _simulationRunnerGo;
        private SimulationDebugViewController _simulationDebugView;

        public InGameState(
            AppStateMachine appStateMachine, 
            ViewStack viewStack, 
            InputActionAsset inputActionAsset)
        {
            _appStateMachine = appStateMachine;
            _viewStack = viewStack;
            _inputActionAsset = inputActionAsset;
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
            
            _inputActionAsset.FindAction("Cancel").performed += Cancel_OnPerformed;
        }

        private void Cancel_OnPerformed(InputAction.CallbackContext obj)
        {
            _appStateMachine.EnterState(new StartScreenStateRequest());
        }

        public override async Task OnExit()
        {
            _inputActionAsset.FindAction("Cancel").performed -= Cancel_OnPerformed;
            
            await _viewStack.PopAll();
            
            SimulationRunner.EndSimulation();
            Object.Destroy(_simulationRunnerGo);
            _simulationRunnerGo = null;
        }
    }
}