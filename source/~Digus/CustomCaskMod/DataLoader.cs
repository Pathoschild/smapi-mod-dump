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
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.GameData.BigCraftables;
using System.Security.AccessControl;

namespace CustomCaskMod
{
    internal class DataLoader
    {
        private const string CaskDataJson = "CaskData.json";
        private const string AgersDataJson = "AgersData.json";
        private const string VanillaCaskName = "Cask";
        private const string VanillaCaskQualifiedItemId = "(BC)163";

        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static Dictionary<object, float> CaskData;
        public static Dictionary<string, float> CaskDataId =  new();

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CaskData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, float>>("data\\CaskData.json") ?? new Dictionary<object, float>() { { 342, 2.66f }, { 724, 2f } };
            DataLoader.Helper.Data.WriteJsonFile("data\\CaskData.json", CaskData);

            IMailFrameworkModApi mailFrameworkModApi = helper.ModRegistry.GetApi<IMailFrameworkModApi>("DIGUS.MailFrameworkMod");
            mailFrameworkModApi?.RegisterLetter(
                new ApiLetter
                {
                    Id = "CustomCaskRecipe", Text = "CustomCask.RecipeLetter",
                    Title = "CustomCask.RecipeLetter.Title", Recipe = VanillaCaskName, I18N = helper.Translation
                }
                , (l) => !DataLoader.ModConfig.DisableLetter && !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && !Game1.player.craftingRecipes.ContainsKey(VanillaCaskName)
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );
            mailFrameworkModApi?.RegisterLetter(
                new ApiLetter
                {
                    Id = "CustomCask"
                    , Text = "CustomCask.Letter"
                    , Title = "CustomCask.Letter.Title"
                    , I18N = helper.Translation
                }
                , (l) => !DataLoader.ModConfig.DisableLetter && !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCaskRecipe") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && Game1.player.craftingRecipes.ContainsKey(VanillaCaskName)
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );

            Helper.Events.Content.AssetRequested += this.OnAssetRequested;

