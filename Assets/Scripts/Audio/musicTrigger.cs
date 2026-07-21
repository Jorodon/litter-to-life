using UnityEngine;
using FMODUnity;

public class musicTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            backgroundMusicManager.Instance.IncreaseMusicState();
        }
    }
        
}
