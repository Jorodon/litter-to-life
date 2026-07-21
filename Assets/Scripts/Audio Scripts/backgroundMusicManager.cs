using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class backgroundMusicManager : MonoBehaviour
{
    // Allows other scripts to access music manager, but not modify values
    public static backgroundMusicManager Instance { get; private set; }
    public int musicState = 0;

    // Serialized field to get music event
    [Header("Background Music Event")]
    [SerializeField] private EventReference musicEventReference;
    private EventInstance musicEventInstance;

    public MonoBehaviour targetScript;

    // Runs before Start() to prevent duplicates, set a single Instance, and ensure it survives across scenes
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Starts the selected music event
    void Start()
    {
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        musicEventInstance.start();

        targetScript.enabled = false;
    }

    // Allows changing the parameter of the selected music event
    public void SetMusicParameter(string parameterName, float value)
    {
        musicEventInstance.setParameterByName(parameterName, value);
    }

    public void IncreaseMusicState()
    {
        musicState++;
        SetGlobalMusicParameter("Music State", musicState);

        if (musicState == 5)
        {
            targetScript.enabled = true;
        }
    }

    // Allows changing the parameter of the selected music event
    public void SetGlobalMusicParameter(string parameterName, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
    }

    // Stops playback of the current music event
    public void StopMusic()
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicEventInstance.release();
    }

    // Stops playback of current music event and changes to a different music event
    public void ChangeMusic(EventReference newMusicEventReference)
    {
        StopMusic();
        musicEventReference = newMusicEventReference;

        musicEventInstance = RuntimeManager.CreateInstance(newMusicEventReference);
        musicEventInstance.start();
    }

    // Stops playback of music event when object is destroyed
    private void OnDestroy()
    {
        StopMusic();
    }
}
