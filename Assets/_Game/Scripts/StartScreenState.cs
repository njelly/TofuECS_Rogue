using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Rogue.UI;

namespace Tofunaut.TofuECS_Rogue
{
    public class StartScreenStateRequest
    {
        
    }
    
    public class StartScreenState : AppState<StartScreenStateRequest>
    {
        private readonly AppStateMachine _stateMachine;
        private readonly ViewStack _viewStack;

        private StartScreenViewController _viewController;

        public StartScreenState(AppStateMachine stateMachine, ViewStack viewStack)
        {
            _stateMachine = stateMachine;
            _viewStack = viewStack;
        }
        
        public override async Task OnEnter(StartScreenStateRequest request)
        {
            _viewController =
                await _viewStack.Push<StartScreenViewController, StartScreenViewModel>(new StartScreenViewModel
                {
                    OnPlay = OnPlay,
                });
        }

        public override async Task OnExit()
        {
            await _viewStack.PopAll();
        }

        private async void OnPlay()
        {
            await _stateMachine.EnterState(new InGameStateRequest());
        }
    }
}