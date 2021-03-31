/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomCaskMod.integrations;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;

namespace CustomCaskMod
{
    internal class DataLoader
    {
        private const string CaskDataJson = "CaskData.json";
        private const string AgersDataJson = "AgersData.json";

        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static Dictionary<object, float> CaskData;
        public static Dictionary<int, float> CaskDataId =  new Dictionary<int, float>();

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CaskData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, float>>("data\\CaskData.json") ?? new Dictionary<object, float>() { { 342, 2.66f }, { 724, 2f } };
            DataLoader.Helper.Data.WriteJsonFile("data\\CaskData.json", CaskData);
            DataLoader.LoadContentPacksCommand();

            MailDao.SaveLetter
            (
                new Letter
                (
                    "CustomCaskRecipe"
                    , I18N.Get("CustomCask.RecipeLetter")
                    , "Cask"
                    , (l) => !DataLoader.ModConfig.DisableLetter && !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && !Game1.player.craftingRecipes.ContainsKey("Cask")
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );
            MailDao.SaveLetter
            (
                new Letter
                (
                    "CustomCask"
                    , I18N.Get("CustomCask.Letter")
                    , (l) => !DataLoader.ModConfig.DisableLetter && !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCaskRecipe") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && Game1.player.craftingRecipes.ContainsKey("Cask")
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );
            
            CreateConfigMenu(manifest);
        }

        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                bool hasFile = false;
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, CaskDataJson)))
                {
                    hasFile = true;
                    CustomCaskModEntry.ModMonitor.Log($"Reading file {AgersDataJson} from content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    Dictionary<object, float> caskData = contentPack.ReadJsonFile<Dictionary<object, float>>(CaskDataJson);
                    foreach (var caskItem in caskData)
                    {
                        DataLoader.CaskData[caskItem.Key] = caskItem.Value;
                    }
                }
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, AgersDataJson)))
                {
                    hasFile = true;
                    CustomCaskModEntry.ModMonitor.Log($"Reading file {AgersDataJson} from content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<CustomAger> agersData = contentPack.ReadJsonFile<List<CustomAger>>(AgersDataJson);
                    foreach (CustomAger customAger in agersData)
                    {
                        if (customAger.Name != "Cask")
                        {
                            customAger.ModUniqueID = contentPack.Manifest.UniqueID;
                            if (AgerController.GetAger(customAger.Name) is CustomAger currentAger)
                            {
                                if (currentAger.ModUniqueID != customAger.ModUniqueID)
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Both mods '{currentAger.ModUniqueID}' and '{customAger.ModUniqueID}' have data for  '{customAger.Name}'. You should report the problem to these mod's authors. Data from mod '{currentAger.ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            customAger.AgingData.ToList().ForEach(d =>
                            {
                                int? id = GetId(d.Key, objects);
                                if (id.HasValue) customAger.AgingDataId[id.Value] = d.Value;
                            });
                            AgerController.SetAger(customAger);
                        }
                        else
                        {
                            CustomCaskModEntry.ModMonitor.Log($"Cask data can't be added on {AgersDataJson} file. Use {CaskDataJson} file instead.", LogLevel.Warn);
                        }
                    }
                }

                if (!hasFile)
                {
                    CustomCaskModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt doesn't have both {CaskDataJson} and {AgersDataJson} files.", LogLevel.Warn);
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

        private static int? GetId(object identifier, Dictionary<int, string> objects)
        {
            if (Int32.TryParse(identifier.ToString(), out int id))
            {
                return id;
            }
            else
            {
                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(identifier + "/"));
                if (pair.Value != null)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.RegisterSimpleOption(manifest, "Disable Letter", "You won't receive the letter about Custom Cask changes and the cask recipe in case you don't know it.", () => DataLoader.ModConfig.DisableLetter, (bool val) => DataLoader.ModConfig.DisableLetter = val);

                api.RegisterSimpleOption(manifest, "Casks Anywhere", "Casks will accept items anywhere.", () => DataLoader.ModConfig.EnableCasksAnywhere, (bool val) => DataLoader.ModConfig.EnableCasksAnywhere = val);

                api.RegisterSimpleOption(manifest, "Quality++", "Casks will be able to increase more than one quality lever per day.", () => DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay, (bool val) => DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay = val);
            }
        }
    }
}