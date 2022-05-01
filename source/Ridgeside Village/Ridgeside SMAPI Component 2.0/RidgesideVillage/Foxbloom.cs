/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewModdingAPI.Utilities;
using SpaceCore.Events;

namespace RidgesideVillage
{
    internal static class Foxbloom
    {
        static List<Vector2> spawn_spots;
        static bool spawned_today = false;
        static bool cc_reloaded = false;

        static IModHelper Helper;
        static IMonitor Monitor;

        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            spawn_spots = new List<Vector2> { new Vector2(84, 126), new Vector2(37, 96), new Vector2(18, 76),
                new Vector2(146, 99), new Vector2(108, 58), new Vector2(118, 44), new Vector2(58, 11) };

            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Player.Warped += OnWarped;
            //Helper.Events.Player.Warped += OnWarped2;
        }

        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Game1.getLocationFromName("Custom_Ridgeside_RidgeForest").modData["RSV_foxbloomSpawned"] = "false";
        }

        private static void OnWarped(object sender, WarpedEventArgs e)
        {
            if ((!CustomCPTokens.FoxbloomCanSpawn(e.NewLocation, spawned_today)) || e.NewLocation.modData["RSV_foxbloomSpawned"] == "true")
                return;

            Random random = new();
            Vector2 spawn_spot = spawn_spots.ElementAt(random.Next(0, 7));
            int FOXBLOOMID = ExternalAPIs.JA.GetObjectId("Foxbloom");
            try
            {
                UtilFunctions.SpawnForage(FOXBLOOMID, e.NewLocation, spawn_spot, true);
                Log.Debug("RSV: Foxbloom spawned as forage.");
                spawned_today = true;
                e.NewLocation.modData["RSV_foxbloomSpawned"] = "true";
            }
            catch(Exception ex)
            {
                Log.Error($"RSV: Error spawning Foxbloom at {spawn_spot.X}, {spawn_spot.Y}\n{ex}");
            }
        }

        /*
        [EventPriority(EventPriority.Low)]
        private static void OnWarped2(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name != "Custom_Ridgeside_RidgeForest")
                return;

            string foxbloomSpawned = e.NewLocation.modData["RSV_foxbloomSpawned"];
            if ((foxbloomSpawned == "false") || cc_reloaded)
                return;
            if ((foxbloomSpawned == "true") && !cc_reloaded)
            {
                Log.Info("RSV: Reloading CCs.");
                ExternalAPIs.CC.ReloadContentPack("Rafseazz.RSVCC");
                cc_reloaded = true;
                return;
            }
        }
        */
        
    }

  
}
