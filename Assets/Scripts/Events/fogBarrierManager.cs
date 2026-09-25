using UnityEngine;
using FMODUnity;
using Game.Objective;

namespace Game.Environment
{
    public class fogBarrierManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject fogObject;

        Transform childTransform;

        private Animator fogAnimator;

        private Collider fogCollider;

        [SerializeField]
        //private List<trashScriptableObject> mainObjectives;
        private trashScriptableObject previousZoneObjective; 

        [SerializeField]
        //private List<trashScriptableObject> mainObjectives;
        private trashScriptableObject currentZoneObjective; 

        // Serialized field to get grab SFX
        [SerializeField] 
        private EventReference barrierSound;

        private int fogState = 5;

        // // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            fogAnimator = fogObject.GetComponent<Animator>();
            fogAnimator.SetInteger("FogState", 5);

            if (transform.childCount > 0)
            {
                childTransform = transform.GetChild(0);
            }

            fogCollider = fogObject.GetComponent<Collider>();
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
            if (previousZoneObjective != null && previousZoneObjective.objectiveID == objectiveID)
            {
                // Clear fog for area
                fogState = 4;
                PlayBarrierSound();
                SetFogState(fogState, false);
                if (fogCollider != null)
                {
                    fogCollider.enabled = false;
                }
            }

            if (currentZoneObjective != null && currentZoneObjective.objectiveID == objectiveID)
            {
                // Clear fog for area
                gameObject.SetActive(false);
            }
        }

        private void HandleMilestoneCompletion(string objectiveID, float milestone)
        {
            if (currentZoneObjective != null && currentZoneObjective.objectiveID == objectiveID)
            {
                // Lighten fog for area
                fogState--;
            }
        }


         // Checks that applicable objective was completed, then increases music state
        private void HandleAllCompleted()
        {
            // Clear all Fog
            SetFogState(0, true);

            if (fogCollider != null)
            {
                fogCollider.enabled = false;
            }
        }

        public void PlayBarrierSound()
        {
            if (!barrierSound.IsNull)
            {
                Debug.Log("Played barrier sound.");
                RuntimeManager.PlayOneShot(barrierSound, childTransform.position);
            }
        }

        public void SetFogState(int fogStateInput, bool fastTransition)
        {
            fogState = fogStateInput;
            fogAnimator.SetInteger("FogState", fogStateInput);
            if (fastTransition)
            {
                fogAnimator.SetTrigger("quickTransition");
            }

            Debug.Log($"Barrier Fog state set to {fogStateInput}");
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && trashCollectionManager.IsObjectiveCompleted(previousZoneObjective.objectiveID)) {
                //fogManager.Instance.SetFogState(4, true);
                if (!trashCollectionManager.IsObjectiveCompleted(currentZoneObjective.objectiveID))
                {
                    fogManager.Instance.EnterFogArea();
                }
                fogAnimator.SetInteger("FogState", 0);
                fogAnimator.SetTrigger("quickTransition");
                Debug.Log($"Barrier Fog state temporarily set to 0");
                //SetFogState(0, true);
                //Destroy(gameObject);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && trashCollectionManager.IsObjectiveCompleted(previousZoneObjective.objectiveID)) {
                fogManager.Instance.ExitFogArea();
                SetFogState(fogState, true);
            }
        }
    }

}
