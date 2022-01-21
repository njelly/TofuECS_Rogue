using System;
using System.Collections.Generic;

namespace Tofunaut.TofuECS_Rogue.Utilities
{
    public delegate void EnumStateMachineOnEnter<in TEnum>(TEnum prevState) where TEnum : Enum;
    public delegate void EnumStateMachineOnExit<in TEnum>(TEnum nextState) where TEnum : Enum;
    
    public class EnumStateMachine<TEnum> where TEnum : Enum
    {
        public TEnum CurrentState { get; private set; }
        
        private readonly Dictionary<TEnum, EnumStateMachineOnEnter<TEnum>> _enumValToOnEnter;
        private readonly Dictionary<TEnum, EnumStateMachineOnExit<TEnum>> _enumValToOnExit;

        public EnumStateMachine()
        {
            _enumValToOnEnter = new Dictionary<TEnum, EnumStateMachineOnEnter<TEnum>>();
            _enumValToOnExit = new Dictionary<TEnum, EnumStateMachineOnExit<TEnum>>();
        }

        public void RegisterTransition(TEnum enumVal, EnumStateMachineOnEnter<TEnum> onEnter,
            EnumStateMachineOnExit<TEnum> onExit)
        {
            _enumValToOnEnter.Add(enumVal, onEnter);
            _enumValToOnExit.Add(enumVal, onExit);
        }

        public void Enter(TEnum enumVal)
        {
            if(_enumValToOnExit.TryGetValue(CurrentState, out var onExit))
                onExit?.Invoke(enumVal);
            
            if (_enumValToOnEnter.TryGetValue(enumVal, out var onEnter))
                onEnter?.Invoke(CurrentState);

            CurrentState = enumVal;
        }
    }
}