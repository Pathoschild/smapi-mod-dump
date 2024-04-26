/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jibblestein/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using StardewValley.Locations;
using IntegratedMinecarts.Patches;
using StardewValley.GameData.Minecarts;
using xTile;
using GenericModConfigMenu;
using ContentPatcher;

namespace IntegratedMinecarts
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal Harmony? Harmony;
        public ModConfig? Config;
        public static ModEntry Instance { get; private set; }
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Harmony = new Harmony(ModManifest.UniqueID);
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            MinecartWarpPatcher.Patch(this);
            MinecartMenuPatcher.Patch(this);
            checkActionPatcher.Patch(this);
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get ContentPatcher api
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            // add some config options
            CreateGMCMOptions(configMenu);
            api.RegisterToken(this.ModManifest, "PlayerName", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { Game1.player.Name };

                // or save is currently loading
                if (SaveGame.loaded?.player != null)
                    return new[] { SaveGame.loaded.player.Name };

                // no save loaded (e.g. on the title screen)
                return null;
            });

        }

        private void Player_Warped(object? sender, WarpedEventArgs e)
        {
            MinecartWarpPatcher.ResetlocationContexts(e.OldLocation, e.NewLocation);
        }
        private void CreateGMCMOptions(IGenericModConfigMenuApi configMenu)
        {
            // add some config options
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Destinations Per Page",
                getValue: () => this.Config.DestinationsPerPage,
                setValue: value => this.Config.DestinationsPerPage = value
            );
        }
    }
}