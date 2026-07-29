using UnityEngine;
using FMODUnity;

namespace Game.Audio.Environment
{
    public class birdAudioManager : MonoBehaviour
    {
        // Serialized field to get bird event
        [Header("Bird SFX Event")]
        [SerializeField] private EventReference birdEventReference;


        private Vector3 currentPos;
        private Vector3 nextPos;
        private PersistentAudioInstance birdAudioWrapper;

        //Starts wind event
        private void Awake()
        {
            birdAudioWrapper = new PersistentAudioInstance(birdEventReference, transform);
        }

        // Calculates starting position, next position to move and plays wind sound
        private void Start()
        {
            //currentPos = transform.position;
            //CalculateNewPosition();

            //birdAudioWrapper.UpdatePosition();
            birdAudioWrapper.PlayAudio();
        }

        // Moves wind toward goal position, calculates next position if within range of goal
        private void Update()
        {
            // currentPos = Vector3.MoveTowards(currentPos, nextPos, directionChangeSpeed * Time.deltaTime);

            //birdAudioWrapper.UpdatePosition(currentPos);

            // if (Vector3.Distance(currentPos, nextPos) < 1f)
            // {
            //     CalculateNewPosition();
            // }
        
        }

        // // Calculates a random position within donut around object and sets a random wind speed
        // public void CalculateNewPosition()
        // {
        //     Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minWindRadius, maxWindRadius);
        //     nextPos = transform.position + new Vector3(randomDirection.x, 1.8f, randomDirection.y);
        //     float windSpeed = Random.Range(minSpeed, maxSpeed);
        //     birdAudioWrapper.SetGlobalMusicParameter("Wind level", windSpeed);
        // }

        // // For debug, draws gizmos for wind spawn zones and wind sound itself
        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireSphere(transform.position, minWindRadius);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawWireSphere(transform.position, maxWindRadius);

        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireSphere(currentPos, 0.5f);
        //     Gizmos.DrawLine(currentPos, nextPos);
        // }
    }
}