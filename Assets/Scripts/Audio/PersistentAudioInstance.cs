using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Game.Audio
{
    public class PersistentAudioInstance
    {
        protected EventInstance instance;

        protected Transform transform;

        // Constructor: initializes class and creates FMOD instance
        public PersistentAudioInstance(EventReference eventReference, Transform attach)
        {
            if (eventReference.IsNull)
            {
                return;
            }

            transform = attach;
            instance = RuntimeManager.CreateInstance(eventReference);
            UpdatePosition();
        }

        //Updates position of audio
        public virtual void Update()
        {
            UpdatePosition();
        }

        //Checks if audio is not playing and plays audio
        public virtual void PlayAudio()
        {
            if (!instance.isValid()) return;

            instance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                instance.start();
            }
        }

        // Checks if audio is playing and stops playback
        public virtual void Stop()
        {
            if (!instance.isValid()) return;

            instance.getPlaybackState(out PLAYBACK_STATE state);
            if (state != PLAYBACK_STATE.STOPPED)
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        // Sets a global parameter (continuous/discrete)
        public virtual void SetGlobalMusicParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }

        // Sets a global parameter (label)
        public virtual void SetGlobalMusicParameter(string parameterName, string name)
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameterName, name);
        }

        // Sets a local parameter (continuous/discrete)
        public virtual void SetMusicParameter(string parameterName, float value)
        {
            if (instance.isValid()) instance.setParameterByName(parameterName, value);
        }

        // Sets a local parameter (label)
        public virtual void SetMusicParameter(string parameterName, string name)
        {
            if (instance.isValid()) instance.setParameterByNameWithLabel(parameterName, name);
        }

        // Updates position (based on parent object)
        protected void UpdatePosition()
        {
            if (transform == null || !instance.isValid()) return;
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }

        // Updates position (based on input vector)
        public virtual void UpdatePosition(Vector3 position)
        {
            if (transform == null || !instance.isValid()) return;
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        }

        // Stops audio playback and releases FMOD instance
        public virtual void Release()
        {
            if (!instance.isValid()) return;
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }
}