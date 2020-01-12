using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace CustomKissingMod
{
    public class DataLoader
    {
        internal const string NpcJson = "npc.json";
        public static IModHelper Helper;
        public static ModConfig ModConfig;
        public static readonly ModConfig DefaultConfig = new ModConfig();

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
        }

        public void LoadContentPacks()
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                if (contentPack.HasFile(NpcJson))
                {
                    CustomKissingModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<NpcConfig> npcs = contentPack.ReadJsonFile<List<NpcConfig>>(NpcJson);

                    foreach (NpcConfig npc in npcs)
                    {
                        NpcConfig existingNpc = ModConfig.NpcConfigs.Find(n => n.Name == npc.Name);
                        if (existingNpc != null)
                        {
                            if (ModConfig.EnableContentPacksOverrides)
                            {
                                CustomKissingModEntry.ModMonitor.Log($"Content pack '{contentPack.Manifest.Name}' is replacing the config for npc '{npc.Name}'", LogLevel.Info);
                                ModConfig.NpcConfigs.Remove(existingNpc);
                                ModConfig.NpcConfigs.Add(npc);
                            }
                            else
                            {
                                CustomKissingModEntry.ModMonitor.Log($"Content pack '{contentPack.Manifest.Name}' is trying to add a new config for npc '{npc.Name}'. You should enable content pack overrides or resolve the conflict.\nThe new config will be ignored.", LogLevel.Warn);
                            }
                        }
                        else
                        {
                            CustomKissingModEntry.ModMonitor.Log($"Content pack '{contentPack.Manifest.Name}' is adding a config for npc '{npc.Name}'",LogLevel.Debug);
                            ModConfig.NpcConfigs.Add(npc);
                        }
                    }
                }
                else
                {
                    CustomKissingModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {NpcJson} file.", LogLevel.Warn);
                }
            }
        }
    }
}
