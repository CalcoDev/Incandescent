using System;
using UnityEngine;

namespace Incandescent.Components
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField] private float _time;
        [SerializeField] private bool _updateAutomatically;
        
        public float Time => _time;

        public bool UpdateAutomatically
        {
            get => _updateAutomatically;
            set => _updateAutomatically = value;
        }
        
        private void Update()
        {
            if (_updateAutomatically)
                _time += UnityEngine.Time.deltaTime;
        }

        public void UpdateTimer(float deltaTime)
        {
            _time += deltaTime;
        }
        
        public void StartTimer()
        {
            _updateAutomatically = true;
        }
        
        public void StopTimer()
        {
            _updateAutomatically = false;
        }
        
        public void SetTimer(float time)
        {
            _time = time;
        }
        
        public void ResetTimer()
        {
            _time = 0;
        }
    }
}