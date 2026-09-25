using UnityEngine;
using FMODUnity;

namespace Game.Audio.Environment
{
    public class birdAudioInstance : MonoBehaviour
    {
        // Serialized field to get bird event
        [Header("Bird SFX Event")]
        [SerializeField] private EventReference birdEventReference;

        private PersistentAudioInstance birdAudioWrapper;


        //Creates a new bird event instance
        private void Awake()
        {
            birdAudioWrapper = new PersistentAudioInstance(birdEventReference, transform);
        }

        // Plays bird sound
        private void Start()
        {
            birdAudioWrapper.PlayAudio();
        }

                // For debug, draws gizmos for wind spawn zones and wind sound itself
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }
}