            CreateConfigMenu(manifest);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(assetData =>
                {
                    var machines = assetData.AsDictionary<String, MachineData>();
                    var machineOutputRules = machines.Data[VanillaCaskQualifiedItemId].OutputRules;
                    foreach (var (id, multiplier) in CaskDataId.OrderByDescending(p=>p.Key))
                    {
                        MachineOutputRule machineOutputRule = null;
                        machineOutputRule = ItemRegistry.Exists(id) ? CreateMachineOutputRule(ItemRegistry.Create(id), multiplier) : CreateMachineOutputRule(id, multiplier);
                        machineOutputRules.RemoveAll(r => r.Id.Equals(machineOutputRule.Id));
                        machineOutputRules.Insert(0, machineOutputRule);
                    }

                    machineOutputRules.RemoveAll(r => r.Id.Equals(GetMachineRuleId(VanillaCaskName)));
                    if (DataLoader.ModConfig.EnableCaskAgeEveryObject)
                    {
                        MachineOutputRule machineOutputRule = CreateMachineDefaultOutputRule(VanillaCaskName, DataLoader.ModConfig.DefaultAgingRate);
                        machineOutputRules.Add(machineOutputRule);
                    }

                    foreach (var customAger in AgerController.GetAgers())
                    {
                        
                        var bigCraftables = StardewValley.DataLoader.BigCraftables(Game1.content);
                        var bigCraftableId = customAger.Key;
                        if (bigCraftableId != null)
                        {
                            machines.Data.TryGetValue(bigCraftableId, out MachineData machineData);
                            if (machineData == null)
                            {
                                machineData = new MachineData()
                                {
                                    OutputRules = new List<MachineOutputRule>()
                                };
                                machines.Data.Add(bigCraftableId, machineData);
                                machineOutputRules = machineData.OutputRules;
                            }

                            foreach (var (id, multiplier) in customAger.Value.AgingDataId)
                            {
                                var machineOutputRule = ItemRegistry.Exists(id) ? CreateMachineOutputRule(ItemRegistry.Create(id), multiplier) : CreateMachineOutputRule(id, multiplier);
                                machineOutputRules.RemoveAll(r => r.Id.Equals(machineOutputRule.Id));
                                machineOutputRules.Insert(0, machineOutputRule);
                            }

                            machineOutputRules.RemoveAll(r => r.Id.Equals(GetMachineRuleId(customAger.Value.Name)));
                            if (customAger.Value.EnableAgeEveryObject)
                            {
                                MachineOutputRule machineOutputRule = CreateMachineDefaultOutputRule(customAger.Key, customAger.Value.DefaultAgingRate);
                                machineOutputRules.Add(machineOutputRule);
                            }
                        }
                    }
                });
            }
        }

        private static string GetMachineRuleId(string agerName)
        {
            return $"Digus.CustomCaskMod.{agerName}.EnableAgeEveryObject";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060", Justification = "Needed for command methods.")]
        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
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
                        if (customAger.Name != VanillaCaskName && customAger.QualifiedItemId != VanillaCaskQualifiedItemId)
                        {
                            customAger.ModUniqueID = contentPack.Manifest.UniqueID;
                            if (customAger.QualifiedItemId == null)
                            {
                                var foundAgers = Game1.bigCraftableData.Where(b => b.Value.Name.Equals(customAger.Name));
                                if (!foundAgers.Any())
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"There is no ager with the name '{customAger.Name}'. This data will be ignored.", LogLevel.Warn);
                                    continue;
                                }
                                else
                                {
                                    customAger.QualifiedItemId = ItemRegistry.type_bigCraftable + foundAgers.First().Key;
                                    if (foundAgers.Count() > 1)
                                    {
                                        CustomCaskModEntry.ModMonitor.Log($"There is more than one ager with the name '{customAger.Name}'. Ager of qualified item id '{customAger.QualifiedItemId}' will be used.", LogLevel.Warn);
                                    }
                                }
                            }
                            if (AgerController.GetAger(customAger.QualifiedItemId) is CustomAger oldAger && oldAger.ModUniqueID != customAger.ModUniqueID)
                            {
                                if (oldAger.OverrideMod.Contains(customAger.ModUniqueID) && customAger.OverrideMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Both mods '{oldAger.ModUniqueID}' and '{customAger.ModUniqueID}' are saying they should override data for  '{customAger.QualifiedItemId}'. You should report the problem to these mod's authors. Data from mod '{oldAger.ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                } 
                                else if (customAger.OverrideMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' is overriding mod '{oldAger.ModUniqueID}' data for ager '{customAger.QualifiedItemId}'." , LogLevel.Debug);
                                }
                                else if (oldAger.OverrideMod.Contains(customAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{oldAger.ModUniqueID}' is overriding mod '{customAger.ModUniqueID}' data for ager '{oldAger.QualifiedItemId}'." , LogLevel.Debug);
                                    continue;
                                }
                                else if (customAger.MergeIntoMod.Contains(oldAger.ModUniqueID))
                                {
                                    if (oldAger.MergeIntoMod.Contains(customAger.ModUniqueID))
                                    {
                                        CustomCaskModEntry.ModMonitor.Log($"Both mods '{oldAger.ModUniqueID}' and '{customAger.ModUniqueID}' are saying they should merge data for  '{customAger.QualifiedItemId}'. You should report the problem to these mod's authors. Data from mod '{customAger.ModUniqueID}' will be merge into '{oldAger.ModUniqueID}'.", LogLevel.Warn);
                                    }
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' is merging with mod '{oldAger.ModUniqueID}' data for ager '{customAger.QualifiedItemId}'.", LogLevel.Debug);
                                    foreach (var (key, value) in customAger.AgingData)
                                    {
                                        oldAger.AgingData[key] = value;
                                    }
                                    FillDataIds(oldAger.AgingData, oldAger.AgingDataId);
                                    continue;

                                }
                                else if (oldAger.MergeIntoMod.Contains(oldAger.ModUniqueID))
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{oldAger.ModUniqueID}' is merging with mod '{customAger.ModUniqueID}' data for ager '{customAger.QualifiedItemId}'.", LogLevel.Debug);
                                    foreach (var (key, value) in oldAger.AgingData)
                                    {
                                        customAger.AgingData[key] = value;
                                    }
                                }
                                else
                                {
                                    CustomCaskModEntry.ModMonitor.Log($"Mod '{customAger.ModUniqueID}' can't override mod '{oldAger.ModUniqueID}' data for '{customAger.QualifiedItemId}'. This data will be ignored.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            FillDataIds(customAger.AgingData, customAger.AgingDataId);
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
            FillDataIds(CaskData, CaskDataId);
            CustomCaskModEntry.Helper.GameContent.InvalidateCache("Data/Machines");
        }

        public static void FillDataIds(Dictionary<object, float> data, Dictionary<string, float> dataIds)
        {
            data.ToList().ForEach(c =>
            {
                var (key, value) = c;
                var id = GetId(key);
                if (id != null)
                {
                    dataIds[id] = value;
                }
            });
        }

        private static string GetId(object identifier)
        {
            if (ItemRegistry.IsQualifiedItemId(identifier.ToString()))
            {
                return identifier.ToString();
            }
            if (Int32.TryParse(identifier.ToString(), out int id))
            {
                return id >= 0 ? ItemRegistry.QualifyItemId(id.ToString()) : identifier.ToString();
            }
            else
            {
                var pair = Game1.objectData.FirstOrDefault(o => o.Value.Name.Equals(identifier));
                if (pair.Value != null)
                {
                    return ItemRegistry.QualifyItemId(pair.Key);
                }
            }
            return null;
        }

        private static MachineOutputRule CreateMachineDefaultOutputRule(string name, float agingRate)
        {
            return new MachineOutputRule
            {
                Id = GetMachineRuleId(name),
                Triggers = new List<MachineOutputTriggerRule>
                {
                    new MachineOutputTriggerRule()
                },
                OutputItem = new List<MachineItemOutput>
                {
                    new MachineItemOutput
                    {
                        CustomData = new Dictionary<string, string> { { "AgingMultiplier", agingRate.ToString() } },
                        OutputMethod = "StardewValley.Objects.Cask, Stardew Valley: OutputCask"
                    }
                }
            };
        }

        private static MachineOutputRule CreateMachineOutputRule(Item itemInput, float agingMultiplier)
        {
            var result = new MachineOutputRule
            {
                Id = "Digus.CustomCaskMod." + itemInput.Name,
                Triggers = new List<MachineOutputTriggerRule>
                {
                    new MachineOutputTriggerRule()
                    {
                        RequiredItemId = itemInput.QualifiedItemId
                    }
                },
                OutputItem = new List<MachineItemOutput>
                {
                    new MachineItemOutput
                    {
                        CustomData = new Dictionary<string, string> { { "AgingMultiplier", agingMultiplier.ToString() } },
                        OutputMethod = "StardewValley.Objects.Cask, Stardew Valley: OutputCask"
                    }
                }
            };
            return result;
        }

        private static MachineOutputRule CreateMachineOutputRule(string category, float agingMultiplier)
        {
            var result = new MachineOutputRule
            {
                Id = "Digus.CustomCaskMod.Category" + category,
                Triggers = new List<MachineOutputTriggerRule>
                {
                    new MachineOutputTriggerRule()
                    {
                        Condition = "ITEM_CATEGORY INPUT "+category
                    }
                },
                OutputItem = new List<MachineItemOutput>
                {
                    new MachineItemOutput
                    {
                        CustomData = new Dictionary<string, string> { { "AgingMultiplier", agingMultiplier.ToString() } },
                        OutputMethod = "StardewValley.Objects.Cask, Stardew Valley: OutputCask"
                    }
                }
            };
            return result;
        }

        private static void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.Register(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableLetter, (bool val) => DataLoader.ModConfig.DisableLetter = val, ()=>"Disable Letter", ()=>"You won't receive the letter about Custom Cask changes and the cask recipe in case you don't know it.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableCasksAnywhere, (bool val) => DataLoader.ModConfig.EnableCasksAnywhere = val, () => "Casks Anywhere", () => "Casks will accept items anywhere.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay, (bool val) => DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay = val, () => "Quality++", () => "Casks will be able to increase more than one quality lever per day.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableCaskAgeEveryObject, (bool val) => DataLoader.ModConfig.EnableCaskAgeEveryObject = val, () => "Cask Age Every Object", () => "Casks will be able to age every object.");

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.DefaultAgingRate, (float val) => DataLoader.ModConfig.DefaultAgingRate = val, () => "Default Aging Rate", () => "Rate that will be used for non declared objects.");
            }
        }
    }
}