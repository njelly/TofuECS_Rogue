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
        public Func<Vector2> GetPlayerPosition;
    }
    
    public class SimulationDebugViewController : ViewController<SimulationDebugViewModel>
    {
        [SerializeField] private TextMeshProUGUI _fpsLabel;
        [SerializeField] private TextMeshProUGUI _currentTickLabel;
        [SerializeField] private TextMeshProUGUI _playerPositionLabel;

        private Func<int> _getCurrentTick;
        private Func<Vector2> _getPlayerPosition;
        private float _lastFpsUpdate;
        private int _framesPerSecondCounter;
        
        public override Task OnPushedToStack(SimulationDebugViewModel model)
        {
            _getCurrentTick = model.GetCurrentTick;
            _lastFpsUpdate = Time.time;
            _getPlayerPosition = model.GetPlayerPosition;
            
            return Task.CompletedTask;
        }

        private void Update()
        {
            UpdateFPSLabel();
            UpdateCurrentTickLabel();
            UpdatePlayerPositionLabel();
        }

        private void UpdateCurrentTickLabel()
        {
            _currentTickLabel.text = $"Tick: {_getCurrentTick?.Invoke()}";
        }

        private void UpdateFPSLabel()
        {
            _framesPerSecondCounter++;
            
            if (Time.time - _lastFpsUpdate < 1f)
                return;

            _lastFpsUpdate = Time.time;
            _fpsLabel.text = $"FPS: {_framesPerSecondCounter}";
            _framesPerSecondCounter = 0;
        }

        private void UpdatePlayerPositionLabel()
        {
            _playerPositionLabel.text = $"Player Pos: {_getPlayerPosition?.Invoke():F2}";
        }
    }
}