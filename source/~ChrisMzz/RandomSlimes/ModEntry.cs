/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StardewValley;
using GenericModConfigMenu;
using RandomSlimes.Compatibility;
using StardewValley.Locations;
using System.Drawing;

namespace RandomSlimes
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private static readonly Random rng = new Random();

        public static ModConfig Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            helper.Events.Player.Warped += this.OnSlimeAdded; // display name of location if location has a slime
            //helper.Events.GameLoop.DayStarted += this.RandomizeGeneratedSlimes;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            Config = helper.ReadConfig<ModConfig>();

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Initialize mod(s)
            ModInitializer modInitializer = new ModInitializer(ModManifest, Helper);
            // Get Generic Mod Config Menu API (if it's installed) thanks to ConvenientInventory as a template for this
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                modInitializer.Initialize(api, Config);
            }
        }



        private void OnSlimeAdded(object sender, WarpedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or if the location change isn't in the current location
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;

            GameLocation currentLocation = Game1.player.currentLocation;

            
            // ignore if player is in a location toggled off from Generic Config
            if (!Config.Anywhere) // works fine
            {
                if (!(currentLocation is MineShaft || currentLocation is Woods)) { return; }
                if (currentLocation is Woods && !Config.InWoods) { return; }
                if (currentLocation is MineShaft mineShaft)
                {
                    if (!Config.InSlimeArea && mineShaft.isSlimeArea) { return; }
                    if (!Config.InQuarryArea && mineShaft.isQuarryArea) { return; }
                    if (!Config.InSkullCaverns && (mineShaft.getMineArea() == 121)) { return; }
                    if (!Config.InRegularMines && (!mineShaft.isSlimeArea && !mineShaft.isQuarryArea && (mineShaft.getMineArea() != 121)))
                        { return; }
                }
            }

            foreach (Farmer farmer2 in currentLocation.farmers)
            {
                if (farmer2 != e.Player) { return; } // if there is a player in the location that isn't main player ?
            }

            // Game1.addHUDMessage(new HUDMessage(currentLocation.Name, 3)); // displays when slime is killed

            foreach (GreenSlime slime in currentLocation.characters.OfType<GreenSlime>())
            {
                slime.color.Value = new Microsoft.Xna.Framework.Color(rng.Next(255), rng.Next(255), rng.Next(255));
            }


        }




    }
}