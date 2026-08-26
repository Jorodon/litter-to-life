using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToggleInteractorComponents : MonoBehaviour
{
    [Tooltip("Assign the NearFarInteractor component from another GameObject.")]
    public NearFarInteractor nearFarInteractor;

    [Tooltip("The GameObject that holds the Teleport Interactor.")]
    public GameObject teleportInteractor;

    void Awake()
    {
        DisableTeleportRay();
    }

    /// <summary>
    /// Enables the Teleport Interactor GameObject and disables far casting on the NearFarInteractor.
    /// </summary>
    public void EnableTeleportRay()
    {
        if (teleportInteractor != null)
            teleportInteractor.SetActive(true); // Fix here

        if (nearFarInteractor != null)
            nearFarInteractor.enableFarCasting = false;
    }

    /// <summary>
    /// Disables the Teleport Interactor GameObject and enables far casting on the NearFarInteractor.
    /// </summary>
    public void DisableTeleportRay()
    {
        if (teleportInteractor != null)
            teleportInteractor.SetActive(false); // Fix here

        if (nearFarInteractor != null)
            nearFarInteractor.enableFarCasting = true;
    }
}