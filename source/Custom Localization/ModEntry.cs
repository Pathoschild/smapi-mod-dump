using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace StardewModdingAPI.Mods.CustomLocalization
{
    public class ModEntry : Mod
    {
        public static ModConfig ModConfig;

        public static IMonitor monitor;

        public static string ModPath;

        private static ModEntry Instance;

        public static void SaveConfig()
        {
            Instance.Helper.WriteConfig(ModConfig);
        }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            ModConfig = helper.ReadConfig<ModConfig>();
            ModPath = helper.DirectoryPath;
            monitor = this.Monitor;
            HarmonyInstance harmony = HarmonyInstance.Create("zaneyork.CustomLocalization");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (ModConfig.CurrentLanguageCode > ModConfig.OriginLocaleCount)
            {
                monitor.Log($"Restore locale to : {ModConfig.CurrentLanguageCode}");
                LocalizedContentManager.CurrentLanguageCode = (LocalizedContentManager.LanguageCode)ModConfig.CurrentLanguageCode;
            }
            else
            {
                Dictionary<string, bool> dictionary = this.Helper.Reflection.GetField<Dictionary<string, bool>>(Game1.content, "_localizedAsset").GetValue();
                dictionary.Clear();
                this.Helper.Reflection.GetMethod(Game1.game1, "TranslateFields").Invoke();
                if (!LocalizedContentManager.CurrentLanguageLatin)
                {
                    this.Helper.Reflection.GetMethod(typeof(SpriteText), "OnLanguageChange").Invoke(LocalizedContentManager.CurrentLanguageCode);
                }
            }
        }
    }
}
