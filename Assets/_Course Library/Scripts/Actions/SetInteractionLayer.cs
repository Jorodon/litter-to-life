using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


/// <summary>
/// Set the interaction layer of an interactor
/// </summary>
public class SetInteractionLayer : MonoBehaviour
{
    [Tooltip("The layer that's switched to")]
    public InteractionLayerMask targetLayer;
    private XRBaseInteractor interactor = null;
    private InteractionLayerMask originalLayer;

    private void Awake()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        originalLayer = interactor.interactionLayers;
    }

    public void SetTargetLayer()
    {
        interactor.interactionLayers = targetLayer;
    }

    public void SetOriginalLayer()
    {
        interactor.interactionLayers = originalLayer;
    }

    public void ToggleTargetLayer(bool value)
    {
        if (value)
        {
            SetTargetLayer();
        }
        else
        {
            SetOriginalLayer();
        }
    }

}
