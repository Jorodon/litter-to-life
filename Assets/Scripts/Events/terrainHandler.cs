using UnityEngine;
using System.Collections.Generic;

namespace Game.Environment
{
    public class terrainHandler
    {

        private TerrainCache terrainCache;
        private TerrainData terrainData;
        private Terrain terrain;

        public List<Vector3> debugFlowerPositions = new List<Vector3>();

        // Constructor : Sets terrain object and terrainData for targetTerrain, and caches current terrain data
        public terrainHandler(Terrain targetTerrain)
        {
            terrain = targetTerrain;
            terrainData = terrain.terrainData;

            CacheTerrain();
        }

        // Caches current terrain data        
        public void CacheTerrain()
        {
            terrainCache = new TerrainCache();

            terrainCache.cachedHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
            terrainCache.cachedAlphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

            terrainCache.cachedTrees = (TreeInstance[]) terrainData.treeInstances.Clone();
            
            int layerCount = terrainData.detailPrototypes.Length;
            terrainCache.cachedDetails = new int[layerCount][,];

            for (int i = 0; i < layerCount; i++)
            {
                terrainCache.cachedDetails[i] = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i);
            }
        }

        // Restores cached terrain data
        public void RestoreCachedTerrain()
        {
            if (terrainCache == null)
            {
                return;
            }

            terrainData.SetHeights(0, 0, terrainCache.cachedHeights);
            terrainData.SyncHeightmap();

            terrainData.SetAlphamaps(0, 0, terrainCache.cachedAlphamaps);

            terrainData.treeInstances = terrainCache.cachedTrees;

            for (int i = 0; i < terrainCache.cachedDetails.Length; i++)
            {
                terrainData.SetDetailLayer(0, 0, i, terrainCache.cachedDetails[i]);
            }
        }

        public void GenerateTrees(int treesToGenerate, int excludedLayerIndex, float exclusionThreshold)
        {
            if (terrainData.treePrototypes.Length == 0)
            {
                Debug.Log("No tree prototypes added to terrain");
                return;
            }

            // Store alphamap dimensions
            int mapWidth = terrainData.alphamapWidth;
            int mapHeight = terrainData.alphamapHeight;

            // Store alphamap for terrain
            float[,,] alphamapData = terrainData.GetAlphamaps(0, 0, mapWidth, mapHeight);

            // Initialize an empty list of tree instances for terrain
            List<TreeInstance> currentInstances = new List<TreeInstance>(terrainData.treeInstances);
            int addedTreeCount = 0;
            int attempts = 0;

            // Main loop : attempt to add up to (treesToGenerate) trees to terrain, but stop after too many attempts
            while (addedTreeCount < treesToGenerate && attempts < treesToGenerate * 5)
            {
                attempts++;

                // Generate random normalized terrain coordinates (0.01 to 0.99)
                float normX = Random.Range(0.01f, 0.99f);
                float normZ = Random.Range(0.01f, 0.99f);

                // Convert normalized coords to alphamap coords
                int mapX = Mathf.FloorToInt(normX * mapWidth);
                int mapZ = Mathf.FloorToInt(normZ * mapHeight);

                // Clamp alphamap coords to prevent out-of-bounds errors
                mapX = Mathf.Clamp(mapX, 0, mapWidth - 1);
                mapZ = Mathf.Clamp(mapZ, 0, mapHeight - 1);

                // Layer exclusion logic : Read texture weight for excluded layer at calculated point
                float textureWeight = alphamapData[mapZ, mapX, excludedLayerIndex];

                // Retry if excluded layer texture weight above threshold
                if (textureWeight > exclusionThreshold)
                {
                    continue;
                }

                // Create new tree instance and add it to List
                TreeInstance newTree = new TreeInstance();
                newTree.position = new Vector3(normX, terrainData.GetInterpolatedHeight(normX, normZ) / terrainData.size.y, normZ);
                newTree.prototypeIndex = Random.Range(0, terrainData.treePrototypes.Length);
                newTree.widthScale = Random.Range(0.8f, 1.2f);
                newTree.heightScale = Random.Range(0.8f, 1.2f);
                currentInstances.Add(newTree);

                addedTreeCount++;
            }

            // Apply new list of trees to terrain 
            terrainData.SetTreeInstances(currentInstances.ToArray(), true);
        }

        // public void GenerateGrass()
        // {
            
        // }

        // Helper function : finds index of detail prototype that matches prefab
        public int FindPrototypeIndex(GameObject prefab)
        {
            if (prefab == null || terrainData == null) return -1;

            DetailPrototype[] existingPrototypes = terrainData.detailPrototypes;

            for (int i = 0; i < existingPrototypes.Length; i++)
            {
                DetailPrototype proto = existingPrototypes[i];
                if (proto.usePrototypeMesh && proto.prototype == prefab)
                {
                    return i;
                }
            }

            return -1;
        }

        // Selectively restores detail mesh from cache
        public void SelectivelyRestoreCache(List<GameObject> targetObjects)
        {
            foreach (GameObject targetObject in targetObjects)
            {
                if (targetObject == null) continue;

                Renderer r = targetObject.GetComponentInChildren<Renderer>();
                if (r != null && r.sharedMaterial != null)
                {
                    r.sharedMaterial.enableInstancing = true;
                }

                int protoIndex = FindPrototypeIndex(targetObject);

                if (protoIndex == -1)
                {
                    Debug.LogWarning($"[TerrainHandler] Prefab '{targetObject.name}' is NOT registered on the Terrain asset!\n" +
                                     $"Add it to the Terrain's 'Paint Details' tab in the Inspector before running again.");
                    continue;
                }

                terrainData.SetDetailLayer(0, 0, protoIndex, terrainCache.cachedDetails[protoIndex]);
            }
        }

        // Removes all instances of certain detail meshes from the terrain object
        public void ClearSingleTerrainDetails(List<GameObject> targetObjects)
        {
            int detailRes = terrainData.detailResolution;
            int[,] emptyArray = new int[detailRes, detailRes]; // Fresh array full of 0s

            foreach (GameObject targetObject in targetObjects)
            {
                if (targetObject == null) continue;

                Renderer r = targetObject.GetComponentInChildren<Renderer>();
                if (r != null && r.sharedMaterial != null)
                {
                    r.sharedMaterial.enableInstancing = true;
                }

                int protoIndex = FindPrototypeIndex(targetObject);

                if (protoIndex == -1)
                {
                    Debug.LogWarning($"[TerrainHandler] Prefab '{targetObject.name}' is NOT registered on the Terrain asset!\n" +
                                     $"Add it to the Terrain's 'Paint Details' tab in the Inspector before running again.");
                    continue;
                }

                terrainData.SetDetailLayer(0, 0, protoIndex, emptyArray);
            }
        }
    }

    // Stores a cache of starting terrain data
    public class TerrainCache
    {
        public float[,] cachedHeights;
        public float[,,] cachedAlphamaps;
        public TreeInstance[] cachedTrees;
        public int[][,] cachedDetails;
    }
}