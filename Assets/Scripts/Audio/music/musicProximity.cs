using UnityEngine;
using Unity.XR.CoreUtils;

namespace Game.Audio.Music.Test
{
    public class musicProximity : MonoBehaviour
    {
        //XR Origin used to find player object
        private XROrigin xrOrigin;
        private GameObject playerObject;

        //Set the max distance (radius) used for dynamic track transition
        [SerializeField] private float maxDistance;

        private void Start()
        {
            xrOrigin = Object.FindFirstObjectByType<XROrigin>();
            if (xrOrigin != null)
            {
                playerObject = xrOrigin.Camera.gameObject; 
            }
        }

        // Calculates the clamped and normalized distance between player character and attached object
        void Update()
        {
            float currentDistance = Vector3.Distance(transform.position, playerObject.transform.position);
            float clampDistance = Mathf.Clamp(currentDistance, 0f, maxDistance);
            float normalizedDistance = 0f + Mathf.InverseLerp(0f, maxDistance, clampDistance);
            backgroundMusicListener.Instance.ChangeDistance(normalizedDistance);
        }
    }
}

