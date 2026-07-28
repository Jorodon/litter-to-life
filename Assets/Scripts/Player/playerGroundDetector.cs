using UnityEngine;
using Game.Audio;

namespace Game.Player
{
    public class playerGroundDetector : MonoBehaviour
    {
        // Layer used for object surfaces and terrain
        [SerializeField]
        private LayerMask GroundLayer;

        // Class containing pairs of materials and correpsonding surface types
        [SerializeField]
        private TextureMaterial[] TextureMaterials;

        // Min velocity needed to trigger audio 
        [SerializeField] private float characterVelocityThreshold = 0.2f;

        // Variables for timer to determine how long player has to move or stop moving to trigger audio
        [Header("Player Move Time Thresholds")]
        [SerializeField] private float startDelay = 0.15f; 
        [SerializeField] private float stopDelay = 0.1f; 
        private float startDelayTimer = 0f;
        private float stopDelayTimer = 0f;

        // Player controller used for movement
        private CharacterController Controller;

        // Audio script used to control footstep audio
        private playerFootstepAudio playerFootstepAudioInstance;
        

        private string currentMaterial;
        private bool wasWalking = false;
        private Vector3 lastPosition;
        private float actualSpeed;

        // Gets components from attached object and sets position
        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            playerFootstepAudioInstance = GetComponent<playerFootstepAudio>();
            lastPosition = transform.position;
        }

        // Calculates position and speed every frame, then checks for movement on ground
        private void Update()
        {
            Vector3 currentPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 previousPos = new Vector3(lastPosition.x, 0f, lastPosition.z);
            actualSpeed = Vector3.Distance(currentPos, previousPos) / Time.deltaTime;
            lastPosition = transform.position;

            CheckGroundState();
        }

        // Checks for player movement and ground material to control audio
        private void CheckGroundState()
        {
            // Casts ray down to ground and returns true if hit
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position + Vector3.up * 0.2f;
            bool hitGround = Physics.Raycast(raycastOrigin, Vector3.down, out hit, 1.0f, GroundLayer);

            // Checks if moving faster than min threshold
            bool isSpeeding = actualSpeed > characterVelocityThreshold;
            bool isMoving = false;

            // Filters out sudden turns (snap turn) using timer
            if (isSpeeding)
            {
                startDelayTimer += Time.deltaTime;
                if (startDelayTimer >= startDelay)
                {
                    isMoving = true;
                }
            }
            else
            {
                startDelayTimer = 0f;
            }

            // Checks if player is moving and touching ground
            if (isMoving && hitGround)
            {
                stopDelayTimer = stopDelay;

                string groundMaterial = null;

                // Fetches ground material
                if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    groundMaterial = getTerrainMaterial(terrain, hit.point);
                }
                else if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    groundMaterial = getRendererMaterial(renderer);
                }

                // Starts footstep SFX if player was not already walking or changed surfaces
                if (!wasWalking || currentMaterial != groundMaterial)
                {
                    ControlFootstepSfx(groundMaterial);
                    wasWalking = true;
                }
            }

            // Filters out sudden stops (lag, etc) using timer : stops footstep audio
            else if (wasWalking)
            {
                stopDelayTimer -= Time.deltaTime;

                if (stopDelayTimer <= 0f)
                {
                    playerFootstepAudioInstance.StopFootsteps();
                    wasWalking = false;
                    currentMaterial = null;
                }
            }
        }
        
        // Gets the material of the renderer and returns it
        private string getRendererMaterial(Renderer renderer)
        {
            if (renderer == null || renderer.sharedMaterial == null) return null;

            Texture mainTex = renderer.sharedMaterial.mainTexture;

            foreach (TextureMaterial textureMaterial in TextureMaterials)
            {
                if (textureMaterial.Albedo == mainTex)
                {
                    return textureMaterial.MaterialName;
                }
            }
            return null;
        }

        // Gets the highest valued material of the terrain and returns it
        private string getTerrainMaterial(Terrain terrain, Vector3 hitPoint)
        {
            Vector3 terrainPosition = hitPoint - terrain.transform.position;
            Vector3 splatMapPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);
        
            int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
            int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

            int activeIndex = 0;
            for (int i = 1; i < alphaMap.GetLength(2); i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0, 0, activeIndex])
                {
                    activeIndex = i;
                }
            }

            foreach (TextureMaterial textureMaterial in TextureMaterials)
            {
                if (textureMaterial.Albedo == terrain.terrainData.terrainLayers[activeIndex].diffuseTexture)
                {
                    return textureMaterial.MaterialName;
                }
            }
            return null;
        }

        //Controls changing footstep sounds
        public void ControlFootstepSfx(string newMaterial)
        {
            playerFootstepAudioInstance.ChangeGroundMaterial(newMaterial);
            currentMaterial = newMaterial;
            playerFootstepAudioInstance.StartFootsteps();
        }

        //Custom serialized class for defining and storing textures and surface names
        [System.Serializable]
        private class TextureMaterial
        {
            public Texture Albedo;
            public string MaterialName;
        }
    }
}