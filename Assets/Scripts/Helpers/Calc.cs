using UnityEngine;

namespace Incandescent.Helpers
{
    public static class Calc
    {
        public static bool SameSign(float a, float b)
        {
            return (a < 0) == (b < 0);
        }
        
        public static float Approach(float current, float target, float maxDelta)
        {
            return current < target ? Mathf.Min(current + maxDelta, target) : Mathf.Max(current - maxDelta, target);
        }
    }
}