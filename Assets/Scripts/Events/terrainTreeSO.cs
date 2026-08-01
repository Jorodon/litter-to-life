using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    // Creates menu asset option for objectives - Go to Create -> Scriptable Objects -> terrainDetailSO
    [CreateAssetMenu(fileName = "terrainTreeSO", menuName = "Scriptable Objects/terrainTreeSO")]
    public class terrainTreeSO : ScriptableObject
    {
        // Objectives have an ID and name associated
        public string objectiveID;
        public string objectName;

        //[SerializeField]
        //public List<GameObject> relaventObjects = new List<GameObject>();
        [SerializeField]
        public List<treePairs> relaventObjects = new List<treePairs>();

        // Sends the objective ID to progression manager upon objective being completed
        public bool CompleteObjective()
        {
            return trashCollectionManager.Instance.CompleteObjective(objectiveID);
        }
    }


    public class treePairs
    {
        GameObject originalPrefab;
        GameObject newPrefab;
    }
}
