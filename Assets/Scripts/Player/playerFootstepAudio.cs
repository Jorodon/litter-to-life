using UnityEngine;
using FMODUnity;
using Game.Audio;

namespace Game.Player
{
    public class playerFootstepAudio : MonoBehaviour
    {
        [Header("Footstep Sounds")]
        [SerializeField] private EventReference footstepReference;

        private PersistentAudioInstance footstepAudioWrapper;

        // Initializes the base class to create wrapper functions
        public void Awake()
        {
            footstepAudioWrapper = new PersistentAudioInstance(footstepReference, transform);
        }

        // Starts playing footstep sounds
        public void StartFootsteps()
        {
            footstepAudioWrapper.SetGlobalMusicParameter("isWalking", 1);
            footstepAudioWrapper.PlayAudio();
        }

        // Changes footstep sound surface parameter
        public void ChangeGroundMaterial(string material)
        {
            footstepAudioWrapper.SetMusicParameter("Surface", material);
        }

        // Stops playing footstep sounds
        public void StopFootsteps()
        {
            footstepAudioWrapper.SetGlobalMusicParameter("isWalking", 0);
            footstepAudioWrapper.Stop();
        }

        // Releases the instance
        private void OnDestroy()
        {
            footstepAudioWrapper.Release();
        }
    }
}