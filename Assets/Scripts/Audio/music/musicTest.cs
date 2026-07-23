using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

namespace Game.Audio.Music.Test
{
    public class musicTest : MonoBehaviour
    {

        // Serialized field to get music event
        [Header("New Music Event")]
        [SerializeField] private EventReference musicEventReference;
        private EventInstance musicEventInstance;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log("Playing initial music sample.");
            StartCoroutine(ExecuteAfterDelay(2.0f)); 

        }

        IEnumerator ExecuteAfterDelay(float delayInSeconds)
        {
            
            // if (backgroundMusicManager.Instance == null) 
            // {
            //     Debug.LogError("The Music Manager Instance is missing from the scene!");
            // }

            // This pauses the execution for the given seconds
            yield return new WaitForSeconds(delayInSeconds);

            Debug.Log("Changing parameters...");
            // backgroundMusicManager.Instance.SetGlobalMusicParameter("Distance", 0.5f);
            // backgroundMusicManager.Instance.SetGlobalMusicParameter("Music State", 4);

        }

        void Update() {
            //backgroundMusicManager.Instance.SetGlobalMusicParameter("Distance", distance);
           // backgroundMusicManager.Instance.SetGlobalMusicParameter("Music State", musicState);
        }
    }
}
