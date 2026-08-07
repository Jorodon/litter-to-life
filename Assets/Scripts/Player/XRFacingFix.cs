using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class XRFacingFix : MonoBehaviour
{
    [SerializeField] private float targetYawAngle = 0f; // Set your target facing angle in degrees

    private void Start()
    {
        // Recenter tracking space via input subsystems
        var inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(inputSubsystems);
        
        foreach (var subsystem in inputSubsystems)
        {
            subsystem.TryRecenter();
        }

        // Force root rotation alignment
        transform.rotation = Quaternion.Euler(0f, targetYawAngle, 0f);
    }
}