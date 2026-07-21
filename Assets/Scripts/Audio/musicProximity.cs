using System;
using UnityEngine;

public class musicProximity : MonoBehaviour
{
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float maxDistance;

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, playerTransform.position);
        float clampDistance = Mathf.Clamp(currentDistance, 0f, maxDistance);
        float normalizedDistance = 0f + Mathf.InverseLerp(0f, maxDistance, clampDistance);
        //Debug.Log($"Distance to player: {currentDistance} meters");
        backgroundMusicManager.Instance.SetGlobalMusicParameter("Distance", normalizedDistance);
    }
}
