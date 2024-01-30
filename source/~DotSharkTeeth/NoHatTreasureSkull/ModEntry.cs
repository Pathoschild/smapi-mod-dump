/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using HarmonyLib;

namespace NoHatTreasureSkull
{
    public partial class ModEntry : Mod
    {
        public static ModConfig Config;
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public override void Entry(IModHelper helper)
        {
            // Skull cave
            //Game1.enterMine(121);
            Config = Helper.ReadConfig<ModConfig>();
            SMonitor = Monitor;
            SHelper = Helper;
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.getTreasureRoomItem)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MineShaftGetTreasureRoomItem_postfix))
            );

            // Cheat to spawn chest every room
            //harmony.Patch(
            //   original: AccessTools.Method(typeof(MineShaft), "addLevelChests"),
            //   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MineShaftaddLevelChests_prefix))
            //);
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable",
                tooltip: () => "Enable mod",
                getValue: () => Config.Enable,
                setValue: value => Config.Enable = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Machine",
                tooltip: () => "Crystalarium, Seed Maker, Auto Grabber, Auto Petter",
                getValue: () => Config.EnableMachine,
                setValue: value => Config.EnableMachine = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Bomb",
                tooltip: () => "Cherry Bomb, Bomb, Mega Bomb",
                getValue: () => Config.EnableBomb,
                setValue: value => Config.EnableBomb = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Seeds",
                tooltip: () => "Random Seed",
                getValue: () => Config.EnableSeed,
                setValue: value => Config.EnableSeed = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Medicine",
                tooltip: () => "Life Elixir, Energy Tonic",
                getValue: () => Config.EnableMedicine,
                setValue: value => Config.EnableMedicine = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Sapling",
                tooltip: () => "Random Sapling",
                getValue: () => Config.EnableSapling,
                setValue: value => Config.EnableSapling = value
            );
        }
    }
}
