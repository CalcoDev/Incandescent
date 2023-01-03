using System;
using Incandescent.Core.Helpers;
using UnityEngine;

namespace Incandescent.Components
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField] private float _time;
        [SerializeField] private bool _updateAutomatically;
        
        public float Time => _time;

        /// <summary>
        /// If true, the timer will update in the Update method.
        /// </summary>
        public bool UpdateAutomatically
        {
            get => _updateAutomatically;
            set => _updateAutomatically = value;
        }
        
        private void Update()
        {
            if (_updateAutomatically)
                _time = Mathf.Max(_time - UnityEngine.Time.deltaTime, 0f);
        }

        public void UpdateTimer(float deltaTime)
        {
            _time = Mathf.Max(_time - deltaTime, 0f);
        }
        
        /// <summary>
        /// Sets the timer to the given value, and sets UpdateAutomatically to true.
        /// </summary>
        public void StartTimer(float time)
        {
            _updateAutomatically = true;
            _time = time;
        }
        
        /// <summary>
        /// Only works if UpdateAutomatically is true.
        /// </summary>
        public void PauseTimer()
        {
            _updateAutomatically = false;
        }

        public void SetTimer(float time)
        {
            _time = time;
        }
        
        public bool HasFinished()
        {
            return Calc.FloatEquals(_time, 0f);
        }
        
        public bool IsRunning()
        {
            return _time > 0f;
        }
    }
}