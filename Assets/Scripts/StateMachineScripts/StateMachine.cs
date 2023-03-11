using System;
using System.Collections.Generic;

namespace Assets.Scripts.StateMachineScripts
{
   // Notes
   // 1. What a finite state machine is
   // 2. Examples where you'd use one
   //     AI, Animation, Game State
   // 3. Parts of a State Machine
   //     States & Transitions
   // 4. States - 3 Parts
   //     Tick - Why it's not Update()
   //     OnEnter / OnExit (setup & cleanup)
   // 5. Transitions
   //     Separated from states so they can be re-used
   //     Easy transitions from any state

   public class StateMachine
   {
      public IState _currentState { get; private set; }
      
      private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type,List<Transition>>();
      private List<Transition> _currentTransitions = new List<Transition>();
      private List<Transition> _anyTransitions = new List<Transition>();
      
      private static readonly List<Transition> EmptyTransitions = new List<Transition>(0);

      public Action<IState,IState> OnStateChanged;

      public void Tick()
      {
         var transition = GetTransition();
         if (transition != null)
            SetState(transition.To);
         
         _currentState?.TickState();
      }

      public void SetState(IState state)
      {
         if (state == _currentState)
            return;

         IState previousState = _currentState;
         
         _currentState?.OnExitState();
         _currentState = state;
         
         _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
         if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;
         
         _currentState.OnEnterState(previousState);
         
         OnStateChanged?.Invoke(_currentState, previousState);
      }

      public void AddTransition(IState from, IState to, Func<bool> predicate)
      {
         if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
         {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
         }
         
         transitions.Add(new Transition(to, predicate));
      }

      public void AddAnyTransition(IState state, Func<bool> predicate)
      {
         _anyTransitions.Add(new Transition(state, predicate));
      }

      private class Transition
      {
         public Func<bool> Condition {get; }
         public IState To { get; }

         public Transition(IState to, Func<bool> condition)
         {
            To = to;
            Condition = condition;
         }
      }

      private Transition GetTransition()
      {
         foreach(var transition in _anyTransitions)
            if (transition.Condition())
               return transition;
         
         foreach (var transition in _currentTransitions)
            if (transition.Condition())
               return transition;

         return null;
      }
   }
}