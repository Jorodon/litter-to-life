using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Objective
{
    public abstract class objectiveManagerBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected HashSet<string> completedObjectives = new HashSet<string>();

        public static event Action<string> OnObjectiveCompleted;

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this as T)
            {
                Instance = null;
            }
        }

        protected void Log(string message)
        {
            Debug.Log($"[{typeof(T).Name}] {message}");
        }

        public virtual bool CompleteObjective(string objectiveID)
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                return false;
            }

            if (!completedObjectives.Add(objectiveID))
            {
                return false;
            }

            Log($"Objective completed: '{objectiveID}'");

            OnObjectiveCompleted?.Invoke(objectiveID);
            return true;
        }

        public bool IsObjectiveCompleted(string objectiveID)
        {
            return completedObjectives.Contains(objectiveID);
        }
    }
}
