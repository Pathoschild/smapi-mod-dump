/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace BushBloomMod {
    public class Configuration {
        public bool EnableDefaultSchedules { get; set; } = true;
        public bool UseSpringBushForSummer { get; set; } = true;
        public bool UseCustomWinterBerrySprite { get; set; } = true;
        //public bool EnableALMIntegration { get; set; } = true;

        internal static Configuration Register(IModHelper helper) {
            var config = helper.ReadConfig<Configuration>();

            helper.Events.GameLoop.GameLaunched += (s, e) => {
                var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                var manifest = helper.ModRegistry.Get(helper.ModContent.ModID)!.Manifest;
                configMenu.Register(
                    mod: manifest,
                    reset: () => config = new Configuration(),
                    save: () => {
                        helper.WriteConfig(config);
                        Schedule.ReloadSchedules();
                    }
                );

                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Enable Default Blooming",
                    tooltip: () => "Enable the game's default spring and fall bush blooming schedules. Disabling them does not modify any other game logic, bundles, quests, or storytelling in the game. Do so at your own risk!",
                    getValue: () => config.EnableDefaultSchedules,
                    setValue: value => config.EnableDefaultSchedules = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Use Spring Bush for Summer",
                    tooltip: () => "The default summer blooming bush does not have berries. This reuses the spring bush to accommodate blooming in summer. Disable if you are using another mod that adds a summer berry bush sprite.",
                    getValue: () => config.UseSpringBushForSummer,
                    setValue: value => config.UseSpringBushForSummer = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Use Custom Winter Berry",
                    tooltip: () => "The default winter blooming bush does not exist. This uses a custom sprite to accommodate blooming in winter. Disable if you are using another mod that adds a winter blooming bush sprite.",
                    getValue: () => config.UseCustomWinterBerrySprite,
                    setValue: value => config.UseCustomWinterBerrySprite = value
                );
                //configMenu.AddBoolOption(
                //    mod: manifest,
                //    name: () => "Support Almanac",
                //    tooltip: () => "When using that mod, allows the almanac to show custom gathering schedules. When enabled, it's highly recommended to disable that mod's option 'Page: Local Notices' > 'Show Gathering' in order to not show duplicates of the default gathering schedules.",
                //    getValue: () => config.EnableALMIntegration,
                //    setValue: value => config.EnableALMIntegration = value
                //);
            };

            return config;
        }
    }

    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
