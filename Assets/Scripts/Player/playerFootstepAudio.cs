using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class playerFootstepAudio : MonoBehaviour
{

    public static playerFootstepAudio Instance { get; private set; }

    [Header("Footstep Sounds")]
    [SerializeField] private EventReference footstepReference;
    private EventInstance footstepInstance;

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

    public void Start()
    {
        footstepInstance = RuntimeManager.CreateInstance(footstepReference);
        RuntimeManager.AttachInstanceToGameObject(footstepInstance, gameObject);
    }

    public void StartFootsteps()
    {
        footstepInstance.start();
        RuntimeManager.StudioSystem.setParameterByName("isWalking", 1.0f);
    }

    public void ChangeGroundMaterial(string material)
    {
        footstepInstance.setParameterByNameWithLabel("Surface", material);
    }

    public void StopFootsteps()
    {
        RuntimeManager.StudioSystem.setParameterByName("isWalking", 0);
        footstepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        StopFootsteps();
        footstepInstance.release();
    }
}

