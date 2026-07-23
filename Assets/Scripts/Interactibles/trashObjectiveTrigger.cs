using UnityEngine;

namespace Game.Objective
{
    public class trashObjectiveTrigger : MonoBehaviour
    {
        // Assign an objective to the object
        [SerializeField] private trashScriptableObject objective;

        // Upon entering a collider trigger, calls objective completion function for assigned objective
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && objective != null)
            {
                objective.CompleteObjective();
            }
        }
    }
}
