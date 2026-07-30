using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private List<terrainDetailSO> terrainSOList;

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
        }

        // Listens for a milestone to be reached
        private void OnEnable()
        {
            trashCollectionManager.OnObjectiveCompleted += HandleMilestoneCompletion;
        }

        // Removes milestone function to preserve memory
        private void OnDisable()
        {
            trashCollectionManager.OnObjectiveCompleted -= HandleMilestoneCompletion;
        }

        // Checks that applicable objective was completed, then adds corresponding
        private void HandleMilestoneCompletion(string objectiveID)
        {
            if (terrainSOList == null)
                return;

            for (int i = 0; i < terrainSOList.Count; i++)
            {
                if (objectiveID == terrainSOList[i].objectiveID)
                {
                    switch (terrainSOList[i].objectName)
                    {
                        case "flowers":
                            SelectivelyRestoreAllCache(terrainSOList[i].relaventObjects);
                            break;
                        case "trees":
                            GenerateAllTrees();
                            break;
                        case "mushrooms":
                            //TO-DO : Generate mushrooms
                            break;
                        case "grass":
                            //TO-DO : Generate low grass
                            break;
                        case "high-grass":
                            //TO-DO : Generate high grass
                            break;
                    }
                }
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

            foreach (terrainDetailSO terrainDetail in terrainSOList)
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
            foreach (terrainHandler instance in terrainList)
            {
                //TO-DO : Restore flowers from cache
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
