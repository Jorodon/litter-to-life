using UnityEngine;
using System.Collections.Generic;

namespace Game.Objective
{
    public class trashCollectionManager : MonoBehaviour
    {
        //Sets up global instance for progression manager
        public static trashCollectionManager Instance { get; private set; }

        //Creates hashset for milestones for faster processing
        private HashSet<string> completedMilestones = new HashSet<string>();

        //Stores list of events that need to be fulfilled upon milestone completion
        public static event System.Action<string> OnMilestoneCompletion;

        //Assigns the instance
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        // On milestone completion, checks if it has already been completed and passes it to listeners
        public void CompleteMilestone(string objectiveID)
        {
            if (completedMilestones.Add(objectiveID)) 
            {
                OnMilestoneCompletion?.Invoke(objectiveID);
            }
        }

        // Helper function for listeners to check if a milestone has been completed yet
        public bool IsMilestoneCompleted(string objectiveID)
        {
            return completedMilestones.Contains(objectiveID);
        }
    }
}
