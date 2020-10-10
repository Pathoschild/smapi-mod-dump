/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

namespace BetterTrainLoot.Config
{
    public static class ModConfigDefaultConfig
    {
        private static readonly int currentVersion = 4;
        public static ModConfig CreateDefaultConfig(string file)
        {            
            ModConfig config = new ModConfig()
            {
                enableMod = true,
                baseChancePercent = 0.20, // Base chance of getting an item   
                useCustomTrainTreasure = true,
                enableNoLimitTreasurePerTrain = false,
                showTrainIsComingMessage = true,
                showDesertTrainIsComingMessage = true,
                enableTrainWhistle = true,
                enableDesertTrainWhistle = true,
                basePctChanceOfTrain = 0.15,
                maxNumberOfItemsPerTrain = 5,
                trainCreateDelay = 10000,  //Base Game Setting
                enableForceCreateTrain = false,
                enableMultiplayerChatMessage = false,
                maxTrainsPerDay = 5,
                configVersion = currentVersion
            };

            BetterTrainLootMod.Instance.Helper.Data.WriteJsonFile(file, config);
            return config;
        }

        internal static ModConfig UpdateConfigToLatest(ModConfig oldConfig, string file)
        {
            ModConfig returnUpdatedConfig = oldConfig;

            if (oldConfig != null && oldConfig.configVersion != currentVersion)
            {
                if (oldConfig.configVersion < 2)
                {
                    returnUpdatedConfig.maxTrainsPerDay = 5;              // Version 2 config addition                          
                }

                if (oldConfig.configVersion < 3)
                {
                    returnUpdatedConfig.showTrainIsComingMessage = true;  // Version 3 config addition
                    returnUpdatedConfig.enableTrainWhistle = true;        // Version 3 config addition
                }

                if (oldConfig.configVersion < 4)
                {
                    returnUpdatedConfig.showDesertTrainIsComingMessage = true;  // Version 4 config addition
                    returnUpdatedConfig.enableDesertTrainWhistle = true;        // Version 4 config addition
                }
                returnUpdatedConfig.configVersion = currentVersion;
                BetterTrainLootMod.Instance.Helper.Data.WriteJsonFile(file, returnUpdatedConfig);
            }

            return returnUpdatedConfig;
        }
    }
}
