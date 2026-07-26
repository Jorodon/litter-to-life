using UnityEngine;
using Game.Objective;

namespace Game.Environment
{
    public class terrainObjectListener : MonoBehaviour
    {

        // Creates singleton access for background music across all scripts (might not need anymore)
        public static terrainObjectListener Instance { get; private set; }

        [SerializeField] private trashScriptableObject mainObjective;

        [SerializeField] public terrainObjectManager terrainManagerInstance;
        public Texture2D grassTexture;


        // Runs before Start() to prevent duplicates, set a single Instance, and ensure it survives across scenes
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Starts the selected music event
        private void Start()
        {
            //targetTerrain.detailObjectDistance = 0f;
            //  TerrainData terrainData = targetTerrain.terrainData;

            // // 1. Setup Detail Prototype
            // DetailPrototype detailProto = new DetailPrototype();
            // detailProto.prototypeTexture = detailTexture;
            // detailProto.renderMode = DetailRenderMode.Grass;

            // var details = new System.Collections.Generic.List<DetailPrototype>(terrainData.detailPrototypes);
            // details.Add(detailProto);
            // terrainData.detailPrototypes = details.ToArray();
            // targetTerrain.Flush();

            // // 2. Define detail map array (Width x Height resolution of the detail layer)
            // int layerIndex = details.Count - 1;
            // int res = terrainData.detailResolution;
            // int[,] detailArray = new int[res, res];

            // // Fill a small patch with density values (0 to 16)
            // for (int x = 10; x < 30; x++)
            // {
            //     for (int z = 10; z < 30; z++)
            //     {
            //         detailArray[x, z] = 8; // Density amount
            //     }
            // }

            // // Apply to the terrain detail layer
            // terrainData.SetDetailLayer(0, 0, layerIndex, detailArray);
            // //backgroundMusicWrapper.PlayAudio();

            // TerrainData terrainData = targetTerrain.terrainData;

            // // 1. Create a temporary DetailPrototype object
            // DetailPrototype grassPrototype = new DetailPrototype();
            
            // // 2. Correctly assign the texture property
            // grassPrototype.prototypeTexture = grassTexture; 
            // grassPrototype.renderMode = DetailRenderMode.GrassBillboard;

            // // 3. To apply it, you MUST overwrite the entire array container
            // DetailPrototype[] newPrototypes = new DetailPrototype[1];
            // newPrototypes[0] = grassPrototype;

            // // 4. Assign the container back to the terrain data
            // terrainData.detailPrototypes = newPrototypes;

            // // 5. Force update the engine graphics chunks
            // targetTerrain.Flush();
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
                terrainManagerInstance.GenerateAllTrees();
            }
        }

        private void GetTerrainData(Terrain t, float threshold)
        {
            //var map = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, 0);

            //     // For each pixel in the detail map...
            // for (var y = 0; y < t.terrainData.detailHeight; y++)
            // {
            //     for (var x = 0; x < t.terrainData.detailWidth; x++)
            //     {
            //         // If the pixel value is below the threshold then
            //         // set it to zero.
            //         if (map[x, y] < threshold)
            //         {
            //             map[x, y] = 0;
            //         }
            //     }
            // }

            // Assign the modified map back.
            //t.terrainData.SetDetailLayer(0, 0, 1, map);
        }
    }
}
