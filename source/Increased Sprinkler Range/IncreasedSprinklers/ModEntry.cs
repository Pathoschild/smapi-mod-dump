/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ArjanSeijs/SprinklerMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using IncreasedSprinklers.API;
using IncreasedSprinklers.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace IncreasedSprinklers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            Instance = this;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Config.RangeIncrease = Math.Clamp(Config.RangeIncrease, -256, 256);
            HarmonyPatch();
        }

        /// <summary>
        /// Applies the harmony patch for the base game and range highlight if it is installed
        /// </summary>
        private void HarmonyPatch()
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            SprinklerPatch.Initialize(Monitor);

            ApplyBaseGamePatch(harmony);
        }

        /// <summary>
        /// Base game harmony patch
        /// </summary>
        /// <param name="harmony"></param>
        private void ApplyBaseGamePatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object),
                    nameof(StardewValley.Object.GetModifiedRadiusForSprinkler)),
                postfix: new HarmonyMethod(typeof(SprinklerPatch),
                    nameof(SprinklerPatch.GetModifiedRadiusForSprinkler_Postfix))
            );
        }

        /// <summary>
        /// Registering the config menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Range",
                tooltip: () => "The Sprinkler Range Increase",
                getValue: () => Config.RangeIncrease,
                setValue: value => { Config.RangeIncrease = Math.Clamp(value,-256, 256); });
        }

    }
}