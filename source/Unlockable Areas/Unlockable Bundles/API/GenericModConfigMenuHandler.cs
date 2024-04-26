/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;


namespace Unlockable_Bundles.API
{
    public class GenericModConfigMenuHandler
    {

        public static void Initialize()
        {
            Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(ModEntry.Config)
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Scroll Delay",
                getValue: () => Config.ScrollDelay,
                setValue: value => Config.ScrollDelay = value,
                interval: 1,
                min: 5,
                max: 60
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Max Cost Name Characters",
                getValue: () => Config.ScrollCharacterLength,
                setValue: value => Config.ScrollCharacterLength = value,
                interval: 1,
                min: 5,
                max: 25
            );
        }
    }
}
