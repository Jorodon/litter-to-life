using System.Collections.Generic;
using Game.Objective;
using UnityEngine;

namespace Game.Environment
{
    public class terrainObjectManager : MonoBehaviour
    {
        [Header("Parent Terrain Object")]
        private GameObject terrainParent;

        private Terrain targetTerrain;

        // List of terrain scriptable objects
        [SerializeField]
        private List<terrainDetailSO> detailSOList;

        // List of terrain scriptable objects
        [SerializeField]
        private List<terrainDetailSO> treeSOList;

        [SerializeField]
        public trashScriptableObject trashSO;

        [SerializeField]
        private List<trashScriptableObject> mainObjectives;

        public Collider colliderTest;

        private List<terrainHandler> terrainList = new List<terrainHandler>();

        public int treesToGenerate = 50;

        // Exclusion settings for trees (do not paint trees on these materials)
        [Header("Exclusion Settings")]
        public int excludedLayerIndex = 1;

        [Range(0f, 1f)]
        public float exclusionThreshold = 0.1f;

        // Caches all terrain immediately
        private void Awake()
        {
            terrainParent = gameObject;
            if (targetTerrain == null)
            {
                targetTerrain = Terrain.activeTerrain;
            }
            CacheAllTerrain();
        }

        // To be fully implemented...
        private void Start()
        {
            // FOR TESTING : Generates trees upon starting game
            //GenerateAllTrees();
            //GenerateAllFlowers();
            ClearTerrainDetailsOnStartup();
            ReplaceAllTrees();
        }

        // Listens for a milestone to be reached
        private void OnEnable()
        {
            trashCollectionManager.OnObjectiveCompleted += HandleMilestoneCompletion;
            trashCollectionManager.OnAllObjectivesCompleted += HandleAllObjectiveCompletion;
        }

        // Removes milestone function to preserve memory
        private void OnDisable()
        {
            trashCollectionManager.OnObjectiveCompleted -= HandleMilestoneCompletion;
            trashCollectionManager.OnAllObjectivesCompleted -= HandleAllObjectiveCompletion;
        }

        // Checks that applicable objective was completed, then adds corresponding
        private void HandleMilestoneCompletion(string objectiveID)
        {
            if (detailSOList == null)
                return;

            if (objectiveID == trashSO.objectiveID)
            {
                SelectivelyRestoreAllCacheInArea(colliderTest, detailSOList[1].relaventObjects);
            }

            int index = mainObjectives.FindIndex(i => i.objectiveID == objectiveID);

            if (mainObjectives != null && index != -1)
            {
                // Add list of mesh collider objects corresponding to zones here.
                SelectivelyRestoreAllCacheInArea(colliderTest, detailSOList[1].relaventObjects);
            }
        }

        private void HandleAllObjectiveCompletion()
        {
            if (detailSOList == null)
                return;

            //Big restoration at end of cleaning up trash?

        }

        public void SelectivelyRestoreAllCacheInArea(Collider other, List<GameObject> detailObjects)
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.SelectivelyRestoreCacheInArea(other, detailObjects);
            }
        }

        // Randomly generates trees on terrain while avoiding excluded layers
        public void GenerateAllTrees()
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.GenerateTrees(treesToGenerate, excludedLayerIndex, exclusionThreshold);
            }
        }

        // Replaces dead trees with live ones
        public void ReplaceAllTrees()
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.ReplaceTrees(treeSOList);
            }
        }

        public void RestoreAllTrees()
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.RestoreTrees(treeSOList);
            }
        }

        public List<Vector3> GetRandomTreeLocation(int numberOfBirds)
        {
            List<Vector3> birdSpawns = new List<Vector3>();

            for (int i = 0; i < numberOfBirds; i++)
            {
                terrainHandler instance = terrainList[Random.Range(0, 9)];

                Vector3 birdSpawn = instance.GetSingleRandomTreeLocation(excludedLayerIndex, exclusionThreshold);
                if (birdSpawn != Vector3.zero)
                {
                    birdSpawns.Add(birdSpawn);
                }
            }
            return birdSpawns;
        }

        // Clears terrain of details when given list of game objects
        public void ClearTerrainDetails(List<GameObject> detailObjects)
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.ClearSingleTerrainDetails(detailObjects);
            }
        }

        // Clears terrain of applicable details from SO list on startup
        public void ClearTerrainDetailsOnStartup()
        {
            List<GameObject> objectiveObjects = new List<GameObject>();

            foreach (terrainDetailSO terrainDetail in detailSOList)
            {
                foreach (GameObject prefab in terrainDetail.relaventObjects)
                {
                    objectiveObjects.Add(prefab);
                }
            }
            ClearTerrainDetails(objectiveObjects);
        }

        // Selectively restores certain detail objects from cache
        public void SelectivelyRestoreAllCache(List<GameObject> detailObjects)
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.SelectivelyRestoreCache(detailObjects);
            }
        }

        public void GenerateAllGrass()
        {
            foreach (terrainHandler instance in terrainList)
            {
                //TO-DO : Restore grass from cache
            }

        }

        public void GenerateAllFlowers()
        {
            for (int i = 0; i < detailSOList.Count; i++)
            {
                if (detailSOList[i].objectName == "flowers")
                {
                    SelectivelyRestoreAllCache(detailSOList[i].relaventObjects);
                }
            }
        }

        // Caches all terrain data into custom class
        public void CacheAllTerrain()
        {
            terrainList.Clear();
            foreach (Transform child in terrainParent.transform)
            {
                if (child.TryGetComponent<Terrain>(out Terrain targetTerrain))
                {
                    terrainList.Add(new terrainHandler(targetTerrain));
                }
                else
                {
                    foreach (Transform borderChild in child.transform)
                    {
                        targetTerrain = borderChild.GetComponent<Terrain>();
                        terrainList.Add(new terrainHandler(targetTerrain));
                    }
                }
            }
        }

        // Restores ALL terrain data stored in custom cache class
        public void RestoreAllCachedTerrain()
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.RestoreCachedTerrain();
            }
        }

        // Restores terrain data from cache on object destruction
        void OnDestroy()
        {
            Debug.Log("Restoring cached terrain...");
            RestoreAllCachedTerrain();
        }
    }

    // [System.Serializable]
    // public class ExclusionZones
    // {
    //     public string zoneName;
    //     public Collider collider;
    // }
}
