namespace BetterTrainLoot.Config
{
    public static class ModConfigDefaultConfig
    {
        private static readonly int currentVersion = 2;
        public static ModConfig CreateDefaultConfig(string file)
        {            
            ModConfig config = new ModConfig()
            {
                enableMod = true,
                baseChancePercent = 0.20, // Base chance of getting an item   
                useCustomTrainTreasure = true,
                enableNoLimitTreasurePerTrain = false,
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
                returnUpdatedConfig.maxTrainsPerDay = 5;                 // Version 2 config addition
                returnUpdatedConfig.configVersion = currentVersion;      // Version 2 config addition
                BetterTrainLootMod.Instance.Helper.Data.WriteJsonFile(file, returnUpdatedConfig);
            }

            return returnUpdatedConfig;
        }
    }
}
