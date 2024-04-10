using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace hand_indicators
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static ConfigEntry<Color> leftc;
        public static ConfigEntry<Color> rightc;
        public static ConfigEntry<bool> _enabled;
        public static ConfigEntry<float> scale;
        public static ConfigEntry<float> opacity;
        private void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Plugin));
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            leftc = Config.Bind<Color>("Colors", "Left Indicator", Color.red, "The color of the indicator of your left hand.");
            rightc = Config.Bind<Color>("Colors", "Right Indicator", Color.blue, "The color of the indicator of your right hand.");
            _enabled = Config.Bind<bool>("Mod", "Enabled", true, "set to false to disable the mod");
            scale = Config.Bind<float>("Mod", "Indicator Scale", 1.0f, "A multiplier for the indicators' scale");
            opacity = Config.Bind<float>("Mod", "Opacity", 0.5f, "How transparent the indicators are (0 = invisible, 1 = max)");
        }

        [HarmonyPatch(typeof(ArmScript_v2), nameof(ArmScript_v2.Start))]
        [HarmonyPostfix]
        public static void onArmStart(ArmScript_v2 __instance) {
            Instance.Config.Reload();
            if (!_enabled.Value) return;
            var sprite = __instance.gameCursor.GetComponent<SpriteRenderer>();
            sprite.enabled = true;
            var col = __instance.isLeft ? leftc.Value : rightc.Value;
            col.a = opacity.Value;
            sprite.color = col;
            sprite.GetComponent<Transform>().localScale *= scale.Value;
        }
    }
}
