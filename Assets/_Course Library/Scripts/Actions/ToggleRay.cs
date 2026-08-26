using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



/// <summary>
/// Toggle between the direct and ray interactor if the direct interactor isn't touching any objects
/// Should be placed on a ray interactor
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor))]
public class ToggleRay : MonoBehaviour
{
    [Tooltip("Switch even if an object is selected")]
    public bool forceToggle = false;

    [Tooltip("The direct interactor that's switched to")]

    public GameObject nearFarInteractor;
    //public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor directInteractor = null;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor = null;
    private bool isSwitched = false;

    private void Awake()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        SwitchInteractors(false); // moved to here
    }

    private void Start()
    {
        //SwitchInteractors(false);
    }

    public void ActivateRay()
    {
        if (!TouchingObject() || forceToggle)
            SwitchInteractors(true);
    }

    public void DeactivateRay()
    {
        if (isSwitched)
            SwitchInteractors(false);
    }

    private bool TouchingObject()
    {
        List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable> targets = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>();
        //directInteractor.GetValidTargets(targets);
        return (targets.Count > 0);
    }


    private void SwitchInteractors(bool value)
    {
        isSwitched = value;
        rayInteractor.enabled = value;
        nearFarInteractor.SetActive(!value);
    }
}
