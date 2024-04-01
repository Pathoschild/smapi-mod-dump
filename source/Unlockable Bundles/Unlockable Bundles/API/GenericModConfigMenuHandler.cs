/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
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

namespace Unlockable_Bundles.API
{
    public class GenericModConfigMenuHandler
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
            Config = ModEntry.Config;

            Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: Mod.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(ModEntry.Config)
            );

            configMenu.AddNumberOption(
                mod: Mod.ModManifest,
                name: () => "Scroll Delay",
                getValue: () => Config.ScrollDelay,
                setValue: value => Config.ScrollDelay = value,
                interval: 1,
                min: 5,
                max: 60
            );

            configMenu.AddNumberOption(
                mod: Mod.ModManifest,
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
