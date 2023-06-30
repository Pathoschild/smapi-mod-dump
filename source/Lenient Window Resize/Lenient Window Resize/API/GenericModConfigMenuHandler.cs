/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-lenient-window-resize
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

namespace Lenient_Window_Resize.API
{
    public class GenericModConfigMenuHandler
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
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
                name: () => "Minimum Window Width",
                getValue: () => Config.MinW,
                setValue: value => Config.MinW = value,
                interval: 10,
                min: 10,
                max: 3840
            );

            configMenu.AddNumberOption(
                mod: Mod.ModManifest,
                name: () => "Minimum Window Height",
                getValue: () => Config.MinH,
                setValue: value => Config.MinH = value,
                interval: 10,
                min: 10,
                max: 2160
            );
        }
    }
}
