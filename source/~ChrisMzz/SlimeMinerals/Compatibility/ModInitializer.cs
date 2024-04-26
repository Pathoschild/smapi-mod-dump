/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;

namespace SlimeMinerals.Compatibility
{
    public class ModInitializer
    {
        private readonly IManifest modManifest;
        private readonly IModHelper helper;

        public ModInitializer(IManifest modManifest, IModHelper helper)
        {
            this.modManifest = modManifest;
            this.helper = helper;
        }

        public void Initialize(IGenericModConfigMenuApi api, ModConfig config)
        {
            api.Register(
                mod: modManifest,
                reset: () =>
                {
                    config = new ModConfig();
                    ModEntry.Config = config;
                },
                save: () => helper.WriteConfig(config)
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.InSlimeHutch,
                setValue: value => config.InSlimeHutch = value,
                name: () => "Activate in Slime Hutch",
                tooltip: () => "Check this to activate the mod in Slime Hutches."
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.InWoods,
                setValue: value => config.InWoods = value,
                name: () => "Activate in Woods",
                tooltip: () => "Check this to activate the mod in the Woods."
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.Anywhere,
                setValue: value => config.Anywhere = value,
                name: () => "Activate anywhere",
                tooltip: () => "Check this to activate the mod in any location of the game. Overrides other options."
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.Range,
                setValue: value => config.Range = value,
                name: () => "Target Difficulty",
                tooltip: () => "How far from target color the Slime can be. Set this value to be low for maximum difficulty.",
                min: 15,
                max: 25,
                interval: 1
            );

        }
    }
}
