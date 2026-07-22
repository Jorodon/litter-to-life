using UnityEngine;
using FMODUnity;

namespace Game.Audio.Music
{
    public class backgroundMusicTest : MonoBehaviour
    {

        // Creates singleton access for background music across all scripts
        public static backgroundMusicTest Instance { get; private set; }

        // Serialized field to get initial music event
        [Header("Background Music Event")]
        [SerializeField] private EventReference musicEventReference;

        private backgroundMusicManager backgroundMusicWrapper;

        // Runs before Start() to prevent duplicates, set a single Instance, and ensure it survives across scenes
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            backgroundMusicWrapper = new backgroundMusicManager(musicEventReference, transform);
        }

        // Starts the selected music event
        private void Start()
        {
            backgroundMusicWrapper.PlayAudio();
        }

        // Set a custom music state value
        public void SetMusicState(int value)
        {
            backgroundMusicWrapper.musicState = value;
            backgroundMusicWrapper.SetGlobalMusicParameter("Music State", value);
        }

        // Increase the music state value by 1
        public void IncreaseMusicState()
        {
            backgroundMusicWrapper.musicState++;
            Debug.Log("New music state:");
            Debug.Log(backgroundMusicWrapper.musicState);
            backgroundMusicWrapper.SetGlobalMusicParameter("Music State", backgroundMusicWrapper.musicState);
        }

        // Set the distance value
        public void ChangeDistance(float value)
        {
            backgroundMusicWrapper.distance = value;
            backgroundMusicWrapper.SetGlobalMusicParameter("Distance", backgroundMusicWrapper.distance);
        }

        // Plays the selected music event
        public void PlayMusic()
        {
            backgroundMusicWrapper.PlayAudio();
        }

        // Stops playback of the current music event
        public void StopMusic()
        {
            backgroundMusicWrapper.Stop();
        }

        // Stops playback of current music event and changes to a different music event
        public void ChangeTrack(EventReference newEventReference)
        {
            backgroundMusicWrapper.SwitchMusicTrack(newEventReference);
        }

        // Stops playback of music event when object is destroyed
        private void OnDestroy()
        {
            backgroundMusicWrapper.Release();
        }
    }

}