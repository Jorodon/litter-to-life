using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// This script either forces the selection or deselection of an interactable object by the interactor this script is on.
/// </summary>
public class ManuallySelectObject : MonoBehaviour
{
    [Tooltip("What object are we selecting?")]
    public XRBaseInteractable interactable = null;

    private IXRSelectInteractor interactor = null;
    private XRInteractionManager interactionManager = null;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor inputInteractor = null;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor.InputTriggerType originalTriggerType;

    private void Awake()
    {
        inputInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>();
        interactor = inputInteractor as IXRSelectInteractor;
        interactionManager = inputInteractor.interactionManager;
        originalTriggerType = inputInteractor.selectActionTrigger;
    }

    public void ManuallySelect()
    {
        interactable.gameObject.SetActive(true);
        inputInteractor.selectActionTrigger = UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor.InputTriggerType.StateChange;

        interactionManager.SelectEnter(interactor, interactable); 
    }

    public void ManuallyDeselect()
    {
        interactionManager.SelectExit(interactor, interactable); 
        inputInteractor.selectActionTrigger = originalTriggerType;
        interactable.gameObject.SetActive(false);
    }
}
