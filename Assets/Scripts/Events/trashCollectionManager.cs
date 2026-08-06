using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    public class trashCollectionManager : MonoBehaviour
    {
        // Sets up global instance for progression manager
        public static trashCollectionManager Instance { get; private set; }

        private class ObjectiveProgress
        {
            public int TrackedCount;
            public int CollectedCount;
            public HashSet<float> ReachedMilestones = new HashSet<float>();

            public float CollectionPercentage =>
                TrackedCount == 0 ? 0f : (float)CollectedCount / TrackedCount * 100f;
        }

        private static HashSet<string> completedObjectives = new HashSet<string>();
        private HashSet<trashObjectiveTrigger> trackedTrash = new HashSet<trashObjectiveTrigger>();
        private HashSet<trashObjectiveTrigger> collectedTrash =
            new HashSet<trashObjectiveTrigger>();

        private Dictionary<trashScriptableObject, ObjectiveProgress> objectiveProgress =
            new Dictionary<trashScriptableObject, ObjectiveProgress>();

        public static event Action<string> OnObjectiveCompleted;
        public static event Action<string, float> OnObjectiveMilestoneReached;
        public static event Action OnAllObjectivesCompleted;

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

        private void Start()
        {
            foreach (var objectiveEntry in objectiveProgress)
            {
                trashScriptableObject objective = objectiveEntry.Key;
                ObjectiveProgress progress = objectiveEntry.Value;
                string milestones = string.Join(", ", objective.MilestonePercentages);
                Debug.Log(
                    $"[TrashCollection] Objective '{objective.objectiveID}' milestones: [{milestones}] "
                        + $"total tracked: {progress.TrackedCount}"
                );
            }
        }

        public void RegisterTrash(trashObjectiveTrigger trash, trashScriptableObject objective)
        {
            if (trash == null)
            {
                return;
            }

            trackedTrash.Add(trash);

            if (objective != null)
            {
                if (!objectiveProgress.TryGetValue(objective, out var progress))
                {
                    progress = new ObjectiveProgress();
                    objectiveProgress[objective] = progress;
                }

                progress.TrackedCount++;
            }
        }

        public bool TryCollectTrash(
            trashObjectiveTrigger trash,
            Collider trashCanCollider,
            trashScriptableObject objective
        )
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

            if (objective != null && objectiveProgress.TryGetValue(objective, out var progress))
            {
                progress.CollectedCount++;
                CheckObjectiveMilestones(objective, progress);
            }

            if (IsCollectionComplete)
            {
                Debug.Log("[TrashCollection] All trash collected across all objectives.");
                OnAllObjectivesCompleted?.Invoke();
            }

            return true;
        }

        private void CheckObjectiveMilestones(
            trashScriptableObject objective,
            ObjectiveProgress progress
        )
        {
            float percent = progress.CollectionPercentage;
            var milestones = objective.MilestonePercentages;

            for (int i = 0; i < milestones.Count; i++)
            {
                float milestone = milestones[i];

                if (percent < milestone)
                {
                    continue;
                }

                if (!progress.ReachedMilestones.Add(milestone))
                {
                    continue;
                }

                string nextMilestone = i + 1 < milestones.Count ? $"{milestones[i + 1]}%" : "none";

                Debug.Log(
                    $"[TrashCollection] Objective '{objective.objectiveID}' reached "
                        + $"{milestone}% milestone ({progress.CollectedCount}/{progress.TrackedCount}). "
                        + $"Current: {percent:F2}%, next milestone: {nextMilestone}"
                );

                OnObjectiveMilestoneReached?.Invoke(objective.objectiveID, milestone);

                if (milestone >= 100f)
                {
                    CompleteObjective(objective.objectiveID);
                }
            }
        }

        public bool CompleteObjective(string objectiveID)
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                return false;
            }

            if (!completedObjectives.Add(objectiveID))
            {
                return false;
            }

            Debug.Log($"[TrashCollection] Objective completed: '{objectiveID}'");

            OnObjectiveCompleted?.Invoke(objectiveID);
            return true;
        }

        public static bool IsObjectiveCompleted(string objectiveID)
        {
            return completedObjectives.Contains(objectiveID);
        }

        public float GetObjectiveProgress(trashScriptableObject objective)
        {
            return objective != null && objectiveProgress.TryGetValue(objective, out var progress)
                ? progress.CollectionPercentage
                : 0f;
        }
    }
}
