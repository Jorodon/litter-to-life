using UnityEngine;
using FMODUnity;
using UnityEngine.XR.Interaction.Toolkit;

public class grabberAudioManager : MonoBehaviour
{

    // Serialized field to get grab SFX
    [Header("Grab Sound")]
    [SerializeField] private EventReference grabSound;

    // Serialized field to get release SFX
    [Header("Release Sound")]
    [SerializeField] private EventReference releaseSound;

    public void GrabObjectSFX(ActivateEventArgs args)
    {
        RuntimeManager.PlayOneShot(grabSound, transform.position);
    }

    public void ReleaseObjectSFX(DeactivateEventArgs args)
    {
        RuntimeManager.PlayOneShot(releaseSound, transform.position);
    }

}

