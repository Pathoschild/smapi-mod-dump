/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using HarmonyLib;
//using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.GameData.Shops;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace ShopMenuFilter
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;

        public static bool accelerating;
        //private static Texture2D boardTexture;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            context = this;

            SMonitor = Monitor;
            SHelper = helper;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Constructor(typeof(ShopMenu), new Type[] { typeof(string), typeof(ShopData), typeof(ShopOwnerData), typeof(NPC), typeof(Func<ISalable, Farmer, int, bool>), typeof(Func<ISalable, bool>), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Shopmenu_Constructor_Postfix)));

            harmony.Patch(
                original: AccessTools.Constructor(typeof(ShopMenu), new Type[] { typeof(string), typeof(Dictionary<ISalable, ItemStockInformation>), typeof(int), typeof(string), typeof(Func<ISalable, Farmer, int, bool>), typeof(Func<ISalable, bool>), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Shopmenu_Constructor_Postfix)));

            harmony.Patch(
                original: AccessTools.Constructor(typeof(ShopMenu), new Type[] { typeof(string), typeof(List<ISalable>), typeof(int), typeof(string), typeof(Func<ISalable, Farmer, int, bool>), typeof(Func<ISalable, bool>), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Shopmenu_Constructor_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.applyTab)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(applyTab_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.gameWindowSizeChanged)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(gameWindowSizeChanged_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.drawCurrency)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(drawCurrency_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveLeftClick)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(receiveLeftClick_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveKeyPress)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(receiveKeyPress_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.performHoverAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(performHoverAction_Postfix)));

            //the auto patch method originally used did not work unless the project was compiled Debug.
            //manual patch here always works regardless.
            //harmony.PatchAll();
        }


        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => ModEntry.SHelper.Translation.Get("GMCM_Option_ModEnabled_Name"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
        }
    }
}