using System.Reflection;
using Harmony;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class SpriteTextRewrites
    {
        private static void LoadFont()
        {
            ModConfig.Locale locale = ModEntry.ModConfig.GetByCode((int)LocalizedContentManager.CurrentLanguageCode);
            if (locale != null && !locale.IsLatin)
            {
                bool configChanged = false;
                if(locale.FontFileName == null)
                {
                    locale.FontFileName = "Fonts\\Chinese";
                    configChanged = true;
                }
                if(locale.FontPixelZoom <= 0)
                {
                    locale.FontPixelZoom = 1.5f;
                    configChanged = true;
                }
                if (configChanged)
                {
                    ModEntry.SaveConfig();
                }
                object fontFile = typeof(SpriteText).GetMethod("loadFont", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { locale.FontFileName });
                typeof(SpriteText).GetField("FontFile", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, fontFile);
                SpriteText.fontPixelZoom = locale.FontPixelZoom;
            }
        }
        [HarmonyPatch(typeof(SpriteText))]
        [HarmonyPatch("setUpCharacterMap")]
        public class SetUpCharacterMapRewrite
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                if (!LocalizedContentManager.CurrentLanguageLatin)
                {
                    if(typeof(SpriteText).GetField("_characterMap", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) == null)
                    {
                        LoadFont();
                    }
                }
            }


        }
        [HarmonyPatch(typeof(SpriteText))]
        [HarmonyPatch("OnLanguageChange")]
        public class OnLanguageChangeRewrite
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                LoadFont();
            }
        }
    }
}
