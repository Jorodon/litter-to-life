using UnityEngine;

namespace Game.Objective
{
    //Creates menu asset option for objectives - Go to Create -> Progression -> Objective
    [CreateAssetMenu(fileName = "trashScriptableObject", menuName = "Progression/Objective")]
    public class trashScriptableObject : ScriptableObject
    {
        // Objectives have an ID and name associated
        public string objectiveID;
        public string objectiveName;
            
        // Sends the objective ID to progression manager upon objective being completed
        public void CompleteObjective()
        {
            trashCollectionManager.Instance.CompleteMilestone(objectiveID);
        }
    }
}
