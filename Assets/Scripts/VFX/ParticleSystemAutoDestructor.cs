using UnityEngine;

// Add this to the top object in a particle effect to make it automatically destroy when completed.
// Note: Will not work if the particle system is looping.

//https://www.youtube.com/watch?v=yQkG4p3bizA&t=828s&ab_channel=PatchQuest
namespace Assets.Scripts.VFX
{
    public class ParticleSystemAutoDestructor : MonoBehaviour
    {
        // The particle system on this object (if one exists)
        private ParticleSystem system;

        void Update()
        {
            // Try to extract a particle system from the specified root object (First time only)
            if (system == null)
            {
                system = GetComponent<ParticleSystem>();
            }

            // Test whether the particle system should be destroyed now (Checks every frame)
            if (system != null && !system.IsAlive(true))
            {
                Destroy(gameObject);
            }
        }
    }
}