using System;
using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.TofuECS_Rogue.UI
{
    public class StartScreenViewModel
    {
        public Action OnPlay;
    }
    
    public class StartScreenViewController : ViewController<StartScreenViewModel>
    {
        [SerializeField] private Button _playButton;
        
        public override Task OnPushedToStack(StartScreenViewModel model)
        {
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() => model.OnPlay?.Invoke());
            
            return Task.CompletedTask;
        }
    }
}