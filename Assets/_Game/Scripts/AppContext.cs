using UnityEngine;
using Tofunaut.Bootstrap;

namespace Tofunaut.TofuECS_Rogue
{
    public class AppContext : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        private AppStateMachine _appStateMachine;
        private CanvasStack _canvasStack;

        private async void Awake()
        {
            _canvasStack = new CanvasStack(_canvas);
            _appStateMachine = new AppStateMachine();
            
            _appStateMachine.RegisterState<InGameState, InGameStateRequest>(new InGameState());

            await _appStateMachine.EnterState(new InGameStateRequest());
        }
    }
}