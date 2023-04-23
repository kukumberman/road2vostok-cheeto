using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cheeto.Core
{
    public class RuntimeRendererFeatureInstaller
    {
        private UniversalRenderPipelineAsset _asset;
        private UniversalRenderer _universalRenderer;
        private FieldInfo _rendererFeaturesField;
        private List<ScriptableRendererFeature> _rendererFeatures;

        private ScriptableRendererData[] _rendererDataList;
        private ScriptableRendererDataWrapper _data;

        public RuntimeRendererFeatureInstaller()
        {
            _asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            _universalRenderer = _asset.scriptableRenderer as UniversalRenderer;
            _rendererFeaturesField = typeof(ScriptableRenderer).GetField(
                "m_RendererFeatures",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            _rendererFeatures =
                _rendererFeaturesField.GetValue(_universalRenderer)
                as List<ScriptableRendererFeature>;
            var rendererDataField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_RendererDataList",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            _rendererDataList = rendererDataField.GetValue(_asset) as ScriptableRendererData[];

            //Debug.Log($"found {_rendererDataList.Length} renderers");

            _data = new ScriptableRendererDataWrapper(_rendererDataList[0]);
        }

        public void Add(ScriptableRendererFeature feature)
        {
            _data.AddFeature(feature);
            _rendererFeatures.Add(feature);
        }

        public void Remove(ScriptableRendererFeature feature) { }
    }

    public sealed class ScriptableRendererDataWrapper
    {
        private readonly ScriptableRendererData _data;

        public ScriptableRendererDataWrapper(ScriptableRendererData data)
        {
            _data = data;
        }

        public void AddFeature(ScriptableRendererFeature feature)
        {
            _data.rendererFeatures.Add(feature);
        }

        public void RemoveFeature(ScriptableRendererFeature feature)
        {
            for (int i = _data.rendererFeatures.Count - 1; i >= 0; i--)
            {
                if (_data.rendererFeatures[i] == feature)
                {
                    _data.rendererFeatures.RemoveAt(i);
                }
            }
        }
    }
}
