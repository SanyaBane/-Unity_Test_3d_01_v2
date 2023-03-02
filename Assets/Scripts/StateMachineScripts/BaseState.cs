using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.StateMachineScripts
{
    public abstract class BaseState : IState
    {
        private readonly string _stateNameCached;
        
        private readonly MonoBehaviour _containerMonoBehaviour;
        
        public bool DebugLogStates = false;

        public virtual bool IsAllowedToMove() => true;
        
        public float TimeInThisState { get; private set; }

        public BaseState(MonoBehaviour containerMonoBehaviour)
        {
            _containerMonoBehaviour = containerMonoBehaviour;
            
            _stateNameCached = this.GetType().Name;

            TimeInThisState = 0;
        }

        public virtual void TickState()
        {
            if (!_containerMonoBehaviour.gameObject.activeInHierarchy)
                return;
            
            TimeInThisState += Time.deltaTime;
        }

        public virtual void OnEnterState(IState previousState)
        {
            TimeInThisState = 0;
            
            if (DebugLogStates)
                Debug.Log($"OnEnterState - {_stateNameCached}");
        }

        public virtual void OnExitState()
        {
            if (DebugLogStates)
                Debug.Log($"OnExitState - {_stateNameCached}");
        }
    }
}
