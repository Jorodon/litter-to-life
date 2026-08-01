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
        private terrainDetailSO testTerrainObj;

        [SerializeField]
        private terrainDetailSO testTerrainTrees;

        [SerializeField]
        private trashCollectionManager collectionManager;

        private bool trashCollected;

        private void OnEnable()
        {
            collectionManager?.RegisterTrash(this, objective);
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

            if (collectionManager.TryCollectTrash(this, other, objective))
            {
                trashCollected = true;

                if (collectionManager.IsCollectionComplete)
                {
                    //testTerrainObj?.CompleteObjective();
                    //testTerrainTrees?.CompleteObjective();
                }

                Destroy(gameObject);
            }
        }
    }
}
