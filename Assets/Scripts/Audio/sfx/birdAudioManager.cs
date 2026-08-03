using UnityEngine;
using Game.Objective;
using System.Collections.Generic;
using Game.Environment;

namespace Game.Audio.Environment
{
    public class birdAudioManager : MonoBehaviour
    {
        // // Serialized field to get bird event
        // [Header("Bird SFX Event")]
        // [SerializeField] private EventReference birdEventReference;

        // private PersistentAudioInstance birdAudioWrapper;

        // List of terrain scriptable objects
        [SerializeField]
        private terrainDetailSO birdSO;

        [SerializeField]
        private terrainObjectManager terrainManager;

        [SerializeField]
        private int numberOfBirds = 5;


        // //Starts wind event
        // private void Awake()
        // {
        //     birdAudioWrapper = new PersistentAudioInstance(birdEventReference, transform);
        // }

        // Listens for a milestone to be reached
        // private void OnEnable()
        // {
        //     trashCollectionManager.OnObjectiveCompleted += HandleMilestoneCompletion;
        // }

        // // Removes milestone function to preserve memory
        // private void OnDisable()
        // {
        //     trashCollectionManager.OnObjectiveCompleted -= HandleMilestoneCompletion;
        // }

        // // Checks that applicable objective was completed, then adds corresponding
        // private void HandleMilestoneCompletion(string objectiveID)
        // {
        //     if (birdSO != null && objectiveID == birdSO.objectiveID)
        //     {
        //         List<Vector3> birdSpawnLocations = terrainManager.GetRandomTreeLocation(numberOfBirds);
                
        //          foreach (Vector3 instance in birdSpawnLocations)
        //         {
        //             Debug.Log("Spawning bird at position: " + instance);
        //             Instantiate(birdSO.relaventObjects[0], instance, Quaternion.identity);
        //         }
        //     }
        // }

        // Checks that applicable objective was completed, then adds corresponding
        public void SpawnBirds()
        {
            if (birdSO != null)
            {
                List<Vector3> birdSpawnLocations = terrainManager.GetRandomTreeLocation(numberOfBirds);
                
                 foreach (Vector3 instance in birdSpawnLocations)
                {
                    Debug.Log("Spawning bird at position: " + instance);
                    Instantiate(birdSO.relaventObjects[0], instance, Quaternion.identity);
                }
            }
        }


        // // Calculates starting position, next position to move and plays wind sound
        // private void Start()
        // {
        //     //currentPos = transform.position;
        //     //CalculateNewPosition();

        //     //birdAudioWrapper.UpdatePosition();
        //     birdAudioWrapper.PlayAudio();
        // }

        // // Moves wind toward goal position, calculates next position if within range of goal
        // private void Update()
        // {
        //     // currentPos = Vector3.MoveTowards(currentPos, nextPos, directionChangeSpeed * Time.deltaTime);

        //     //birdAudioWrapper.UpdatePosition(currentPos);

        //     // if (Vector3.Distance(currentPos, nextPos) < 1f)
        //     // {
        //     //     CalculateNewPosition();
        //     // }
        
        // }

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
