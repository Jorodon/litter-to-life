using UnityEngine;

public class GrabberHead : MonoBehaviour
{
    private GameObject heldTrash;

    private void OnTriggerEnter(Collider other)
    {
        // Already holding something
        if (heldTrash != null)
            return;

        // Only grab trash
        if (!other.CompareTag("Trash"))
            return;

        PickupTrash(other.gameObject);
    }

    private void PickupTrash(GameObject trash)
    {
        heldTrash = trash;

        Rigidbody rb = trash.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Attach the trash to the grabber head
        trash.transform.SetParent(transform);
        trash.transform.localPosition = Vector3.zero;
        trash.transform.localRotation = Quaternion.identity;
    }
}