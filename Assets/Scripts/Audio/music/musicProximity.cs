using UnityEngine;

namespace Game.Audio.Music.Test
{
    public class musicProximity : MonoBehaviour
    {
        
        [SerializeField] private Transform playerTransform;
        //Set the max distance (radius) used for dynamic track transition
        [SerializeField] private float maxDistance;

        // Calculates the clamped and normalized distance between player character and attached object
        void Update()
        {
            float currentDistance = Vector3.Distance(transform.position, playerTransform.position);
            float clampDistance = Mathf.Clamp(currentDistance, 0f, maxDistance);
            float normalizedDistance = 0f + Mathf.InverseLerp(0f, maxDistance, clampDistance);
            backgroundMusicTest.Instance.ChangeDistance(normalizedDistance);
        }
    }
}

