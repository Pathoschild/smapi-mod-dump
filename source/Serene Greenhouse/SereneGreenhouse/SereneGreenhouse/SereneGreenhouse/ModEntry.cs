/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using SereneGreenhouse.API.Interfaces;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace SereneGreenhouse
{
    public class ModEntry : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;

        // ModData related
        internal static string offeringsStoredInWaterHutKey;

        // API related
        IContentPatcherAPI contentPatcherApi;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and config
            monitor = Monitor;
            modHelper = helper;

            // Setup our ModData keys
            offeringsStoredInWaterHutKey = $"{this.ModManifest.UniqueID}/offerings-stored-in-water-hut";

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
                return;
            }

            // Hook into GameLaunched event
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            // Hook into the Warped event
            helper.Events.Player.Warped += OnWarped;

            // Hook into the DayStarted event
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Hook into Pathoschild's Content Patcher API
            try
            {
                contentPatcherApi = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to hook into ContentPatcher: {ex}", LogLevel.Error);
                return;
            }

            contentPatcherApi.RegisterToken(this.ModManifest, "GreenhouseStage", () =>
            {
                if (Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_3"))
                {
                    return new[] { "SereneGreenhouse_Expansion_3" };
                }
                if (Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_2"))
                {
                    return new[] { "SereneGreenhouse_Expansion_2" };
                }
                if (Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_1"))
                {
                    return new[] { "SereneGreenhouse_Expansion_1" };
                }
                return new[] { "SereneGreenhouse" };
            });
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.OldLocation.NameOrUniqueName == "Greenhouse")
            {
                for (int i = e.OldLocation.characters.Count - 1; i >= 0; i--)
                {
                    if (e.OldLocation.characters[i] is Junimo)
                    {
                        e.OldLocation.characters.RemoveAt(i);
                    }
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GameLocation greenhouse = Game1.getLocationFromName("Greenhouse");
            if (greenhouse is null)
            {
                Monitor.Log("Was unable to locate Greenhouse!", LogLevel.Debug);
                return;
            }

            greenhouse.map.Properties["AmbientLight"] = "255 255 255";
            if (Game1.isSnowing || Game1.isRaining)
            {
                greenhouse.map.Properties["AmbientLight"] = "95 95 95";
            }

            // Water any crops in the greenhouse if the Waterhut has offerings
            if (Game1.player.IsMainPlayer && Game1.MasterPlayer.modData.ContainsKey(offeringsStoredInWaterHutKey))
            {
                int offeringsCount = 0;
                if (!int.TryParse(Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey], out offeringsCount))
                {
                    monitor.Log($"Issue parsing ModData key [{ModEntry.offeringsStoredInWaterHutKey}]'s value to int", LogLevel.Trace);
                }

                if (offeringsCount > 0)
                {
                    Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey] = (offeringsCount - 1).ToString();

                    foreach (HoeDirt hoeDirt in greenhouse.terrainFeatures.Values.Where(p => p is HoeDirt))
                    {
                        hoeDirt.state.Value = 1;
                    }
                }
            }

            // Reset the custom tile properties
            foreach (var tile in greenhouse.map.GetLayer("Buildings").Tiles.Array)
            {
                if (tile is null || tile.Properties is null)
                {
                    continue;
                }

                if (tile.Properties.ContainsKey("HasReceivedOfferingToday"))
                {
                    tile.Properties["HasReceivedOfferingToday"] = false;
                }
            }
        }

        public static void AcceptOffering(Farmer who, string message, int countToRemove)
        {
            Game1.drawObjectDialogue(message);
            RemoveActiveItemByCount(who, countToRemove);

            Game1.playSound("newRecord");
        }

        public static void AcceptOffering(Farmer who, string message, int countToRemove, Tile tile)
        {
            AcceptOffering(who, message, countToRemove);

            tile.Properties["HasReceivedOfferingToday"] = true;
        }

        public static void RemoveActiveItemByCount(Farmer farmer, int countToRemove)
        {
            if (farmer.CurrentItem != null && (farmer.CurrentItem.Stack -= countToRemove) <= 0)
            {
                farmer.removeItemFromInventory(farmer.CurrentItem);
                farmer.showNotCarrying();
            }
        }
    }
}
