/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

namespace FishFinder.Config
{
    public static class ModConfigDefaultConfig
    {
        private static readonly int currentVersion = 1;
        public static ModConfig CreateDefaultConfig(string file)
        {
            ModConfig config = new ModConfig()
            {
                showHudData = true,
                hudXPostion = 0,
                hudYPostion = 0,
                showDistance = true,
                configVersion = currentVersion
            };

            FishFinderMod.Instance.Helper.Data.WriteJsonFile(file, config);
            return config;
        }

        internal static ModConfig UpdateConfigToLatest(ModConfig oldConfig, string file)
        {
            ModConfig returnUpdatedConfig = oldConfig;

            if (oldConfig != null && oldConfig.configVersion != currentVersion)
            {
                // This is for future changes (if needed)
                returnUpdatedConfig.configVersion = currentVersion;
                FishFinderMod.Instance.Helper.Data.WriteJsonFile(file, returnUpdatedConfig);
            }
            
            return returnUpdatedConfig;
        }
    }
}
