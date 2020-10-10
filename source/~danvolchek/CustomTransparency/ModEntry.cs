/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System.Reflection;

namespace CustomTransparency
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance instance = HarmonyInstance.Create(this.Helper.ModRegistry.ModID);

            if (ValidateConfig(helper.ReadConfig<ModConfig>(), out Config))
            {
                helper.WriteConfig(Config);
            }

            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>Validate the given config, resulting in a new config with values changed to the default if invalid.</summary>
        private static bool ValidateConfig(ModConfig config, out ModConfig validatedConfig)
        {
            bool changed = false;

            validatedConfig = new ModConfig();

            if (config.MinimumBuildingTransparency >= 0 && config.MinimumBuildingTransparency <= 1)
                validatedConfig.MinimumBuildingTransparency = config.MinimumBuildingTransparency;
            else changed = true;

            if (config.MinimumTreeTransparency >= 0 && config.MinimumTreeTransparency <= 1)
                validatedConfig.MinimumTreeTransparency = config.MinimumTreeTransparency;
            else changed = true;

            return changed;
        }
    }
}
