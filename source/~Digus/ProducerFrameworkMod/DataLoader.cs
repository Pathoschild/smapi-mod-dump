using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;

namespace ProducerFrameworkMod
{
    internal class DataLoader
    {
        public const string ProducerRulesJson = "ProducerRules.json";
        public const string ProducersConfigJson = "ProducersConfig.json";
        public static IModHelper Helper;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
        }

        public static void LoadContentPacks(object sender, EventArgs e)
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                bool haveProducerRulesFile = File.Exists(Path.Combine(contentPack.DirectoryPath, ProducerRulesJson));
                if (haveProducerRulesFile)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<ProducerRule> producerItems = contentPack.ReadJsonFile<List<ProducerRule>>(ProducerRulesJson);
                    ProducerController.AddProducerItems(producerItems,contentPack.Translation);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ProducerRulesJson} file.", LogLevel.Trace);
                }

                bool haveProducersConfigFile = File.Exists(Path.Combine(contentPack.DirectoryPath, ProducersConfigJson));
                if (haveProducersConfigFile)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<ProducerConfig> producersConfigs = contentPack.ReadJsonFile<List<ProducerConfig>>(ProducersConfigJson);
                    ProducerController.AddProducersConfig(producersConfigs);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ProducersConfigJson} file.", LogLevel.Trace);
                }

                if (!haveProducerRulesFile && !haveProducersConfigFile)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have any of the required files.", LogLevel.Warn);
                }
            }
        }
    }
}
