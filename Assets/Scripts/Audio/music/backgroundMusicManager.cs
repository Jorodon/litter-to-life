using UnityEngine;
using FMODUnity;

namespace Game.Audio.Music
{
    public class backgroundMusicManager : PersistentAudioInstance
    {
        public int musicState;
        public float distance;

        // Constructor: Inherits from PersistentAudioInstance and adds variables for music state and distance
        public backgroundMusicManager(EventReference eventReference, Transform attach, int musicStateInput = 0, float distanceInput = 1) 
            : base(eventReference, attach)
        {
            musicState = musicStateInput;
            distance = distanceInput;
        }

        // Switches music track to a new track
        public void SwitchMusicTrack(EventReference newEventReference)
        {
            if (newEventReference.IsNull)
            {
                Debug.Log("Null event reference passed.");
                return;
            }

            if (instance.isValid())
            {
                Stop();
                Release();
            }

            instance = RuntimeManager.CreateInstance(newEventReference);
            PlayAudio();

        }
    }
}