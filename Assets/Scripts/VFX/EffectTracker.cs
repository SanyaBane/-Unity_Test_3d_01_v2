using UnityEngine;
using System.Collections.Generic;

// Allows particle effects to follow a GameObject without being parented to that GameObject.
// This way, destroying the track target doesn't also destroy the effect.

namespace Assets.Scripts.VFX
{
//https://www.youtube.com/watch?v=yQkG4p3bizA&t=828s&ab_channel=PatchQuest
    public class EffectTracker : MonoBehaviour
    {
        // A list of effect roots and their corresponding targets
        private static List<(GameObject effect, GameObject target)> trackers = new List<(GameObject, GameObject)>();

        // Add an effect to the list
        public static void Track(GameObject effect, GameObject target)
        {
            trackers.Add((effect, target));
        }

        // Update all tracked effects
        void Update()
        {
            // Loop backwards through every tracked effect
            for (int i = trackers.Count - 1; i >= 0; i--)
            {
                var (effect, target) = trackers[i];

                // If either the effect or the target has been destroyed...
                if (effect == null || target == null)
                {
                    // Stop tracking this effect
                    trackers.RemoveAt(i);
                }

                // Otherwise...
                else
                {
                    // Change the world position of the effect
                    effect.transform.position = target.transform.position;
                }
            }
        }
    }
}