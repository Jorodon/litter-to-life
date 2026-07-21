using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Game.Audio
{
    public class PersistentAudioInstance
    {
        protected EventInstance instance;

        protected Transform transform;

        public bool IsPlaying {get; private set; }

        public PersistentAudioInstance(EventReference eventReference, Transform attach)
        {
            if (eventReference.IsNull)
            {
                return;
            }

            transform = attach;
            instance = RuntimeManager.CreateInstance(eventReference);

        }

        public virtual void PlayAudio()
        {
            if (IsPlaying)
            {
                return;
            }

            instance.start();
            IsPlaying = true;
        }

        public virtual void SetGlobalMusicParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }

        public virtual void SetGlobalMusicParameter(string parameterName, string name )
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameterName, name);
        }

        public virtual void SetMusicParameter(string parameterName, float value)
        {
            instance.setParameterByName(parameterName, value);
        }

        public virtual void SetMusicParameter(string parameterName, string name)
        {
            instance.setParameterByNameWithLabel(parameterName, name);
        }

        public virtual void Stop()
        {
            if (!IsPlaying)
            {
                return;
            }

            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            IsPlaying = false;
        }

        public virtual void Release()
        {
            Stop();
            instance.release();
        }
    }

}