using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SandrockTimeScale
{
    [BepInPlugin("sandrock.timescale.mod", "Sandrock TimeScale", "1.0.0")]
    public class TimeScalePlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private ConfigEntry<bool> ModEnabled;
        private ConfigEntry<bool> DebugEnabled;

        private ConfigEntry<KeyCode> SlowKey;
        private ConfigEntry<KeyCode> ResetKey;
        private ConfigEntry<KeyCode> SpeedKey;

        private ConfigEntry<float> SlowValue;
        private ConfigEntry<float> SpeedValue;

        internal static bool IsLocked = false;
        internal static float TargetScale = 1f;
        internal static bool DebugMode = false;

        private void Awake()
        {
            Log = Logger;

            ModEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the mod");
            DebugEnabled = Config.Bind("General", "Debug", false, "Enable debug logging");
            DebugMode = DebugEnabled.Value;

            SlowKey = Config.Bind("Controls", "SlowKey", KeyCode.LeftAlt, "Key for slow motion");
            ResetKey = Config.Bind("Controls", "ResetKey", KeyCode.Z, "Key to reset to 1.0");
            SpeedKey = Config.Bind("Controls", "SpeedKey", KeyCode.Keypad0, "Key for speed up");

            SlowValue = Config.Bind("Scaling", "SlowValue", 0.25f, "timeScale when SlowKey is pressed");
            SpeedValue = Config.Bind("Scaling", "SpeedValue", 2.0f, "timeScale when SpeedKey is pressed");

            Harmony.CreateAndPatchAll(typeof(SetTimeScalePatch));
            if (DebugMode) Log.LogInfo("Sandrock TimeScale Mod loaded with Harmony patch.");
        }

        private void Update()
        {
            if (!ModEnabled.Value)
            {
                IsLocked = false;
                return;
            }

            if (Input.GetKeyDown(SlowKey.Value))
            {
                TargetScale = SlowValue.Value;
                IsLocked = true;
                if (DebugMode) Log.LogInfo($"Slow: target = {TargetScale}, locked");
            }
            else if (Input.GetKeyDown(ResetKey.Value))
            {
                TargetScale = 1f;
                IsLocked = true;
                if (DebugMode) Log.LogInfo($"Reset: target = {TargetScale}, locked");
            }
            else if (Input.GetKeyDown(SpeedKey.Value))
            {
                TargetScale = SpeedValue.Value;
                IsLocked = true;
                if (DebugMode) Log.LogInfo($"Speed: target = {TargetScale}, locked");
            }

            if (IsLocked && Time.timeScale != TargetScale)
            {
                Time.timeScale = TargetScale;
                if (DebugMode) Log.LogInfo($"Forced timeScale = {Time.timeScale}");
            }
        }
    }

    [HarmonyPatch(typeof(Time), "set_timeScale")]
    public static class SetTimeScalePatch
    {
        static bool Prefix(ref float value)
        {
            if (TimeScalePlugin.IsLocked && value != TimeScalePlugin.TargetScale)
            {
                if (TimeScalePlugin.DebugMode)
                    TimeScalePlugin.Log.LogInfo($"Intercepted set_timeScale({value}) -> replaced with {TimeScalePlugin.TargetScale}");
                value = TimeScalePlugin.TargetScale;
            }
            return true;
        }
    }
}