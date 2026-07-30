using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    // Creates menu asset option for objectives - Go to Create -> Progression -> Objective
    [CreateAssetMenu(fileName = "trashScriptableObject", menuName = "Progression/Objective")]
    public class trashScriptableObject : ScriptableObject
    {
        // Objectives have an ID and name associated
        public string objectiveID;
        public string objectiveName;

        [Header("Milestones")]
        [SerializeField]
        private float[] milestonePercentages = { 25f, 50f, 75f };

        public IList<float> MilestonePercentages => milestonePercentages;

        public float GetCurrentProgress()
        {
            return trashCollectionManager.Instance.GetObjectiveProgress(this);
        }

        public bool CompleteObjective()
        {
            return trashCollectionManager.Instance.CompleteObjective(objectiveID);
        }
    }
}
