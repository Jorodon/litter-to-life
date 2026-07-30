using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    // Creates menu asset option for objectives - Go to Create -> Scriptable Objects -> terrainDetailSO
    [CreateAssetMenu(fileName = "terrainDetailSO", menuName = "Scriptable Objects/terrainDetailSO")]
    public class terrainDetailSO : ScriptableObject
    {
        // Objectives have an ID and name associated
        public string objectiveID;
        public string objectName;

        [SerializeField]
        public List<GameObject> relaventObjects = new List<GameObject>();

        // Sends the objective ID to progression manager upon objective being completed
        public bool CompleteObjective()
        {
            return trashCollectionManager.Instance.CompleteObjective(objectiveID);
        }
    }
}
