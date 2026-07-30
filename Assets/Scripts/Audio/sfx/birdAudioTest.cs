using UnityEngine;
using Game.Objective;

public class birdAudioTest : MonoBehaviour
{

    [SerializeField]
        private terrainDetailSO testBirds;

     private void OnTriggerEnter(Collider other)
        {
            testBirds?.CompleteObjective();
        }
}
