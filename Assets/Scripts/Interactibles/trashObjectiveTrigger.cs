using UnityEngine;

namespace Game.Objective
{
    public class trashObjectiveTrigger : MonoBehaviour
    {
        [Header("Trash Collection")]
        [SerializeField]
        private string trashCanTag = "TrashCan";

        [SerializeField]
        private trashScriptableObject objective;

        [SerializeField]
        private trashCollectionManager collectionManager;

        private bool trashCollected;

        private void OnEnable()
        {
            collectionManager?.RegisterTrash(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryCollect(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryCollect(other);
        }

        private void TryCollect(Collider other)
        {
            if (trashCollected || other == null || !other.CompareTag(trashCanTag))
            {
                return;
            }

            if (collectionManager.TryCollectTrash(this, other))
            {
                trashCollected = true;

                if (collectionManager.IsCollectionComplete)
                {
                    objective?.CompleteObjective();
                }

                Destroy(gameObject);
            }
        }
    }
}
