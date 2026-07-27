using UnityEngine;
using Game.Objective;
using System.Collections.Generic;

namespace Game.Environment
{
    public class terrainObjectListener : MonoBehaviour
    {

        // Creates singleton access for background music across all scripts (might not need anymore)
        public static terrainObjectListener Instance { get; private set; }

        [SerializeField]
        private List<terrainDetailSO> terrainSOList;

        [SerializeField] public terrainObjectManager terrainManagerInstance;



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
            if (terrainSOList == null) return;

            for (int i = 0; i < terrainSOList.Count; i++)
            {
                if (terrainSOList[i].objectName == "flowers")
                {
                    terrainManagerInstance.SelectivelyRestoreAllCache(terrainSOList[i].relaventObjects);
                }
            }
        }
    }
}
