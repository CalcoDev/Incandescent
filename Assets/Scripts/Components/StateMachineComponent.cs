using System;
using UnityEngine;

namespace Incandescent.Components
{
    public class StateMachineComponent : MonoBehaviour
    {
        [SerializeField] private int _state;
        [SerializeField] private int _previousState;

        private Action[] _enters;
        private Func<int>[] _updates;
        private Action[] _fixedUpdates;
        private Action[] _exits;

        private int _stateCount;

        public int State => _state;

        public void Init(int maxStates, int defaultState)
        {
            _stateCount = maxStates;
            
            _enters = new Action[maxStates];
            _updates = new Func<int>[maxStates];
            _fixedUpdates = new Action[maxStates];
            _exits = new Action[maxStates];
            
            _previousState = _state = defaultState;
        }

        public void SetState(int state)
        {
            if (_state == state)
                return;
            
            if (state < 0 || state >= _stateCount)
                throw new ArgumentOutOfRangeException(nameof(state), "StateMachineComponent: State out of range.");

            _previousState = _state;
            _state = state;
            
            if (_previousState != -1 && _exits[_previousState] != null)
                _exits[_previousState].Invoke();

            if (_enters[_state] != null)
                _enters[_state].Invoke();
        }
        
        public void SetCallbacks(int index, Func<int> onUpdate, Action onEnter = null, Action onExit = null, Action onFixedUpdate = null)
        {
            _enters[index] = onEnter;
            _updates[index] = onUpdate;
            _fixedUpdates[index] = onFixedUpdate;
            _exits[index] = onExit;
        }
    }
}