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
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using Microsoft.Xna.Framework;

namespace RidgesideVillage
{
    //Corrects the location name in the "X has begun in Y" message
    internal static class Fish
    {
        private static IModHelper Helper { get; set; }

        const int CURIOSITY_LURE = 856;
        const int MAGIC_BAIT = 908;
        const double BASE_CATCH_CHANCE = 0.3;
        const double CATCH_CHANCE_WITH_CURIOLURE = 0.75;  // Curiosity Lure increases chance by 7%
        const int MIN_FISHING = 7;

        static Dictionary<string, int> fishIDs = new Dictionary<string, int>();

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            harmony.Patch(
              original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.isFishBossFish)),
              postfix: new HarmonyMethod(typeof(Fish), nameof(Fish.IsFishBossFish_Postfix))
              );
            harmony.Patch(
              original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
              prefix: new HarmonyMethod(typeof(Fish), nameof(Fish.DoFunction_Prefix))
              );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(Fish), nameof(Fish.GetFish_Postfix))
               );
        }

        public static void IsFishBossFish_Postfix(ref bool __result, int index)
        {
            if (__result)
                return;
            if (index == GetFishID("Sockeye Salmon"))
                __result = true;
            else if (index == GetFishID("Waterfall Snakehead"))
                __result = true;
            else if (index == GetFishID("Deep Ridge Angler"))
                __result = true;
        }

        public static bool DoFunction_Prefix(FishingRod __instance, GameLocation location, int x, int y, Farmer who)
        {
            if ((location.Name == RSVConstants.L_POND) && Game1.player.team.SpecialOrderActive(RSVConstants.SO_LINKEDFISH))
            {
                int tileX = (int)(__instance.bobber.X / 64f);
                int tileY = (int)(__instance.bobber.Y / 64f);
                if (location.isTileFishable(tileX, tileY))
                {
                    Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("RidgePond.NoFishing"));
                    who.UsingTool = false;
                    who.canMove = true;
                    return false;
                }
            }
            return true;
        }

        public static void GetFish_Postfix(GameLocation __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref StardewValley.Object __result)
        {
            try
            {
                string here = locationName ?? __instance.Name;

                double catchChance =
                    (who.CurrentTool is StardewValley.Tools.FishingRod rod && rod.getBobberAttachmentIndex() == CURIOSITY_LURE)
                    ? CATCH_CHANCE_WITH_CURIOLURE
                    : BASE_CATCH_CHANCE
                    ;

                int replaceByFishID = -1;
                bool shouldReplace = false;
                switch (here)
                {
                    //Linked Fish
                    case RSVConstants.L_POND:
                        var linked_fish = new List<string> { "Bladetail Sturgeon", "Caped Tree Frog", "Cardia Septal Jellyfish", "Crimson Spiked Clam",
                            "Fairytale Lionfish", "Fixer Eel", "Golden Rose Fin", "Harvester Trout", "Lullaby Carp", "Pebble Back Crab" };
                        Random random = new();
                        replaceByFishID = GetFishID(linked_fish.ElementAt(random.Next(0, 10)));
                        shouldReplace = true;
                        break;
                    //Legendaries
                    case RSVConstants.L_VILLAGE:
                    case RSVConstants.L_FALLS:
                    case RSVConstants.L_FOREST:
                        replaceByFishID = TryGetLegendary(who, here, bobberTile);
                        shouldReplace = Game1.random.NextDouble() <= catchChance;
                        break;
                    default:
                        return;
                }
                if (shouldReplace && replaceByFishID != -1)
                {
                    __result = new StardewValley.Object(replaceByFishID, 1);
                    return;
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(GetFish_Postfix)}:\n{ex}");
            }
        }

        private static int TryGetLegendary(Farmer who, string location, Vector2 bobberTile)
        {
            if (who.FishingLevel < MIN_FISHING)
                    return -1;

            int fishID = -1;
            bool is_bait_magic = (who.CurrentTool is FishingRod rod && rod.getBaitAttachmentIndex() == MAGIC_BAIT);
            switch (location)
            {
                case RSVConstants.L_VILLAGE:
                    int legendary = GetFishID("Sockeye Salmon");
                    if (new Rectangle(71, 93, 3, 3).Contains((int)bobberTile.X, (int)bobberTile.Y) && !CheckCaughtBefore(who, legendary))
                    {
                        if (is_bait_magic || (Game1.currentSeason.Equals("fall") && Game1.IsRainingHere(Game1.currentLocation)))
                            fishID = legendary;
                    }
                    break;
                case RSVConstants.L_FALLS:
                    legendary = GetFishID("Waterfall Snakehead");
                    if (new Rectangle(53, 11, 4, 4).Contains((int)bobberTile.X, (int)bobberTile.Y) && !CheckCaughtBefore(who, legendary))
                    {
                        if (is_bait_magic || ((Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")) && Game1.timeOfDay >= 2000))
                            fishID = legendary;
                    }
                    break;
                case RSVConstants.L_FOREST:
                    legendary = GetFishID("Deep Ridge Angler");
                    if (new Rectangle(67, 30, 5, 6).Contains((int)bobberTile.X, (int)bobberTile.Y) && !CheckCaughtBefore(who, legendary))
                    {
                        if (is_bait_magic || (Game1.currentSeason.Equals("winter") && Game1.timeOfDay >= 1200))
                            fishID = legendary;
                    }
                    break;
            }
            return fishID;
        }

        private static bool CheckCaughtBefore(Farmer who, int fishID)
        {
            if (Helper.ModRegistry.IsLoaded("DaLion.Overhaul") && who.professions.Contains(100 + Farmer.angler))
            {
                return false;
            }
            if (who.fishCaught.ContainsKey(fishID))
            {
                return true;
            }
            return false;
        }

        private static int GetFishID(string name)
        {
            if(fishIDs.TryGetValue(name, out int fishID))
            {
                return fishID;
            }
            else
            {
                fishID = ExternalAPIs.JA.GetObjectId(name);
                if(fishID != -1)
                {
                    fishIDs[name] = fishID;
                    return fishID;
                }
                else
                {
                    Log.Error($"Fish {name} not found.");
                    return -1;
                }
            }
        }
    }

}
