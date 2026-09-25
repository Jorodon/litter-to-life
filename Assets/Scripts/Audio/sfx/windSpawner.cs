using UnityEngine;
using FMODUnity;

namespace Game.Audio.Environment
{
    public class windSpawner : MonoBehaviour
    {
        // Serialized field to get wind event
        [Header("Wind Event")]
        [SerializeField] private EventReference windEventReference;

        //Serialized fields for wind control parameters
        [SerializeField] private float minWindRadius = 20f;     //Minimum radius wind will spawn around object
        [SerializeField] private float maxWindRadius = 40f;     //Max readius wind will spawn around object
        [SerializeField] private float directionChangeSpeed = 1f;   //Speed that wind position moves in 3D space
        [SerializeField] private float minSpeed = .25f;         //Min wind speed
        [SerializeField] private float maxSpeed = 1f;           // Max wind speed

        private Vector3 currentPos;
        private Vector3 nextPos;
        private windAudioManager windAudioWrapper;

        //Starts wind event
        private void Awake()
        {
            windAudioWrapper = new windAudioManager(windEventReference, transform);
        }

        // Calculates starting position, next position to move and plays wind sound
        private void Start()
        {
            currentPos = transform.position;
            CalculateNewPosition();

            windAudioWrapper.UpdatePosition(currentPos);
            windAudioWrapper.PlayAudio();
        }

        // Moves wind toward goal position, calculates next position if within range of goal
        private void Update()
        {
            currentPos = Vector3.MoveTowards(currentPos, nextPos, directionChangeSpeed * Time.deltaTime);

            windAudioWrapper.UpdatePosition(currentPos);

            if (Vector3.Distance(currentPos, nextPos) < 1f)
            {
                CalculateNewPosition();
            }
        
        }

        // Calculates a random position within donut around object and sets a random wind speed
        public void CalculateNewPosition()
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minWindRadius, maxWindRadius);
            nextPos = transform.position + new Vector3(randomDirection.x, 1.8f, randomDirection.y);
            float windSpeed = Random.Range(minSpeed, maxSpeed);
            windAudioWrapper.SetGlobalMusicParameter("Wind level", windSpeed);
        }

        // For debug, draws gizmos for wind spawn zones and wind sound itself
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minWindRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, maxWindRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(currentPos, 0.5f);
            Gizmos.DrawLine(currentPos, nextPos);
        }
    }
}