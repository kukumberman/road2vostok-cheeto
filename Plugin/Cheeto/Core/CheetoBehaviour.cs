using UnityEngine;
using UnityEngine.SceneManagement;
using Cheeto.Core.Chams;

namespace Cheeto.Core
{
    public sealed class CheetoBehaviour : MonoBehaviour
    {
        private const string kSandboxSceneName = "Sandbox";

        private readonly Vector3 kSandboxStartPosition = new Vector3(19.5f, 0.05f, 7.2f);
        private readonly float kSandboxStartAngle = 250;

        private Material[] _defaultMaterials = null;
        private ChamsURP _chams = null;
        private int _enemyLayer = 0;

        private void Awake()
        {
            SceneManager.LoadScene(kSandboxSceneName);
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");

            var enemyPrefab = Resources.Load<GameObject>("AI");
            _defaultMaterials = enemyPrefab.GetComponentInChildren<SkinnedMeshRenderer>().materials;

            _enemyLayer = LayerMask.NameToLayer("Enemy");

            var chamsLayerMask = 1 << _enemyLayer;
            _chams = new ChamsURP(chamsLayerMask);

            _chams.Create();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");

            SceneManager.sceneLoaded -= OnSceneLoaded;

            _chams.Dispose();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                _chams.Active = !_chams.Active;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                ReloadScene();
            }

            foreach (var enemy in FindObjectsOfType<AI>())
            {
                var renderers = enemy.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers)
                {
                    rend.gameObject.layer = _enemyLayer;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == kSandboxSceneName)
            {
                Invoke(nameof(TeleportPlayer), 0.5f);
            }
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void TeleportPlayer()
        {
            var player = FindObjectOfType<Controller>();
            // var cc = player.GetComponent<CharacterController>();
            // cc.enabled = false;
            player.transform.position = kSandboxStartPosition;
            // cc.enabled = true;
            player.transform.rotation = Quaternion.AngleAxis(kSandboxStartAngle, Vector3.up);
        }
    }
}
