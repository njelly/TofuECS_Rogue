using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.ECSUnity.UI;
using Tofunaut.TofuECS_Rogue.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tofunaut.TofuECS_Rogue
{
    public class AppContext : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [Header("UI Views")]
        [SerializeField] private AssetReference _startScreenViewReference;
        [SerializeField] private AssetReference _simulationDebugViewReference;

        private async void Start()
        {
            var viewStack = new ViewStack(_canvas);
            viewStack.RegisterViewController<StartScreenViewController, StartScreenViewModel>(
                _startScreenViewReference);
            viewStack.RegisterViewController<SimulationDebugViewController, SimulationDebugViewModel>(
                _simulationDebugViewReference);
            
            var appStateMachine = new AppStateMachine();
            appStateMachine.RegisterState<StartScreenState, StartScreenStateRequest>(
                new StartScreenState(appStateMachine, viewStack));
            appStateMachine.RegisterState<InGameState, InGameStateRequest>(new InGameState(appStateMachine, viewStack));

            await appStateMachine.EnterState(new StartScreenStateRequest());
        }
    }
}