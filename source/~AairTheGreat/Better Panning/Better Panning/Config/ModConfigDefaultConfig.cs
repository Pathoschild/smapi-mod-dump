using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterPanning.Config
{
    
    public static class ModConfigDefaultConfig
    {
        private static readonly int currentVersion = 3;
        public static ModConfig CreateDefaultConfig(string file)
        {
            ModConfig config = new ModConfig()
            {
                useCustomPanningTreasure = true,

                showHudData = true,
                hudXPostion = 0,
                hudYPostion = 0,
                showDistance = true,

                enableGeodeMineralsTreasure = true,
                enablePanningTrash = true,
                enableArtifactTreasures = true,
                enableAllArtifactsAfterFoundThemAll = true,
                enableSeedPanning = true,
                enableAllSeedsEverySeason = false,
                enableAllSecondYearSeedsOnFirstYear = false,
                enableSplashSounds = true,

                sp_alwaysCreatePanningSpots = true,
                mp_alwaysCreatePanningSpots = false,
                chanceOfCreatingPanningSpot = 1.0,
                maxNumberOfOrePointsGathered = 50,
                
                additionalLootChance = 0.4,
                useCustomFarmMaps = false,
                customMaps = new Dictionary<int, string>(),
                configVersion = currentVersion
            };
           
            PanningMod.Instance.Helper.Data.WriteJsonFile(file, config);
            return config;
        }

        internal static ModConfig UpdateConfigToLatest(ModConfig oldConfig, string file)
        {
            ModConfig returnUpdatedConfig = oldConfig;

            if (oldConfig != null && oldConfig.configVersion != currentVersion)
            {
                if (oldConfig.configVersion != 2)
                {
                    returnUpdatedConfig.enableSplashSounds = true;           // Version 2 config addition                 
                }
                if (oldConfig.configVersion != 3)
                {
                    returnUpdatedConfig.chanceOfCreatingPanningSpot = 1.0;   // Version 3 config addition
                }
                returnUpdatedConfig.configVersion = currentVersion;      
                PanningMod.Instance.Helper.Data.WriteJsonFile(file, returnUpdatedConfig);
            }
            
            return returnUpdatedConfig;
        }
    }
}
