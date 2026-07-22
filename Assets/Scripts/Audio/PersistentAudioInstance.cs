using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Game.Audio
{
    public class PersistentAudioInstance
    {
        protected EventInstance instance;

        protected Transform transform;

        public bool IsPlaying {get; private set; }

        // Constructor: initializes class and creates FMOD instance
        public PersistentAudioInstance(EventReference eventReference, Transform attach)
        {
            if (eventReference.IsNull)
            {
                return;
            }

            transform = attach;
            instance = RuntimeManager.CreateInstance(eventReference);

        }

        //Checks if audio is not playing and plays audio
        public virtual void PlayAudio()
        {
            if (IsPlaying)
            {
                return;
            }

            instance.start();
            IsPlaying = true;
        }

        // Sets a global parameter (continuous/discrete)
        public virtual void SetGlobalMusicParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }

        // Sets a global parameter (label)
        public virtual void SetGlobalMusicParameter(string parameterName, string name )
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameterName, name);
        }

        // Sets a local parameter (continuous/discrete)
        public virtual void SetMusicParameter(string parameterName, float value)
        {
            instance.setParameterByName(parameterName, value);
        }

        // Sets a local parameter (label)
        public virtual void SetMusicParameter(string parameterName, string name)
        {
            instance.setParameterByNameWithLabel(parameterName, name);
        }

        // Checks if audio is playing and stops playback
        public virtual void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            IsPlaying = false;
        }

        // Stops audio playback and releases FMOD instance
        public virtual void Release()
        {
            Stop();
            instance.release();
        }
    }

}