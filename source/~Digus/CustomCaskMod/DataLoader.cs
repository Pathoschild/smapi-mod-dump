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
            Dictionary<int, string> objects = DataLoader.Helper.GameContent.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            AgerController.ClearAgers();
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
                            if (AgerController.GetAger(customAger.Name) is CustomAger oldAger && oldAger.ModUniqueID != customAger.ModUniqueID)
                            {
                                if (oldAger.OverrideMod.Contains(customAger.ModUniqueID) && customAger.OverrideMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Both mods '{oldAger.ModUniqueID}' and '{customAger.ModUniqueID}' are saying they should override data for  '{customAger.Name}'. You should report the problem to these mod's authors. Data from mod '{oldAger.ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                } 
                                else if (customAger.OverrideMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' is overriding mod '{oldAger.ModUniqueID}' data for ager '{customAger.Name}'." , LogLevel.Debug);
                                }
                                else if (oldAger.OverrideMod.Contains(customAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{oldAger.ModUniqueID}' is overriding mod '{customAger.ModUniqueID}' data for ager '{oldAger.Name}'." , LogLevel.Debug);
                                    continue;
                                }
                                else if (customAger.MergeIntoMod.Contains(oldAger.ModUniqueID))
                                {
                                    if (oldAger.MergeIntoMod.Contains(customAger.ModUniqueID))
                                    {
                                        CustomCaskModEntry.ModMonitor.Log($"Both mods '{oldAger.ModUniqueID}' and '{customAger.ModUniqueID}' are saying they should merge data for  '{customAger.Name}'. You should report the problem to these mod's authors. Data from mod '{customAger.ModUniqueID}' will be merge into '{oldAger.ModUniqueID}'.", LogLevel.Warn);
                                    }
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' is merging with mod '{oldAger.ModUniqueID}' data for ager '{customAger.Name}'.", LogLevel.Debug);
                                    foreach (var (key, value) in customAger.AgingData)
                                    {
                                        oldAger.AgingData[key] = value;
                                    }
                                    FillDataIds(objects, oldAger.AgingData, oldAger.AgingDataId);
                                    continue;

                                }
                                else if (oldAger.MergeIntoMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{oldAger.ModUniqueID}' is merging with mod '{customAger.ModUniqueID}' data for ager '{customAger.Name}'.", LogLevel.Debug);
                                    foreach (var (key, value) in oldAger.AgingData)
                                    {
                                        customAger.AgingData[key] = value;
                                    }
                                }
                                else
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' can't override mod '{oldAger.ModUniqueID}' data for '{customAger.Name}'. This data will be ignored.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            FillDataIds(objects, customAger.AgingData, customAger.AgingDataId);
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
            FillDataIds(objects, CaskData, CaskDataId);
        }

        public static void FillDataIds(Dictionary<int, string> objects, Dictionary<object, float> data, Dictionary<int, float> dataIds)
        {
            data.ToList().ForEach(c =>
            {
                var (key, value) = c;
                int? id = GetId(key, objects);
                if (id.HasValue)
                {
                    dataIds[id.Value] = value;
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