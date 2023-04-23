using System.Reflection;
using UnityEngine;
using UImGui;
using UImGui.Renderer;
using UImGui.Assets;

namespace Cheeto.Core
{
    public sealed class ImguiInstaller
    {
        public RenderImGui RendererFeature;
        private ShaderResourcesAsset ShaderAsset;
        private StyleAsset StyleAsset;

        private readonly UImGui.UImGui _imgui;

        public ImguiInstaller(UImGui.UImGui imgui)
        {
            _imgui = imgui;
            WithRendererFeature();
            WithShaderAsset();
        }

        private void WithRendererFeature()
        {
            RendererFeature = ScriptableObject.CreateInstance<RenderImGui>();
            RendererFeature.name = nameof(RenderImGui);
        }

        private void WithShaderAsset()
        {
            var bundlePath = System.IO.Path.Combine(Application.dataPath, "mybundle");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            ShaderAsset = bundle.LoadAsset<ShaderResourcesAsset>("ShaderResources");

            var shader1 = bundle.LoadAsset<Shader>("DearImGui-Mesh");
            var shader2 = bundle.LoadAsset<Shader>("DearImGui-Procedural");

            ShaderAsset.Shader = new ShaderData { Mesh = shader1, Procedural = shader2, };

            ShaderAsset.PropertyNames = new ShaderProperties
            {
                Texture = "_Texture",
                Vertices = "_Vertices",
                BaseVertex = "_BaseVertex"
            };

            StyleAsset = bundle.LoadAsset<StyleAsset>("StyleResources");
        }

        public void Apply()
        {
            var t = _imgui.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var featureField = t.GetField("_renderFeature", flags);
            featureField.SetValue(_imgui, RendererFeature);

            var shaderAssetField = t.GetField("_shaders", flags);
            shaderAssetField.SetValue(_imgui, ShaderAsset);

            //var styleAssetField = t.GetField("_style", flags);
            //styleAssetField.SetValue(_imgui, StyleAsset);
        }
    }
}
