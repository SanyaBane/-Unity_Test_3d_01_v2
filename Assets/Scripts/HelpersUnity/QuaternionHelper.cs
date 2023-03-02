using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class QuaternionHelper
    {
        private const float AlmostEqualAcceptableRange = 0.0001f;
        
        // http://answers.unity.com/answers/1670751/view.html
        public static bool ApproximatelyEqualTo(this Quaternion quatA, Quaternion value, float acceptableRange)
        {
            var tmp = 1 - Mathf.Abs(Quaternion.Dot(quatA, value));
            var ret = tmp < acceptableRange;
            return ret;
        }
        
        public static bool AlmostEqualTo(this Quaternion quatA, Quaternion value)
        {
            return ApproximatelyEqualTo(quatA, value, AlmostEqualAcceptableRange);
        }
    }
}