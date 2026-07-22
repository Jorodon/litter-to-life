using UnityEngine;
using FMODUnity;
using Game.Objective;
using System.Security.Cryptography;

namespace Game.Audio.Music
{
    public class backgroundMusicListener : MonoBehaviour
    {

        // Creates singleton access for background music across all scripts (might not need anymore)
        public static backgroundMusicListener Instance { get; private set; }

        // Serialized field to get initial music event
        [Header("Background Music Event")]
        [SerializeField] private EventReference musicEventReference;

        [SerializeField] private trashScriptableObject mainObjective;

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

        // Listens for a milestone to be reached
        private void OnEnable()
        {
            trashCollectionManager.OnMilestoneCompletion += HandleMilestoneCompletion;
        }

        // Removes milestone function to preserve memory
        private void OnDisable()
        {
            trashCollectionManager.OnMilestoneCompletion -= HandleMilestoneCompletion;
        }

        // Checks that applicable objective was completed, then increases music state
        private void HandleMilestoneCompletion(string objectiveID)
        {
            if (mainObjective != null && objectiveID == mainObjective.objectiveID)
            {
                IncreaseMusicState();
            }
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