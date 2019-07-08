using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterPanning.Config
{
    public static class ModConfigDefaultConfig
    {
        public static ModConfig CreateDefaultConfig(string file)
        {
            ModConfig config = new ModConfig()
            {
                showDistance = true,
                sp_alwaysCreatePanningSpots = true,
                mp_alwaysCreatePanningSpots = false,
                maxNumberOfOrePointsGathered = 50,
                showHudData = true,
                hudXPostion = 0,
                hudYPostion = 200,
                additionalLootChance = 0.4,
                useCustomPanningTreasure = true,
                enableGeodeMineralsTreasure = true,
                enablePanningTrash = true,
                enableArtifactTreasures = true,
                enableAllArtifactsAfterFoundThemAll = true,
                enableSeedPanning = true,
                enableAllSeedsEverySeason = false,
                enableAllSecondYearSeedsOnFirstYear = false,
                useCustomFarmMaps = false,
                customMaps = new Dictionary<int, string>() 
            };

            //config.customMaps.Add(0, "Immersive_Farm_2.json");

            PanningMod.Instance.Helper.Data.WriteJsonFile(file, config);
            return config;
        }
    }
}
