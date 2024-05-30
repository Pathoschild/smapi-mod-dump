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
using System.Text;
using System.Threading.Tasks;
using CustomCrystalariumMod.integrations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Objects;
using ModConfig = CustomCrystalariumMod.ModConfig;

namespace CustomCrystalariumMod
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        internal static Dictionary<string,int> CrystalariumDataId = new Dictionary<string, int>();
        internal static Dictionary<object,int> CrystalariumData = new Dictionary<object, int>();


        public const string ClonersDataJson = "ClonersData.json";
        public const string CrystalariumDataJson = "data/CrystalariumData.json";
        public const string VanillaClonerName = "Crystalarium";
        public const string VanillaClonerQualifiedItemId = "(BC)21";
        public static Dictionary<object, int> DefaultCystalariumData = new Dictionary<object, int>() { { 74, 20160 } };

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CrystalariumData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, int>>(CrystalariumDataJson) ?? DefaultCystalariumData;
            DataLoader.Helper.Data.WriteJsonFile(CrystalariumDataJson, CrystalariumData);

            IMailFrameworkModApi mailFrameworkModApi = helper.ModRegistry.GetApi<IMailFrameworkModApi>("DIGUS.MailFrameworkMod");
            mailFrameworkModApi?.RegisterLetter(
                new ApiLetter
                {
                    Id = "CustomCrystalarium"
                    , Text = "CustomCrystalarium.Letter"
                    , Title = "CustomCrystalarium.Letter.Title"
                    , I18N = helper.Translation
                }
                , (l) => !ModConfig.DisableLetter && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.stats.PrismaticShardsFound > 0
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );

            CreateConfigMenu(manifest);
        }

        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, ClonersDataJson)))
                {
                    CustomCrystalariumModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<CustomCloner> clonerData = contentPack.ReadJsonFile<List<CustomCloner>>(ClonersDataJson);
                    foreach (var cloner in clonerData)
                    {
                        if (String.IsNullOrEmpty(cloner.Name))
                        {
                            CustomCrystalariumModEntry.ModMonitor.Log($"The cloner name property can't be null or empty. This cloner will be ignored.", LogLevel.Warn);
                        }
                        if (cloner.Name == VanillaClonerName || cloner.QualifiedItemId == VanillaClonerQualifiedItemId)
                        {
                            cloner.CloningData.ToList().ForEach(d =>
                            {
                                var (key, value) = d;
                                var id = GetId(key);
                                if (id != null && !CrystalariumDataId.ContainsKey(id))
                                {
                                    CrystalariumData[key] = value;
                                    CrystalariumDataId[id] = value;
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Adding crystalarium data for item '{d.Key}' from mod '{contentPack.Manifest.UniqueID}'.", LogLevel.Trace);
                                }
                            });
                        }
                        else
                        {
                            cloner.ModUniqueID = contentPack.Manifest.UniqueID;
                            if (cloner.QualifiedItemId == null)
                            {
                                var foundCloner = Game1.bigCraftableData.Where(b => b.Value.Name.Equals(cloner.Name));
                                if (!foundCloner.Any())
                                {
                                    CustomCrystalariumModEntry.ModMonitor.Log($"There is no cloner with the name '{cloner.Name}'. This data will be ignored.", LogLevel.Warn);
                                    continue;
                                }
                                else
                                {
                                    cloner.QualifiedItemId = ItemRegistry.type_bigCraftable + foundCloner.First().Key;
                                    if (foundCloner.Count() > 1)
                                    {
                                        CustomCrystalariumModEntry.ModMonitor.Log($"There is more than one big craftable with the name '{cloner.Name}'. Cloner of qualified item id '{cloner.QualifiedItemId}' will be used.", LogLevel.Warn);
                                    }
                                }
                            }
                            if (ClonerController.GetCloner(cloner.QualifiedItemId) is { } currentCloner)
                            {
                                if (currentCloner.ModUniqueID != cloner.ModUniqueID)
                                {
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Both mods '{currentCloner.ModUniqueID}' and '{cloner.ModUniqueID}' have data for  '{cloner.Name??cloner.QualifiedItemId}'. You should report the problem to these mod's authors. Data from mod '{currentCloner.ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            cloner.CloningData.ToList().ForEach(d => 
                            {
                                var (key, value) = d;
                                var id = GetId(key);
                                if (id != null) cloner.CloningDataId[id] = value;
                            });
                            ClonerController.SetCloner(cloner);
                        }
                    }
                }
                else
                {
                    CustomCrystalariumModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ClonersDataJson} file.", LogLevel.Warn);
                }
            }
        }

        internal static void LoadCrystalariumDataIds()
        {
            CrystalariumData.ToList().ForEach(d =>
            {
                var (key, value) = d;
                var id = GetId(key);
                if (id != null)
                {
                    CrystalariumDataId[id] = value;
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
                var pair = Game1.objectData.FirstOrDefault(o => identifier.Equals(o.Value.Name));
                if (pair.Value != null)
                {
                    return ItemRegistry.QualifyItemId(pair.Key);
                }
            }
            return null;
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.Register(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableLetter, (bool val) => DataLoader.ModConfig.DisableLetter = val, () => "Disable Letter", () => "You won't receive the letter about how the Crystalarium can clone Prismatic Shards and can be tuned to clone more stuff. Needs to restart.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableCrystalariumCloneEveryObject, (bool val) => DataLoader.ModConfig.EnableCrystalariumCloneEveryObject = val, () => "Clone Every Object", () => "Crystalarium will be able to clone every object.");

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.DefaultCloningTime, (int val) => DataLoader.ModConfig.DefaultCloningTime = val, () => "Default Cloning Time", () => "Cloning time in minutes that will be used for non declared objects.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.OverrideContentPackGetObjectProperties, (bool val) => DataLoader.ModConfig.OverrideContentPackGetObjectProperties = val, () => "Override Cloner Config", () => "If checked the mod will use the below properties instead of the ones defined for each cloner.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.KeepQuality, (bool val) => DataLoader.ModConfig.KeepQuality = val, () => "Keep Quality", () => "If checked the mod will keep the quality of items placed in the crystalarium.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.BlockChange, (bool val) => DataLoader.ModConfig.BlockChange = val, () => "Block Change", () => "You won't be able to change the object inside. You will need to remove the cloner from the ground.");

                api.AddSectionTitle(manifest, () => "Get Object Back Properties:", () => "Properties that affect what happen to the object you place into cloners.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.GetObjectBackOnChange, (bool val) => DataLoader.ModConfig.GetObjectBackOnChange = val, () => "On Change", () => "Get the object back when changing the object being cloned or removing the cloner from the ground.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.GetObjectBackImmediately, (bool val) => DataLoader.ModConfig.GetObjectBackImmediately = val, () => "Immediately", () => "Get the object back immediately after placing it into the cloner. If set, the mod will ignore the 'On Change' property.");
            }

        }
    }
}
