using UnityEngine;
using System.Collections.Generic;
using Game.Objective;

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

        // Test that collider overlaps with terrain and returns grid bounds for terrain detail layer
        public bool GetDetailGridBounds(Collider other, out int startX, out int startZ, out int width, out int height)
        {
            startX = startZ = width = height = 0;

            Bounds colliderBounds = other.bounds;

            // Finds local terrain min/max of collider
            Vector3 localMin = colliderBounds.min - terrain.transform.position;
            Vector3 localMax = colliderBounds.max - terrain.transform.position;

            int detailRes = terrainData.detailResolution;

            // Find the norm min/max for X/Z axis
            float normMinX = localMin.x / terrainData.size.x;
            float normMinZ = localMin.z / terrainData.size.z;
            float normMaxX = localMax.x / terrainData.size.x;
            float normMaxZ = localMax.z / terrainData.size.z;

            // Map norm coords to grid and clamp to terrain detail res
            int minX = Mathf.Clamp(Mathf.FloorToInt(normMinX * detailRes), 0, detailRes - 1);
            int minZ = Mathf.Clamp(Mathf.FloorToInt(normMinZ * detailRes), 0, detailRes - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt(normMaxX * detailRes), 0, detailRes);
            int maxZ = Mathf.Clamp(Mathf.CeilToInt(normMaxZ * detailRes), 0, detailRes);

            // Assign start values and find weidth/height
            startX = minX;
            startZ = minZ;
            width = maxX - minX;
            height = maxZ - minZ;

            // Returns true if bounds overlap
            return width > 0 && height > 0;

        }

        // Restore cached details in area based on mesh collider
        public void SelectivelyRestoreCacheInArea(Collider other, List<GameObject> targetObjects)
        {
            // Check if terrain and mesh collider overlap
            if (!GetDetailGridBounds(other, out int startX, out int startZ, out int width, out int height))
            {
                return;
            }

            // Slice 2D detail layer and restore smaller sections for higher resolution
            foreach (GameObject targetObject in targetObjects)
            {
                if (targetObject == null) continue;

                int protoIndex = FindPrototypeIndex(targetObject);
                if (protoIndex == -1) continue;

                // Create a new slice of detail and record current detail layer status
                int[,] detailSlice = new int[height, width];
                int[,] fullCache = terrainCache.cachedDetails[protoIndex];
                int[,] currentDetails = terrainData.GetDetailLayer(startX, startZ, width, height, protoIndex);

                // Test each point to see if it falls inside bounds
                for (int z = 0; z < height; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Normalize terrain coords
                        float normX = (float)(startX + x) / terrainData.detailResolution;
                        float normZ = (float)(startZ + z) / terrainData.detailResolution;

                        // Find local coords of terrain
                        float localX = normX * terrainData.size.x;
                        float localZ = normZ * terrainData.size.z;
                        float localY = terrainData.GetInterpolatedHeight(normX, normZ); // Samples heightmap

                        // Find position vector of point in world coords
                        Vector3 pointWorldPos = terrain.transform.position + new Vector3(localX, localY, localZ);

                        // If point aligns with collider bounds, apply cached details
                        if (other.ClosestPoint(pointWorldPos) == pointWorldPos)
                        {
                            detailSlice[z, x] = fullCache[startZ + z, startX + x];
                        }
                        else
                        {
                            detailSlice[z, x] = currentDetails[z, x];
                        }
                    }
                }

                // Apply slice back to target area
                terrainData.SetDetailLayer(startX, startZ, protoIndex, detailSlice);
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

        public void ReplaceTrees(List<terrainDetailSO> treeSOList) 
        {
            Dictionary<int, int> prototypeIndexes = new Dictionary<int, int>();
            List<GameObject> protoPrefabs = new List<GameObject>();

            foreach (TreePrototype treeProto in terrainData.treePrototypes)
            {
                protoPrefabs.Add(treeProto.prefab);
            }

            foreach(terrainDetailSO treeList in treeSOList)
            {
                if (treeList.relaventObjects == null)
                {
                    Debug.Log($"No prefabs added to detail scriptable object {treeList.objectName}");
                    return;
                }

                int key = protoPrefabs.IndexOf(treeList.relaventObjects[0]);
                int value = protoPrefabs.IndexOf(treeList.relaventObjects[1]);

                if (key == -1 || value == -1) 
                {
                    Debug.Log($"Prototype not added to terrain {terrain.name}. Check that both {treeList.relaventObjects[0].name} and {treeList.relaventObjects[1].name} have been added to the terrain.");
                    continue;
                }

                prototypeIndexes.Add(key, value);
            }

            List<TreeInstance> currentInstances = new List<TreeInstance>(terrainData.treeInstances);

            for (int i = 0; i < currentInstances.Count; i++)
            {
                TreeInstance tree = currentInstances[i];
                if (prototypeIndexes.TryGetValue(tree.prototypeIndex, out int newIndex))
                {
                    tree.prototypeIndex = newIndex;
                    currentInstances[i] = tree;
                }
            }

            terrainData.treeInstances = currentInstances.ToArray();
            terrain.Flush();
        }

        public void RestoreTrees(List<terrainDetailSO> treeSOList) 
        {
            // Dictionary<int, int> prototypeIndexes = new Dictionary<int, int>();
            List<GameObject> prototypePrefabs = new List<GameObject>();

            foreach (TreePrototype treeProto in terrainData.treePrototypes)
            {
                prototypePrefabs.Add(treeProto.prefab);
            }

            List<GameObject> treePrefabs = new List<GameObject>();
            List<int> prototypeIndexes = new List<int>();

            foreach(terrainDetailSO treeList in treeSOList)
            {
                if (treePrefabs.Contains(treeList.relaventObjects[1])) continue;

                treePrefabs.Add(treeList.relaventObjects[1]);
                prototypeIndexes.Add(prototypePrefabs.IndexOf(treeList.relaventObjects[1]));
                //int value = protoPrefabs.IndexOf(treeList.relaventObjects[0]);

                //if (key == -1 || value == -1) continue;

                //prototypeIndexes.Add(key, value);
            }

            List<TreeInstance> currentInstances = new List<TreeInstance>(terrainData.treeInstances);

            for (int i = 0; i < currentInstances.Count; i++)
            {
                TreeInstance tree = currentInstances[i];
                int checkIndex = prototypeIndexes.IndexOf(tree.prototypeIndex);
                if (checkIndex != -1)
                {
                    tree = terrainCache.cachedTrees[i];
                    currentInstances[i] = tree;
                }
            }

            terrainData.treeInstances = currentInstances.ToArray();
            terrain.Flush();
        }

        public Vector3 GetSingleRandomTreeLocation(int excludedLayerIndex, float exclusionThreshold)
        {

            // Store alphamap dimensions
            int mapWidth = terrainData.alphamapWidth;
            int mapHeight = terrainData.alphamapHeight;

            // Store alphamap for terrain
            float[,,] alphamapData = terrainData.GetAlphamaps(0, 0, mapWidth, mapHeight);
            
            // Pull tree instances from terrain
            List<TreeInstance> currentInstances = new List<TreeInstance>(terrainData.treeInstances);

            TreeInstance birdTree = currentInstances[Random.Range(0, currentInstances.Count)];

            // Get prefab from tree instance and find height using renderer component bounds on first LOD
            float heightScale = birdTree.heightScale;
            GameObject treePrefab = terrainData.treePrototypes[birdTree.prototypeIndex].prefab;
            LODGroup lodGroup = treePrefab.GetComponent<LODGroup>();
            LOD[] lodArray = lodGroup.GetLODs();
            Renderer[] lodRenderers = lodArray[0].renderers;
            float treeHeight = heightScale * lodRenderers[0].bounds.size.y;

            // Find the local coordinates for tree top, convert to global coordinates and return them
            float localX = birdTree.position.x * terrainData.size.x;
            float localZ = birdTree.position.z * terrainData.size.z;
            float localY = terrainData.GetInterpolatedHeight(birdTree.position.x, birdTree.position.z);

            return terrain.transform.position + new Vector3(localX, localY + treeHeight, localZ);
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