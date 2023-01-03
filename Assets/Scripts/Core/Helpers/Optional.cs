using System;
using UnityEngine;

namespace Incandescent.Core.Helpers
{
    [Serializable]
    public class Optional<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool Enabled => enabled;
        public T Value => value;

        public Optional(T initialValue)
        {
            enabled = true;
            value = initialValue;
        }
    }
}