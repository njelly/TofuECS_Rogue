using System;
using System.Threading.Tasks;
using TMPro;
using Tofunaut.Bootstrap;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity.UI
{
    public class SimulationDebugViewModel
    {
        public Func<int> GetCurrentTick;
    }
    
    public class SimulationDebugViewController : ViewController<SimulationDebugViewModel>
    {
        [SerializeField] private TextMeshProUGUI _fpsLabel;
        [SerializeField] private TextMeshProUGUI _currentTickLabel;

        private Func<int> _getCurrentTick;
        private float _lastFpsUpdate;
        private int _framesPerSecondCounter;
        
        public override Task OnPushedToStack(SimulationDebugViewModel model)
        {
            _getCurrentTick = model.GetCurrentTick;
            _lastFpsUpdate = Time.time;
            
            return Task.CompletedTask;
        }

        private void Update()
        {
            UpdateFPSLabel();
            UpdateCurrentTickLabel();
        }

        private void UpdateCurrentTickLabel()
        {
            if (!_currentTickLabel)
                return;
            
            _currentTickLabel.text = $"Tick: {_getCurrentTick?.Invoke()}";
        }

        private void UpdateFPSLabel()
        {
            if (!_fpsLabel)
                return;

            _framesPerSecondCounter++;
            
            if (Time.time - _lastFpsUpdate < 1f)
                return;

            _lastFpsUpdate = Time.time;
            _fpsLabel.text = $"FPS: {_framesPerSecondCounter}";
            _framesPerSecondCounter = 0;
        }
    }
}