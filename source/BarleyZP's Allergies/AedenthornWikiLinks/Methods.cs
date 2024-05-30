/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.WildTrees;
using StardewValley.ItemTypeDefinitions;
using System.Linq;

namespace WikiLinks
{
    public partial class ModEntry
    {
        private static bool englishGameEnglishWiki => Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en &&
                                          Config.WikiLang == "English";

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void OpenPage(string page)
        {
            SMonitor.Log($"Opening wiki page for {page}");

            if (Config.SendToBack)
                SetWindowPos(Process.GetCurrentProcess().MainWindowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);

            //ShowWindow(Process.GetCurrentProcess().MainWindowHandle, SW_MINIMIZE);

            string prefix = "";
            if (ModEntry.Config.WikiLang == "Auto-Detect")  // get the right wiki
            {
                LocalizedContentManager.LanguageCode code = Game1.content.GetCurrentLanguage();
                prefix = code switch
                {
                    LocalizedContentManager.LanguageCode.de => "de.",
                    LocalizedContentManager.LanguageCode.es => "es.",
                    LocalizedContentManager.LanguageCode.fr => "fr.",
                    LocalizedContentManager.LanguageCode.it => "it.",
                    LocalizedContentManager.LanguageCode.ja => "ja.",
                    LocalizedContentManager.LanguageCode.ko => "ko.",
                    LocalizedContentManager.LanguageCode.hu => "hu.",
                    LocalizedContentManager.LanguageCode.pt => "pt.",
                    LocalizedContentManager.LanguageCode.ru => "ru.",
                    LocalizedContentManager.LanguageCode.zh => "zh.",
                    _ => ""
                };
            }

            var ps = new ProcessStartInfo($"https://{prefix}stardewvalleywiki.com/{page}")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        public static string GetWikiPageForItem(Item obj, ITranslationHelper helper)
        {
            if (Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki)
            {
                // is there a key in i18n json for qualified id?
                string translated = helper.Get(obj.QualifiedItemId).UsePlaceholder(false);
                return translated ?? obj.DisplayName;
            }

            // we use english wiki but we arent playing in english; search with internal name
            // is there a key in i18n default for internal name?
            var translatedDict = helper.GetInAllLocales(obj.QualifiedItemId);
            if (translatedDict.ContainsKey("default")) return translatedDict["default"] ?? obj.Name;

            return obj.Name;
        }

        public static string GetWikiPageForPet(ITranslationHelper helper)
        { 
            return Config.WikiLang == "Auto-Detect" ? helper.Get("pet").Default("Pet") : "Pet";
        }

        public static string GetWikiPageForFarmAnimal(FarmAnimal animal)
        {
            if (Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki)
            {
                return animal.shortDisplayType();
            }

            if (animal.type.Value.Contains("Cow")) return "Cow";
            else if (animal.type.Value.Contains("Chicken")) return "Chicken";
            else if (animal.type.Value.Contains("Pig")) return "Pig";
            return animal.type.Value;
        }

        public static string GetWikiPageForNpcOrMonster(NPC npc)
        {
            if (Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki)
            {
                return npc.displayName;
            }

            return npc.Name;
        }

        public static string GetWikiPageForCrop(HoeDirt dirt)
        {
            var obj = new StardewValley.Object(dirt.crop.indexOfHarvest.Value, 1);

            if (Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki)
            {
                return obj.DisplayName;
            }
            return obj.Name;
        }

        public static string GetWikiPageForTree(Tree tree)
        {
            WildTreeData data = tree.GetData();
            if (data == null) return "";
            ParsedItemData seedData = ItemRegistry.GetData(data.SeedItemId);
            return Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki ? seedData.DisplayName : seedData.InternalName;
        }

        public static string GetWikiPageForFruitTree(FruitTree tree)
        {
            StardewValley.GameData.FruitTrees.FruitTreeData data = tree.GetData();
            if (data == null || data.Fruit.Count == 0) return "";
            ParsedItemData fruitData = ItemRegistry.GetData(data.Fruit[0].ItemId);
            return Config.WikiLang == "Auto-Detect" || englishGameEnglishWiki ? tree.GetDisplayName() : fruitData.InternalName;
        }
    }
}