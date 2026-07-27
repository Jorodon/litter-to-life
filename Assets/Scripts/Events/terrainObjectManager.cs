using System.Collections.Generic;
using UnityEngine;
using Game.Objective;

namespace Game.Environment
{
    public class terrainObjectManager : MonoBehaviour
    {
        [Header("Parent Terrain Object")]
        public GameObject terrainParent;

        private Terrain targetTerrain;

        private List<terrainHandler> terrainList = new List<terrainHandler>();

        [SerializeField] private trashScriptableObject mainObjective;

        public int treesToGenerate = 50;

        [SerializeField]
        public List<GameObject> grassObjects = new List<GameObject>();
        //public int grassToGenerate = 50;

        [SerializeField]
        public List<GameObject> flowerObjects = new List<GameObject>();
        //public int flowersToGenerate = 50;
        
        // Exclusion settings for trees (do not paint trees on these materials)
        [Header("Exclusion Settings")]
        public int excludedLayerIndex = 1;
        [Range(0f, 1f)]
        public float exclusionThreshold = 0.1f;

        // [SerializeField]
        // private List<ExclusionZones> exclusionZones = new List<ExclusionZones>();


        // Caches all terrain immediately
        private void Awake()
        {
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
            GenerateAllTrees();
            //GenerateAllFlowers();
            RemoveAllTerrainDetails(flowerObjects);
        }

        // Listens for a milestone to be reached
        private void OnEnable()
        {
            trashCollectionManager.OnMilestoneCompletion += HandleMilestoneCompletion;
        }

        // Removes milestone function to preserve memory
        private void OnDisable()
        {
            trashCollectionManager.OnMilestoneCompletion -= HandleMilestoneCompletion;
        }

        // Checks that applicable objective was completed, then increases music state
        private void HandleMilestoneCompletion(string objectiveID)
        {
            if (mainObjective != null && objectiveID == mainObjective.objectiveID)
            {
                SelectivelyRestoreAllCache(flowerObjects);
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

        public void RemoveAllTerrainDetails(List<GameObject> detailObjects)
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.RemoveTerrainDetails(detailObjects);
            }
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

        // Caches alll terrain data into custom class
        public void CacheAllTerrain()
        {
            terrainList.Clear();
            foreach (Transform child in terrainParent.transform)
            {
                targetTerrain = child.GetComponent<Terrain>();
                terrainList.Add(new terrainHandler(targetTerrain));
                
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
