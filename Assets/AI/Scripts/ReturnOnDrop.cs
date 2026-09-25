using UnityEngine;

public class ReturnOnDrop : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody brushBody;

    private void Awake(){
        startPosition = transform.position;
        startRotation = transform.rotation;
        brushBody = GetComponent<Rigidbody>();
    }

    public void ReturnToStart(){
        // Gives XR Grab Interactable 0.05 secs to finish releasing
        CancelInvoke(nameof(ReturnNow));
        Invoke(nameof(ReturnNow), 0.05f);
    }

    // Resets position of paint brush to starting location
    private void ReturnNow(){
        brushBody.linearVelocity = Vector3.zero;
        brushBody.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(startPosition, startRotation);

        brushBody.Sleep();
    }
}