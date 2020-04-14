using System.Reflection;
using StardewValley;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class StartupPreferencesRewrites
    {
        public class WriteSettingsRewrite
        {
            public static void Prefix(StartupPreferences __instance)
            {
                ModEntry.ModConfig.CurrentLanguageCode = (int)LocalizedContentManager.CurrentLanguageCode;
                ModEntry.SaveConfig();
                if (ModEntry.ModConfig.CurrentLanguageCode > ModEntry.ModConfig.OriginLocaleCount)
                {
                    typeof(StartupPreferences).GetField("languageCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, LocalizedContentManager.LanguageCode.en);
                }
            }
            public static void Postfix(StartupPreferences __instance)
            {
                if (ModEntry.ModConfig.CurrentLanguageCode > ModEntry.ModConfig.OriginLocaleCount)
                {
                    typeof(StartupPreferences).GetField("languageCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, ModEntry.ModConfig.CurrentLanguageCode);
                }
            }
        }
        public class ReadSettingsRewrite
        {
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
