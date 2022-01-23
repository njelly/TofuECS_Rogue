using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Tofunaut.TofuECS_Rogue.ECS.PlayerInput;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class PlayerInputManager 
    {
        private InputActionAsset _inputActionAsset;
        private PlayerInput _current;

        public void Enable(InputActionAsset inputActionAsset)
        {
            if (_inputActionAsset != null)
            {
                Debug.LogError("PlayerInputManager has already subscribed.");
                return;
            }

            _inputActionAsset = inputActionAsset;
            
            var moveAction = _inputActionAsset.FindAction("Player/Move");
            moveAction.started += Move_Started;
            moveAction.performed += Move_Performed;
            moveAction.canceled += Move_Canceled;
            moveAction.Enable();

            var sprintAction = _inputActionAsset.FindAction("Player/Sprint");
            sprintAction.performed += Sprint_Performed;
            sprintAction.canceled += Sprint_Canceled;
            sprintAction.Enable();
        }

        public void Disable()
        {
            if (_inputActionAsset == null)
            {
                Debug.LogError("PlayerInputManager has not yet subscribed.");
                return;
            }

            _current = default;
            
            var moveAction = _inputActionAsset.FindAction("Player/Move");
            moveAction.started -= Move_Started;
            moveAction.performed -= Move_Performed;
            moveAction.canceled -= Move_Canceled;
            moveAction.Disable();

            var sprintAction = _inputActionAsset.FindAction("Player/Sprint");
            sprintAction.performed -= Sprint_Performed;
            sprintAction.canceled -= Sprint_Canceled;
            sprintAction.Disable();

            _inputActionAsset = null;
        }

        public void Poll(out PlayerInput current)
        {
            _current.DeltaTime = Time.deltaTime;
            current = _current;
        }
        
        private void Move_Started(InputAction.CallbackContext obj)
        {
            _current.UnitInput.DirMagnitude = UnitInput.FaceThreshold;
            var v = obj.ReadValue<Vector2>();
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
                _current.UnitInput.Dir = v.x > 0 ? CardinalDirection4.East : CardinalDirection4.West;
            else
                _current.UnitInput.Dir = v.y > 0 ? CardinalDirection4.North : CardinalDirection4.South;
        }

        private void Move_Performed(InputAction.CallbackContext obj)
        {
            _current.UnitInput.DirMagnitude = UnitInput.MoveThreshold;
            var v = obj.ReadValue<Vector2>();
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
                _current.UnitInput.Dir = v.x > 0 ? CardinalDirection4.East : CardinalDirection4.West;
            else
                _current.UnitInput.Dir = v.y > 0 ? CardinalDirection4.North : CardinalDirection4.South;
        }

        private void Move_Canceled(InputAction.CallbackContext obj)
        {
            _current.UnitInput.DirMagnitude = 0;
            _current.UnitInput.Dir = CardinalDirection4.None;
        }

        private void Sprint_Performed(InputAction.CallbackContext obj)
        {
            if (_current.UnitInput.DirMagnitude == UnitInput.MoveThreshold)
                _current.UnitInput.DirMagnitude = UnitInput.SprintThreshold;
        }

        private void Sprint_Canceled(InputAction.CallbackContext obj)
        {
            if (_current.UnitInput.DirMagnitude == UnitInput.SprintThreshold)
                _current.UnitInput.DirMagnitude = UnitInput.MoveThreshold;
        }
    }
}