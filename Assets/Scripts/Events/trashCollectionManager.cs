using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    public class trashCollectionManager : MonoBehaviour
    {
        // Sets up global instance for progression manager
        public static trashCollectionManager Instance { get; private set; }

        // Creates hashsets for milestones and trash tracking for faster processing
        private HashSet<string> completedMilestones = new HashSet<string>();
        private HashSet<trashObjectiveTrigger> trackedTrash = new HashSet<trashObjectiveTrigger>();
        private HashSet<trashObjectiveTrigger> collectedTrash =
            new HashSet<trashObjectiveTrigger>();

        // Stores list of events that need to be fulfilled upon milestone completion
        public static event System.Action<string> OnMilestoneCompletion;
        public int TotalTrashCount => trackedTrash.Count;
        public int CollectedTrashCount => collectedTrash.Count;
        public float CollectionPercentage =>
            TotalTrashCount == 0 ? 0f : (float)CollectedTrashCount / TotalTrashCount * 100f;
        public bool IsCollectionComplete =>
            TotalTrashCount > 0 && CollectedTrashCount >= TotalTrashCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void RegisterTrash(trashObjectiveTrigger trash)
        {
            if (trash == null)
            {
                return;
            }

            if (!trackedTrash.Add(trash))
            {
                return;
            }

            Debug.Log(
                $"[TrashCollection] Registered trash='{trash.name}'. Total tracked: {TotalTrashCount}"
            );
        }

        public bool TryCollectTrash(trashObjectiveTrigger trash, Collider trashCanCollider)
        {
            if (trash == null || trashCanCollider == null)
            {
                return false;
            }

            if (!collectedTrash.Add(trash))
            {
                return false;
            }

            Debug.Log(
                $"[TrashCollection] Collected trash='{trash.name}' into trashCan='{trashCanCollider.name}'. "
                    + $"Progress: {CollectedTrashCount} / {TotalTrashCount} ({CollectionPercentage:F2}%)"
            );

            return true;
        }

        // On milestone completion, checks if it has already been completed and passes it to listeners
        public bool CompleteMilestone(string objectiveID)
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                return false;
            }

            if (!completedMilestones.Add(objectiveID))
            {
                return false;
            }

            Debug.Log($"[TrashCollection] Milestone completed: '{objectiveID}'");

            OnMilestoneCompletion?.Invoke(objectiveID);
            return true;
        }

        // Helper function for listeners to check if a milestone has been completed yet
        public bool IsMilestoneCompleted(string objectiveID)
        {
            return completedMilestones.Contains(objectiveID);
        }
    }
}
