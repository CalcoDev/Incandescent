using System;
using System.Collections;
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
        
        private Func<IEnumerator>[] _coroutines;

        private int _stateCount;
        
        private Coroutine _currentCoroutine;

        public int State => _state;

        public void Init(int maxStates, int defaultState)
        {
            _stateCount = maxStates;
            
            _enters = new Action[maxStates];
            _updates = new Func<int>[maxStates];
            _fixedUpdates = new Action[maxStates];
            _exits = new Action[maxStates];
            
            _coroutines = new Func<IEnumerator>[maxStates];
            
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

            if (_coroutines[_state] != null)
            {
                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                
                _currentCoroutine = StartCoroutine(_coroutines[_state].Invoke());
            }
        }
        
        public int RunUpdate()
        {
            if (_updates[_state] != null)
                return _updates[_state].Invoke();

            return _state;
        }
        
        public void RunFixedUpdate()
        {
            if (_fixedUpdates[_state] != null)
                _fixedUpdates[_state].Invoke();
        }
        
        public void SetCallbacks(int index, Func<int> update, Action enter = null, Action exit = null, Action fixedUpdate = null, Func<IEnumerator> coroutine = null)
        {
            _enters[index] = enter;
            _updates[index] = update;
            _fixedUpdates[index] = fixedUpdate;
            _exits[index] = exit;
            
            _coroutines[index] = coroutine;
        }
    }
}