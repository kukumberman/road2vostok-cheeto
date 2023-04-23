using UnityEngine;
using UnityEngine.SceneManagement;
using Cheeto.Core.Chams;
using ImGuiNET;

namespace Cheeto.Core
{
    public sealed class CheetoBehaviour : MonoBehaviour
    {
        private const string kMenuSceneName = "Menu";
        private const string kSandboxSceneName = "Sandbox";

        private readonly Vector3 kSandboxStartPosition = new Vector3(19.5f, 0.05f, 7.2f);
        private readonly float kSandboxStartAngle = 250;

        private Material[] _defaultMaterials = null;
        private ChamsURP _chams = null;
        private int _enemyLayer = 0;

        private AI[] _enemies;
        private int _ragdollLayer = 0;

        private RuntimeRendererFeatureInstaller _runtimeRendererFeatures;

        private GameObject _imguiGameObject;
        private UImGui.UImGui _imgui;
        private ImguiInstaller _imguiInstaller;
        private Camera _currentCamera;

        private bool _showWindow = true;
        private bool _showDemoWindow = false;
        private int _imguiCounter = 0;
        private bool _drawBones = true;

        private Vector3 _color1;
        private Vector3 _color2;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name == kMenuSceneName)
            {
                SceneManager.LoadScene(kSandboxSceneName);
            }
        }

        private void OnEnable()
        {
            _runtimeRendererFeatures = new RuntimeRendererFeatureInstaller();

            Debug.Log("OnEnable");
            Debug.Log(Application.unityVersion);

            var enemyPrefab = Resources.Load<GameObject>("AI");
            _defaultMaterials = enemyPrefab.GetComponentInChildren<SkinnedMeshRenderer>().materials;

            _enemyLayer = LayerMask.NameToLayer("Enemy");
            _ragdollLayer = LayerMask.NameToLayer("Ragdoll");

            var chamsLayerMask = 1 << _enemyLayer;
            _chams = new ChamsURP(chamsLayerMask);

            _chams.Create(_runtimeRendererFeatures);

            SceneManager.sceneLoaded += OnSceneLoaded;

            _imguiGameObject = new GameObject("Imgui");
            _imguiGameObject.transform.SetParent(transform);
            _imguiGameObject.SetActive(false);

            _imgui = _imguiGameObject.AddComponent<UImGui.UImGui>();
            _currentCamera = Camera.main;
            _imgui.SetCamera(_currentCamera);
            _imgui.Layout += Imgui_Layout;
            _imgui.OnInitialize += (obj) => Debug.Log("OnInitialize");

            _imguiInstaller = new ImguiInstaller(_imgui);
            _imguiInstaller.Apply();

            _runtimeRendererFeatures.Add(_imguiInstaller.RendererFeature);

            _imguiGameObject.SetActive(true);

            _color1 = ColorToVector3(_chams.DefaultColor);
            _color2 = ColorToVector3(_chams.WallColor);
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");

            SceneManager.sceneLoaded -= OnSceneLoaded;

            _chams.Dispose();

            _imgui.Layout -= Imgui_Layout;
            Destroy(_imgui);
            _imgui = null;
            Destroy(_imguiGameObject);
            _imguiGameObject = null;
        }

        private void Update()
        {
            if (_currentCamera != Camera.main)
            {
                _currentCamera = Camera.main;
                _imgui.SetCamera(_currentCamera);
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                _showWindow = !_showWindow;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                _chams.Active = !_chams.Active;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                ReloadScene();
            }

            _enemies = FindObjectsOfType<AI>();

            foreach (var enemy in _enemies)
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

        private void Imgui_Layout(UImGui.UImGui obj)
        {
            if (_showWindow)
            {
                DrawWindow();
            }

            if (_showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }

            DrawOverlayExample();
        }

        private void DrawWindow()
        {
            ImGui.Begin("Bepinex");

            ImGui.Text($"Hello, world! {_imguiCounter}");

            if (ImGui.Button("Click me"))
            {
                _imguiCounter++;
                Debug.Log("Clicked!");
            }

            ImGui.Checkbox("Show Demo Window", ref _showDemoWindow);

            if (ImGui.ColorEdit3("Color 1", ref _color1))
            {
                _chams.DefaultColor = Vector3ToColor(_color1);
            }

            if (ImGui.ColorEdit3("Color 2", ref _color2))
            {
                _chams.WallColor = Vector3ToColor(_color2);
            }

            ImGui.Checkbox("Draw Bones", ref _drawBones);

            ImGui.End();
        }

        private void DrawOverlayExample()
        {
            var draw = ImGui.GetBackgroundDrawList();

            //ARGB as uint, RGBA as Vector4
            var red = ImGui.ColorConvertFloat4ToU32(new Vector4(255, 0, 0, 255));
            var green = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 255, 0, 255));
            var blue = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 255, 255));

            draw.AddLine(new Vector2(0, 0), new Vector2(Screen.width, Screen.height), red);
            draw.AddLine(new Vector2(Screen.width, 0), new Vector2(0, Screen.height), green);
            draw.AddCircle(new Vector2(Screen.width, Screen.height) * 0.5f, 100, blue);

            if (_drawBones)
            {
                foreach (var enemy in _enemies)
                {
                    DrawEnemySkeleton(enemy);
                }
            }
        }

        private void DrawEnemySkeleton(AI enemy)
        {
            var draw = ImGui.GetBackgroundDrawList();

            var blue = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 255, 255));
            var black = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 255));

            var armature = enemy.transform.GetChild(2);
            var bones = armature.GetComponentsInChildren<Transform>();

            for (int i = 0; i < bones.Length; i++)
            {
                var bone = bones[i];

                if (bone.gameObject.layer != _ragdollLayer)
                {
                    continue;
                }

                var position = _currentCamera.WorldToScreenPoint(bone.transform.position);
                if (position.z <= 0)
                {
                    continue;
                }

                position.y = Screen.height - position.y;

                draw.AddCircle(position, 10, blue);
                draw.AddText(position, black, $"{i}");
            }
        }

        private static Color Vector3ToColor(Vector3 vec)
        {
            return new Color(vec.x, vec.y, vec.z, 1);
        }

        private static Vector3 ColorToVector3(Color col)
        {
            return new Vector3(col.r, col.g, col.b);
        }
    }
}
