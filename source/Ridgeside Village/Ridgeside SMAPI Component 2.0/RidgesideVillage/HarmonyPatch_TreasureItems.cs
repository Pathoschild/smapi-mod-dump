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
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System.Reflection;

namespace RidgesideVillage
{
    //Handles the treasures for the spirit realm.
    //Mostly hardcoded for performance
    internal static class HarmonyPatch_TreasureItems
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        //activated often in getFish, so we cache it
        private static int CachedSapphireID;
        //activate tracker for fox statue
        private static bool OnFoxStatueMap;
        //count consecutive 10min intervals where player was close to statue
        private static int FoxStatueCounter;

        //mailflags
        const string FLAGMOOSE = "RSV.MooseStatue";
        const string FLAGFOXMASK = "RSV.FoxMask";
        const string FLAGSAPPHIRE = "RSV.Sapphire";
        const string FLAGMUSICBOX = "RSV.MusicBox";
        const string FLAGEVERFROST = "RSV.EverFrost";
        const string FLAGHEROSTATUE = "RSV.HeroStatue";
        const string FLAGCANDELABRUM = "RSV.Candelabrum";
        const string FLAGCOMB = "RSV.ElvenComb";
        const string FLAGOPAL = "RSV.OpalHalo";

        internal static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;

            Log.Trace($"Applying Harmony Patch from \"{nameof(HarmonyPatch_TreasureItems)}\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(Axe), nameof(Axe.DoFunction)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(Axe_DoFunction_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Hoe), nameof(Hoe.DoFunction)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(Hoe_DoFunction_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(WateringCan_DoFunction_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(FishingRod_DoFunction_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(FishingRod_PullFishFromWater_PostFix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TreasureItems), nameof(GameLocation_GetFish_Postifx))
            );

        }

        private static void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            FoxStatueCounter = 0;
            OnFoxStatueMap = false;
            Helper.Events.GameLoop.TimeChanged -= OnTimeChanged;
        }

        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //reset flags every season
            if(Game1.dayOfMonth == 1)
            {
                foreach (var field in typeof(HarmonyPatch_TreasureItems).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                {
                    if (field.Name.Contains("FLAG"))
                    {
                        Game1.player.mailReceived.Remove(field.GetValue(null).ToString());
                    }
                }
            }
        }

        //setup tracker for fox statue if needed
        private static void OnWarped(object sender, WarpedEventArgs e)
        {
            if(!OnFoxStatueMap && e.NewLocation.Name.Equals("Custom_Ridgeside_Ridge") && !Game1.player.mailReceived.Contains(FLAGFOXMASK)){
                FoxStatueCounter = 0;
                Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
                OnFoxStatueMap = true;
            }
            else if(OnFoxStatueMap && e.OldLocation.Name.Equals("Custom_Ridgeside_Ridge") && !e.NewLocation.Name.Equals("Custom_Ridgeside_Ridge")){

                FoxStatueCounter = 0;
                Helper.Events.GameLoop.TimeChanged -= OnTimeChanged;
                OnFoxStatueMap = false;
            }
        }

        //track player position for fox statue
        private static void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            int distance = Math.Abs(Game1.player.getTileX() - 18) + Math.Abs(Game1.player.getTileY() - 10);
            if(distance < 3)
            {
                FoxStatueCounter += 1;
            }
            else
            {
                FoxStatueCounter = 0;
            }

            if(FoxStatueCounter >= 12)
            {
                SpawnJAItemAsDebris("Relic Fox Mask", 18, 10, Game1.getLocationFromName("Custom_Ridgeside_Ridge"));
                Game1.player.mailReceived.Add(FLAGFOXMASK);
                FoxStatueCounter = 0;
                Helper.Events.GameLoop.TimeChanged -= OnTimeChanged;
                OnFoxStatueMap = false;
            }
        }

        private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            CachedSapphireID = ExternalAPIs.JA.GetObjectId("Sapphire Pearl");
        }

        private static void GameLocation_GetFish_Postifx(GameLocation __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref StardewValley.Object __result)
        {
            if ((int)bobberTile.X == 60 && (int)bobberTile.Y == 55 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RidgesideVillage") && !Game1.player.mailReceived.Contains(FLAGSAPPHIRE))
            {
                __result = new StardewValley.Object(CachedSapphireID, 1);
            }
        }

        internal static void Axe_DoFunction_Postfix(ref Axe __instance, int x, int y, int power, Farmer who)
        {
            try
            {
                int tileX = x / 64;
                int tileY = y / 64;
                if(tileX == 14 && tileY == 3 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_LogCabinHotel3rdFloor") && !Game1.player.mailReceived.Contains(FLAGMUSICBOX))
                {
                    SpawnJAItemAsDebris("Ancient Music Box", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGMUSICBOX);
                }
                else if(tileX == 5 && tileY == 3 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_AlissaShed") && !Game1.player.mailReceived.Contains(FLAGEVERFROST))
                {
                    Game1.player.mailReceived.Add(FLAGEVERFROST);
                    SpawnJAItemAsDebris("Everfrost Stone", tileX, tileY, Game1.currentLocation);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patch \"{nameof(Axe_DoFunction_Postfix)}\" has encountered an error. \n{e.ToString()}");
            }
        }

        internal static void Hoe_DoFunction_Postfix(ref Hoe __instance, int x, int y, int power, Farmer who)
        {
            try
            {
                int tileX = x / 64;
                int tileY = y / 64;
                if (tileX == 80 && tileY == 22 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RSVCliff") && !Game1.player.mailReceived.Contains(FLAGMOOSE))
                {
                    SpawnJAItemAsDebris("Moose Statue", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGMOOSE);

                }
                else if (tileX == 63 && tileY == 40 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RSVTheHike") && !Game1.player.mailReceived.Contains(FLAGCOMB))
                {
                    SpawnJAItemAsDebris("Elven Comb", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGCOMB);
                }
                else if (tileX == 23 && tileY == 6 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RSVCableCar") && !Game1.player.mailReceived.Contains(FLAGOPAL))
                {
                    SpawnJAItemAsDebris("Opal Halo", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGOPAL);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patch \"{nameof(Hoe_DoFunction_Postfix)}\" has encountered an error. \n{e.ToString()}");
            }
        }

        internal static void WateringCan_DoFunction_Postfix(ref WateringCan __instance, int x, int y, int power, Farmer who)
        {
            try
            {
                int tileX = x / 64;
                int tileY = y / 64;
                if (tileX == 11 && tileY == 7 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RSVGreenhouse2") && !Game1.player.mailReceived.Contains(FLAGCANDELABRUM))
                {
                    SpawnJAItemAsDebris("Pale Candelabrum", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGCANDELABRUM);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patch \"{nameof(WateringCan_DoFunction_Postfix)}\" has encountered an error. \n{e.ToString()}");
            }
        }

        //change result to sapphire if appropriate
        internal static void FishingRod_DoFunction_Postfix(ref WateringCan __instance, int x, int y, int power, Farmer who)
        {
            try
            {
                int tileX = x / 64;
                int tileY = y / 64;

                if (tileX == 145 && tileY == 69 && Game1.currentLocation.Name.Equals("Custom_Ridgeside_RidgesideVillage") && !Game1.player.mailReceived.Contains(FLAGHEROSTATUE))
                {
                    SpawnJAItemAsDebris("Village Hero Sculpture", tileX, tileY, Game1.currentLocation);
                    Game1.player.mailReceived.Add(FLAGHEROSTATUE);
                }
                
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patch \"{nameof(FishingRod_DoFunction_Postfix)}\" has encountered an error. \n{e.ToString()}");
            }
        }

        //if player pulled sapphire, add flag. not done in getFish() cus mod compatibility
        private static void FishingRod_PullFishFromWater_PostFix(ref FishingRod __instance, int whichFish, int fishSize, int fishQuality, int fishDifficulty, bool treasureCaught, bool wasPerfect, bool fromFishPond, bool caughtDouble, string itemCategory)
        {
            if (whichFish == CachedSapphireID)
            {
                Game1.player.mailReceived.Add(FLAGSAPPHIRE);
            }
        }

        internal static void SpawnJAItemAsDebris(string itemName, int tileX, int tileY, GameLocation location)
        {
            int itemID = ExternalAPIs.JA.GetObjectId(itemName);
            if (itemID > 0)
            {
                Game1.createObjectDebris(itemID, tileX, tileY, location: location);
            }

        }


    }
}
