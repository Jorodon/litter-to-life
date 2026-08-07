using Game.Audio.Music;
using UnityEngine;

public class backgroundMusicTutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("player entered");
            backgroundMusicListener.Instance.IncreaseMusicState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("player exited");
            Destroy(gameObject);
        }
    }
}
