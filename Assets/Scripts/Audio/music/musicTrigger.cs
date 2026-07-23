using UnityEngine;

namespace Game.Audio.Music.Test
{
    public class musicTrigger : MonoBehaviour
    {
        // Increases the music state of background music when trigger is hit by player
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                backgroundMusicTest.Instance.IncreaseMusicState();
            }
        }
            
    }
}
