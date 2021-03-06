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
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;
using ModConfig = CustomCrystalariumMod.ModConfig;

namespace CustomCrystalariumMod
{
    internal class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        internal static Dictionary<int,int> CrystalariumDataId = new Dictionary<int, int>();

        public const string ClonersDataJson = "ClonersData.json";
        public const string CrystalariumDataJson = "data\\CrystalariumData.json";
        public static Dictionary<object, int> DefaultCystalariumData = new Dictionary<object, int>() { { 74, 20160 } };

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            Dictionary<object, int>  CrystalariumData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, int>>(CrystalariumDataJson) ?? DefaultCystalariumData;
            DataLoader.Helper.Data.WriteJsonFile(CrystalariumDataJson, CrystalariumData);

            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            CrystalariumData.ToList().ForEach(d =>
            {
                int? id = GetId(d.Key, objects);
                if (id.HasValue && !CrystalariumDataId.ContainsKey(id.Value)) CrystalariumDataId[id.Value] = d.Value;                
            });

            DataLoader.LoadContentPacksCommand();

            if (!ModConfig.DisableLetter)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCrystalarium"
                        , I18N.Get("CustomCrystalarium.Letter")
                        , (l) => !Game1.player.mailReceived.Contains(l.Id)
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                    {
                        Title = I18N.Get("CustomCrystalarium.Letter.Title")
                    }
                );
            }
            CreateConfigMenu(manifest);
        }

        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);            

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
                        if (cloner.Name == "Crystalarium")
                        {
                            cloner.CloningData.ToList().ForEach(d =>
                            {
                                int? id = GetId(d.Key, objects);
                                if (id.HasValue && !CrystalariumDataId.ContainsKey(id.Value))
                                {
                                    CrystalariumDataId[id.Value] = d.Value;
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Adding crystalarium data for item '{d.Key}' from mod '{contentPack.Manifest.UniqueID}'.", LogLevel.Trace);
                                }
                            });
                        }
                        else
                        {
                            cloner.ModUniqueID = contentPack.Manifest.UniqueID;
                            if (ClonerController.GetCloner(cloner.Name) is CustomCloner currentCloner)
                            {
                                if (currentCloner.ModUniqueID != cloner.ModUniqueID)
                                {
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Both mods '{currentCloner.ModUniqueID}' and '{cloner.ModUniqueID}' have data for  '{cloner.Name}'. You should report the problem to these mod's authors. Data from mod '{currentCloner.ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            cloner.CloningData.ToList().ForEach(d => 
                            {
                                int? id = GetId(d.Key, objects);
                                if (id.HasValue) cloner.CloningDataId[id.Value] = d.Value;
                            });
                            ClonerController.SetCloner(cloner);
                        }
                    }
                }
                else
                {
                    CustomCrystalariumModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an CaskData.json file.", LogLevel.Warn);
                }
            }
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

                api.RegisterSimpleOption(manifest, "Disable Letter", "You won't receive the letter about how the Crystalarium can clone Prismatic Shards and can be tuned to clone more stuff. Needs to restart.", () => DataLoader.ModConfig.DisableLetter, (bool val) => DataLoader.ModConfig.DisableLetter = val);

                api.RegisterLabel(manifest, "Get Object Back Properties:", "Properties that affect what happen to the object you place into cloners.");

                api.RegisterSimpleOption(manifest, "On Change", "Get the object back when changing the object being cloned or removing the cloner from the ground.", () => DataLoader.ModConfig.GetObjectBackOnChange, (bool val) => DataLoader.ModConfig.GetObjectBackOnChange = val);

                api.RegisterSimpleOption(manifest, "Immediately", "Get the object back immediately after placing it into the cloner. If set, the mod will ignore the 'On Change' property.", () => DataLoader.ModConfig.GetObjectBackImmediately, (bool val) => DataLoader.ModConfig.GetObjectBackImmediately = val);

                api.RegisterSimpleOption(manifest, "Override Cloner Config", "If checked the mod will use these 'get object back properties' instead of the ones defined for each cloner.", () => DataLoader.ModConfig.OverrideContentPackGetObjectProperties, (bool val) => DataLoader.ModConfig.OverrideContentPackGetObjectProperties = val);
            }

        }
    }
}
