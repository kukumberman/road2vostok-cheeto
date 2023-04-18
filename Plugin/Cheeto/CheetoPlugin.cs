using BepInEx;
using Cheeto.Core;

namespace Cheeto
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public sealed class CheetoPlugin : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "com.cucumba.cheetos.road2vostok";
        private const string PLUGIN_NAME = "Road2Vostok";
        private const string PLUGIN_VERSION = "0.0.1";

        private CheetoBehaviour _cheetoBehaviour;

        private void OnEnable()
        {
            _cheetoBehaviour = gameObject.AddComponent<CheetoBehaviour>();
        }

        private void OnDisable()
        {
            Destroy(_cheetoBehaviour);
            _cheetoBehaviour = null;
        }
    }
}
