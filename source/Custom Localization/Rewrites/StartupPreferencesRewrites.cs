using System.IO;
using System.Reflection;
using Harmony;
using StardewValley;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class StartupPreferencesRewrites
    {
        [HarmonyPatch(typeof(StartupPreferences))]
        [HarmonyPatch("writeSettings")]
        public class WriteSettingsRewrite
        {
            [HarmonyPrefix]
            public static void Prefix(StartupPreferences __instance)
            {
                ModEntry.ModConfig.CurrentLanguageCode = (int)LocalizedContentManager.CurrentLanguageCode;
                ModEntry.SaveConfig();
                if (ModEntry.ModConfig.CurrentLanguageCode > ModEntry.ModConfig.OriginLocaleCount)
                {
                    typeof(StartupPreferences).GetField("languageCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, LocalizedContentManager.LanguageCode.en);
                }
            }
            [HarmonyPostfix]
            public static void Postfix(StartupPreferences __instance)
            {
                if (ModEntry.ModConfig.CurrentLanguageCode > ModEntry.ModConfig.OriginLocaleCount)
                {
                    typeof(StartupPreferences).GetField("languageCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, ModEntry.ModConfig.CurrentLanguageCode);
                }
            }
        }
        [HarmonyPatch(typeof(StartupPreferences))]
        [HarmonyPatch("readSettings")]
        public class ReadSettingsRewrite
        {
            [HarmonyPostfix]
            public static void Postfix(StartupPreferences __instance)
            {
                if (ModEntry.ModConfig.CurrentLanguageCode > ModEntry.ModConfig.OriginLocaleCount)
                {
                    typeof(StartupPreferences).GetField("languageCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, ModEntry.ModConfig.CurrentLanguageCode);
                }
            }
        }
    }
}
