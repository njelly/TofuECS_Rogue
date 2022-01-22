using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.ECSUnity;
using Tofunaut.TofuECS_Rogue.ECSUnity.UI;
using Tofunaut.TofuECS_Rogue.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = UnityEngine.InputSystem.PlayerInput;

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
        private readonly SimulationRunner _simulationRunner;
        
        private SimulationDebugViewController _simulationDebugView;
        private PlayerInput _playerInput;

        public InGameState(
            AppStateMachine appStateMachine, 
            ViewStack viewStack, 
            InputActionAsset inputActionAsset,
            SimulationRunner simulationRunner)
        {
            _appStateMachine = appStateMachine;
            _viewStack = viewStack;
            _inputActionAsset = inputActionAsset;
            _simulationRunner = simulationRunner;
        }
        
        public override async Task OnEnter(InGameStateRequest request)
        {
            _simulationRunner.BeginSimulation();

            _simulationDebugView = await _viewStack.Push<SimulationDebugViewController, SimulationDebugViewModel>(
                new SimulationDebugViewModel
                {
                    GetCurrentTick = () => _simulationRunner.Current.CurrentTick,
                    GetPlayerPosition = () =>
                    {
                        var playerUnitEntity = _simulationRunner.Current.GetSingletonComponent<Player>().UnitEntity;
                        return _simulationRunner.Current.Buffer<Unit>().Get(playerUnitEntity, out var unit)
                            ? Vector2Utils.ToUnityVector2(unit.CurrentPos)
                            : Vector2.zero;
                    },
                });
            
            _inputActionAsset.FindAction("Cancel").performed += Cancel_OnPerformed;
        }

        public override async Task OnExit()
        {
            _inputActionAsset.FindAction("Cancel").performed -= Cancel_OnPerformed;
            
            await _viewStack.PopAll();
            
            _simulationRunner.EndSimulation();
        }

        private void Cancel_OnPerformed(InputAction.CallbackContext obj)
        {
            _appStateMachine.EnterState(new StartScreenStateRequest());
        }
    }
}