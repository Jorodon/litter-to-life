using UnityEngine;
using System.Collections;
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

        private CharacterController Controller;
        private string currentMaterial;

        // Instance of player footstep audio
        playerFootstepAudio playerFootstepAudioInstance;

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            playerFootstepAudioInstance = GetComponent<playerFootstepAudio>();
        }

        private void Start()
        {
            StartCoroutine(CheckGround());
        }

        // Uses raycaster to check if player is touching ground
        private IEnumerator CheckGround()
        {
            while (true)
            {
                RaycastHit hit;
                if (Controller.isGrounded && Controller.velocity != Vector3.zero && Physics.Raycast(transform.position, Vector3.down, out hit, 5f, GroundLayer))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);

                    // Start coroutine for finding ground material of terrain
                    if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                    {
                        yield return StartCoroutine(getTerrainMaterial(terrain, hit.point));

                    }

                    // Start coroutine for finding ground material of renderer
                    else if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer))
                    {
                        yield return StartCoroutine(getRendererMaterial(renderer));
                    }
                }

                else
                {
                    playerFootstepAudioInstance.StopFootsteps();
                }

                yield return null;
            }
        }

        // Gets the material of the renderer and applies that sound to footsteps
        private IEnumerator getRendererMaterial(Renderer renderer)
        {
            foreach(TextureMaterial textureMaterial in TextureMaterials)
            {
                if (textureMaterial.Albedo == renderer.sharedMaterial.GetTexture("_MainTex"))
                {
                    ControlFootstepSfx(textureMaterial.MaterialName);
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }
        }

        // Gets the highest valued material of the terrain and applies that sound to footsteps
        private IEnumerator getTerrainMaterial(Terrain terrain, Vector3 hitPoint)
        {
            Vector3 terrainPosition = hitPoint - terrain.transform.position;
            Vector3 splatMapPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);
        
            int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
            int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

            int activeIndex = 0;
            for (int i = 1; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0, 0, activeIndex])
                {
                    activeIndex = i;
                }
            }

            foreach(TextureMaterial textureMaterial in TextureMaterials)
            {
                if (textureMaterial.Albedo == terrain.terrainData.terrainLayers[activeIndex].diffuseTexture)
                {
                    ControlFootstepSfx(textureMaterial.MaterialName);
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }
        }

        //Controls changing footstep sounds
        public void ControlFootstepSfx(string newMaterial)
        {
            if (currentMaterial != newMaterial)
            {
                Debug.Log("Changed audio:");
                Debug.Log(newMaterial);
                playerFootstepAudioInstance.ChangeGroundMaterial(newMaterial);
                currentMaterial = newMaterial;
            }
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
