using UnityEngine;
using Game.Objective;
using System.Collections.Generic;
using Game.Environment;

namespace Game.Audio.Environment
{
    public class birdAudioManager : MonoBehaviour
    {
        [SerializeField]
        private terrainDetailSO birdSO;

        [SerializeField]
        private terrainObjectManager terrainManager;

        [SerializeField]
        private int numberOfBirds = 5;

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
    }
}
