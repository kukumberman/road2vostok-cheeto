using UnityEngine;

namespace Cheeto.Core.Chams
{
    public sealed class ChamsStandard
    {
        private const string kShaderName = "Hidden/Internal-Colored";

        public readonly Material[] Materials = new Material[2];

        public void Create()
        {
            Materials[0] = CreateMaterial_1();
            Materials[1] = CreateMaterial_2();
        }

        public void Dispose()
        {
            for (int i = 0; i < Materials.Length; i++)
            {
                Object.Destroy(Materials[i]);
                Materials[i] = null;
            }
        }

        private Material CreateEmptyMaterial()
        {
            var material = new Material(Shader.Find(kShaderName));
            material.renderQueue = 3001;
            return material;
        }

        private Material CreateMaterial_1()
        {
            var material = CreateEmptyMaterial();
            material.name = "Chams Visible Behind Wall";
            material.SetInt("_Cull", 0);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_ZTest", 8);
            material.SetColor("_Color", new Color(1, 0, 0));
            return material;
        }

        private Material CreateMaterial_2()
        {
            var material = CreateEmptyMaterial();
            material.name = "Chams Visible Default";
            material.SetColor("_Color", new Color(1, 1, 0));
            return material;
        }
    }
}
