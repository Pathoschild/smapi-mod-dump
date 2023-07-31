/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
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

namespace Custom_Farm_Loader.API
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
                name: () => "Load Menu Icon Scale",
                getValue: () => Config.LoadMenuIconScale,
                setValue: value => Config.LoadMenuIconScale = value,
                interval: 0.05f,
                min: 0.75f,
                max: 1.25f
            );

            configMenu.AddNumberOption(
                mod: Mod.ModManifest,
                name: () => "Co-Op Menu Icon Scale",
                getValue: () => Config.CoopMenuIconScale,
                setValue: value => Config.CoopMenuIconScale = value,
                interval: 0.05f,
                min: 0.75f,
                max: 1.25f
            );

            configMenu.AddBoolOption(
                mod: Mod.ModManifest,
                name: () => "Incude Vanilla Farms in Selection",
                getValue: () => Config.IncludeVanilla,
                setValue: value => Config.IncludeVanilla = value
                );

            configMenu.AddBoolOption(
                mod: Mod.ModManifest,
                name: () => "Disable FarmHouse StartFurniture",
                getValue: () => Config.DisableStartFurniture,
                setValue: value => Config.DisableStartFurniture = value
                );
        }
    }
}
