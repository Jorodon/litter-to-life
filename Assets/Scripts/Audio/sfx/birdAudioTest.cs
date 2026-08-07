using UnityEngine;
using Game.Objective;
using Game.Environment;

public class birdAudioTest : MonoBehaviour
{

    [SerializeField]
        private terrainDetailSO testBirds;

    [SerializeField] 
    private fogManager fogManagerTest;


     private void OnTriggerEnter(Collider other)
        {
            //testBirds?.CompleteObjective();
            //fogManagerTest.IncreaseFogState();
        }
}
