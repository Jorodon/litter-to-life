using Game.Audio.Music;
using UnityEngine;
using Game.Environment;

public class backgroundMusicTutorialTrigger : MonoBehaviour
{

    [SerializeField] 
    private fogManager fogManagerTest;

    [SerializeField]
    private GameObject cabinFog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("player entered");
            backgroundMusicListener.Instance.IncreaseMusicState();
            fogManagerTest.SetFogState(4, true);
            //Destroy(cabinFog);
            fogManagerTest.DestroyCabinFog();
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
