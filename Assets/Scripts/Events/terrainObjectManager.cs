using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class terrainObjectManager : MonoBehaviour
    {
        [Header("Parent Terrain Object")]
        public GameObject terrainParent;

        private Terrain targetTerrain;

        private List<terrainHandler> terrainList = new List<terrainHandler>();


        public int treesToGenerate = 50;
        
        // Exclusion settings for trees (do not paint trees on these materials)
        [Header("Exclusion Settings")]
        public int excludedLayerIndex = 1;
        [Range(0f, 1f)]
        public float exclusionThreshold = 0.1f;

        [SerializeField]
        private List<ExclusionZones> exclusionZones = new List<ExclusionZones>();


        private void Awake()
        {
            if (targetTerrain == null) 
            {
                targetTerrain = Terrain.activeTerrain;
            }

            CacheAllTerrain();

        }

        private void Start()
        {
            // FOR TESTING : Generates trees upon starting game
            GenerateAllTrees();
        }

        public void GenerateAllTrees()
        {
            int count = 0;
            foreach (terrainHandler instance in terrainList)
            {
                instance.GenerateTrees(treesToGenerate, excludedLayerIndex, exclusionThreshold);
                Debug.Log("Generating trees for terrin object " + count);
                count++;
            }
        }

        public void CacheAllTerrain()
        {
            foreach (Transform child in terrainParent.transform)
            {
                targetTerrain = child.GetComponent<Terrain>();
                terrainList.Add(new terrainHandler(targetTerrain));
            }
        }

        public void RestoreAllCachedTerrain()
        {
            foreach (terrainHandler instance in terrainList)
            {
                instance.RestoreCachedTerrain();
            }
        }


        void OnDestroy()
        {
            RestoreAllCachedTerrain();
        }
    }

    [System.Serializable]
    public class ExclusionZones
    {
        public string zoneName;
        public Collider collider;
    }
}
