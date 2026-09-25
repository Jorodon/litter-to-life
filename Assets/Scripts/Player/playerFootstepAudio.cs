using UnityEngine;
using FMODUnity;

namespace Game.Audio
{
    public class playerFootstepAudio : MonoBehaviour
    {
        [Header("Footstep Sounds")]
        [SerializeField] private EventReference footstepReference;

        private PersistentAudioInstance footstepAudioWrapper;

        // Initializes the base class to create wrapper functions
        private void Awake()
        {
            footstepAudioWrapper = new PersistentAudioInstance(footstepReference, transform);
        }

        // Updates the position with object
        private void Update()
        {
            footstepAudioWrapper?.Update();
        }

        // Starts playing footstep sounds
        public void StartFootsteps()
        {
            footstepAudioWrapper.SetGlobalMusicParameter("isWalking", 1f);
            footstepAudioWrapper.PlayAudio();
        }

        // Changes footstep sound surface parameter
        public void ChangeGroundMaterial(string material)
        {
            if (string.IsNullOrEmpty(material)) return;
            footstepAudioWrapper.SetGlobalMusicParameter("Surface", material);
        }

        // Stops playing footstep sounds
        public void StopFootsteps()
        {
            footstepAudioWrapper.SetGlobalMusicParameter("isWalking", 0f);
            footstepAudioWrapper.Stop();
        }

        // Releases the instance
        private void OnDestroy()
        {
            footstepAudioWrapper?.Release();
        }
    }
}