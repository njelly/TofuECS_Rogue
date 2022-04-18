using UnityEngine;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.ECSUnity;

namespace Tofunaut.TofuECS_Rogue
{
    public class AppContext : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [Header("Game")]
        [SerializeField] private RogueTile _rogueTile;

        private AppStateMachine _appStateMachine;
        private CanvasStack _canvasStack;

        private async void Awake()
        {
            _canvasStack = new CanvasStack(_canvas);
            _appStateMachine = new AppStateMachine();
            
            _appStateMachine.RegisterState<InGameState, InGameStateRequest>(new InGameState(_rogueTile));

            await _appStateMachine.EnterState(new InGameStateRequest());
        }
    }
}