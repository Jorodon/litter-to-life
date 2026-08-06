using UnityEngine;
using System.Collections.Generic;
using Game.Objective;

namespace Game.Environment
{
    public class fogManager : MonoBehaviour
    {

        public static fogManager Instance { get; private set; }

        [SerializeField]
        private GameObject playerFogObject;

        private Animator playerFogAnimator;

        [SerializeField]
        private GameObject cabinFogObject;

        private Animator cabinFogAnimator;

        [SerializeField]
        private List<trashScriptableObject> mainObjectives;

        private int fogState = 0;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            playerFogAnimator = playerFogObject.GetComponent<Animator>();
            cabinFogAnimator = cabinFogObject.GetComponent<Animator>();
            playerFogAnimator.SetInteger("FogState", fogState);
            cabinFogAnimator.SetInteger("FogState", 4);
        }

        // Listens for a milestone to be reached
        private void OnEnable()
        {
            trashCollectionManager.OnObjectiveCompleted += HandleObjectiveCompletion;
            trashCollectionManager.OnObjectiveMilestoneReached += HandleMilestoneCompletion;
            trashCollectionManager.OnAllObjectivesCompleted += HandleAllCompleted;
        }

        // Removes milestone function to preserve memory
        private void OnDisable()
        {
            trashCollectionManager.OnObjectiveCompleted -= HandleObjectiveCompletion;
            trashCollectionManager.OnObjectiveMilestoneReached -= HandleMilestoneCompletion;
            trashCollectionManager.OnAllObjectivesCompleted -= HandleAllCompleted;
        }

        // Checks that applicable objective was completed, then increases music state
        private void HandleObjectiveCompletion(string objectiveID)
        {
            int index = mainObjectives.FindIndex(i => i.objectiveID == objectiveID);

            if (mainObjectives != null && index != -1)
            {
                // Clear fog for area
                fogState = 0;
                SetFogState(fogState, false);
            }
        }

         // Checks that applicable objective was completed, then increases music state
        private void HandleMilestoneCompletion(string objectiveID, float milestone)
        {
            int index = mainObjectives.FindIndex(i => i.objectiveID == objectiveID);

            if (mainObjectives != null && index != -1)
            {
                // Lighten fog for area
                fogState--;
                SetFogState(fogState, false);
                //Debug.Log($"Fog state set to {fogState}");
            }
        }

         // Checks that applicable objective was completed, then increases music state
        private void HandleAllCompleted()
        {
            // Clear all Fog
        }

        public void DestroyCabinFog()
        {
            cabinFogAnimator.SetInteger("FogState", 0);
            Destroy(cabinFogObject);
        }


        private void IncreaseFogState()
        {

            if (fogState == 0)
            {
                fogState = 5;
                playerFogAnimator.SetInteger("FogState", fogState);
            }
            else
            {
                fogState--;
                playerFogAnimator.SetInteger("FogState", fogState);
            }

            Debug.Log($"Fog state set to {fogState}");

        }

        public void SetFogState(int fogStateInput, bool fastTransition)
        {
            fogState = fogStateInput;
            playerFogAnimator.SetInteger("FogState", fogStateInput);
            if (fastTransition)
            {
                playerFogAnimator.SetTrigger("quickTransition");
            }

            Debug.Log($"Player Fog state set to {fogStateInput}");
        }

        public void EnterFogArea()
        {
            if (fogState == 0)
            {
                fogState = 4;
                SetFogState(fogState, true);
            }
            else
            {
                SetFogState(fogState, true);
            }
        }

        public void ExitFogArea()
        {
            //SetFogState(0, true);
            playerFogAnimator.SetInteger("FogState", 0);
            playerFogAnimator.SetTrigger("quickTransition");
            Debug.Log($"Player Fog state temporarily set to 0");
        }

        public int GetFogState()
        {
            return fogState;
        }
    }

}
