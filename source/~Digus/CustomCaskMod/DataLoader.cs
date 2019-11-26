using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;

namespace CustomCaskMod
{
    internal class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static Dictionary<object, float> CaskData;
        public static Dictionary<int, float> CaskDataId =  new Dictionary<int, float>();

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CaskData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, float>>("data\\CaskData.json") ?? new Dictionary<object, float>() { { 342, 2.66f }, { 724, 2f } };
            DataLoader.Helper.Data.WriteJsonFile("data\\CaskData.json", CaskData);
            DataLoader.LoadContentPacksCommand();

            if (!ModConfig.DisableLetter)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCaskRecipe"
                        , I18N.Get("CustomCask.RecipeLetter")
                        , "Cask"
                        , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask.Letter") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && !Game1.player.craftingRecipes.ContainsKey("Cask")
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                );
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCask"
                        , I18N.Get("CustomCask.Letter")
                        , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask.RecipeLetter") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && Game1.player.craftingRecipes.ContainsKey("Cask")
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                );
            }
        }

        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, "CaskData.json")))
                {
                    CustomCaskModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    Dictionary<object, float> caskData = contentPack.ReadJsonFile<Dictionary<object, float>>("CaskData.json");
                    foreach (var caskItem in caskData)
                    {
                        DataLoader.CaskData[caskItem.Key] = caskItem.Value;
                    }
                }
                else
                {
                    CustomCaskModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an CaskData.json file.", LogLevel.Warn);
                }
            }
            FillCaskDataIds();
        }

        public static void FillCaskDataIds()
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            CaskData.ToList().ForEach(c =>
            {
                if (Int32.TryParse(c.Key.ToString(), out int id))
                {
                    CaskDataId[id] = c.Value;
                }
                else
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(c.Key + "/"));
                    if (pair.Value != null)
                    {
                        CaskDataId[pair.Key] = c.Value;
                    }
                }
            });
        }
    }
}