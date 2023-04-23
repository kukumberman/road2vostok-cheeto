using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

namespace Cheeto.Core.Chams
{
    public sealed class ChamsFeature : RenderObjects
    {
        private int _called = 0;

        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData
        )
        {
            base.AddRenderPasses(renderer, ref renderingData);
            _called++;
        }
    }

    // https://www.youtube.com/watch?v=GAh225QNpm0
    public sealed class ChamsURP
    {
        private const string kShaderName = "Universal Render Pipeline/Unlit";
        private const string kShaderColorPropertyName = "_BaseColor";

        private Material _chamsWallMaterial = null;
        private Material _chamsMaterial = null;

        private RenderObjects _featureWall = null;
        private RenderObjects _featureDefault = null;

        private bool _isActive = true;
        private Color _wallColor = Color.red;
        private Color _defaultColor = Color.yellow;

        public readonly int LayerMask;

        public bool Active
        {
            get => _isActive;
            set
            {
                _isActive = value;
                SetActive(_isActive);
            }
        }

        public Color WallColor
        {
            get => _wallColor;
            set
            {
                _wallColor = value;
                _chamsWallMaterial.SetColor(kShaderColorPropertyName, value);
            }
        }

        public Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                _chamsMaterial.SetColor(kShaderColorPropertyName, _defaultColor);
            }
        }

        public ChamsURP(int layerMask)
        {
            LayerMask = layerMask;
        }

        private void SetActive(bool active)
        {
            _featureWall.SetActive(active);
            _featureDefault.SetActive(active);
        }

        public void Create(RuntimeRendererFeatureInstaller runtimeRendererFeatures)
        {
            var shader = Shader.Find(kShaderName);

            _chamsWallMaterial = new Material(shader);
            WallColor = _wallColor;

            _chamsMaterial = new Material(shader);
            DefaultColor = _defaultColor;

            _featureWall = CreateEmptyFeature();
            _featureDefault = CreateEmptyFeature();

            InitWallFeature();
            InitDefaultFeature();

            _featureWall.Create();
            _featureDefault.Create();

            runtimeRendererFeatures.Add(_featureWall);
            runtimeRendererFeatures.Add(_featureDefault);

            SetActive(_isActive);
        }

        public void Dispose()
        {
            SetActive(false);

            //_rendererFeatures =
            //    _rendererFeaturesField.GetValue(_universalRenderer)
            //    as List<ScriptableRendererFeature>;
            //_rendererFeatures.RemoveAt(_rendererFeatures.Count - 1);
            //_rendererFeatures.RemoveAt(_rendererFeatures.Count - 1);
            //_rendererFeaturesField.SetValue(_universalRenderer, _rendererFeatures);

            //Debug.Log($"features after Dispose: {_rendererFeatures.Count}");

            //_data.RemoveFeature(_featureDefault);
            //_data.RemoveFeature(_featureWall);

            DisposeFeature(_featureDefault);
            DisposeFeature(_featureWall);

            Destroy(_chamsWallMaterial);
            Destroy(_chamsMaterial);
        }

        private RenderObjects CreateEmptyFeature()
        {
            //return ScriptableObject.CreateInstance<MyShittyFeature>();
            var f = new ChamsFeature();
            f.Create();
            return f;
        }

        private void DisposeFeature(ScriptableRendererFeature feature)
        {
            feature.Dispose();

            Destroy(feature);
        }

        private void Destroy(Object instance)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(instance);
            }
            else
            {
                Object.DestroyImmediate(instance);
            }
        }

        private void InitWallFeature()
        {
            _featureWall.name = "ChamsFeature";
            _featureWall.settings.Event = RenderPassEvent.AfterRenderingOpaques;
            _featureWall.settings.filterSettings.RenderQueueType = RenderQueueType.Opaque;
            _featureWall.settings.filterSettings.LayerMask.value = LayerMask;
            _featureWall.settings.overrideMaterial = _chamsWallMaterial;
            _featureWall.settings.overrideDepthState = true;
            _featureWall.settings.enableWrite = false;
            _featureWall.settings.depthCompareFunction = CompareFunction.Greater;
        }

        private void InitDefaultFeature()
        {
            _featureDefault.name = "DefaultFeature";
            _featureDefault.settings.Event = RenderPassEvent.AfterRenderingOpaques;
            _featureDefault.settings.filterSettings.RenderQueueType = RenderQueueType.Opaque;
            _featureDefault.settings.filterSettings.LayerMask.value = LayerMask;
            _featureDefault.settings.overrideMaterial = _chamsMaterial;
        }
    }
}
