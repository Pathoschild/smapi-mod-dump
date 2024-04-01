/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Linq;

namespace FishingTrawler.Patches.Locations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly Type _gameLocation = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {

            harmony.Patch(AccessTools.Method(_gameLocation, nameof(GameLocation.RunLocationSpecificEventCommand), new[] { typeof(Event), typeof(string), typeof(bool), typeof(string[]) }), postfix: new HarmonyMethod(GetType(), nameof(RunLocationSpecificEventCommandPatch)));
            harmony.Patch(AccessTools.Method(_gameLocation, nameof(GameLocation.performTouchAction), new[] { typeof(string), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(PerformTouchActionPatch)));
            harmony.Patch(AccessTools.Method(_gameLocation, nameof(GameLocation.isActionableTile), new[] { typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(IsActionableTilePatch)));
        }

        private static bool IsValidLocation(GameLocation location)
        {
            return location is Beach || location is IslandSouthEast;
        }

        internal static void RunLocationSpecificEventCommandPatch(GameLocation __instance, ref bool __result, Event current_event, string command_string, bool first_run, params string[] args)
        {
            if (!IsValidLocation(__instance))
            {
                return;
            }

            switch (command_string)
            {
                case "animate_boat_start":
                    FishingTrawler.trawlerObject._boatAnimating = true;
                    __result = true;
                    return;
                case "non_blocking_pause":
                    if (first_run)
                    {
                        int delay = 0;
                        if (args.Length < 0 || !int.TryParse(args[0], out delay))
                        {
                            delay = 0;
                        }
                        FishingTrawler.trawlerObject.nonBlockingPause = delay;
                        __result = false;
                        return;
                    }
                    FishingTrawler.trawlerObject.nonBlockingPause -= (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                    if (FishingTrawler.trawlerObject.nonBlockingPause < 0)
                    {
                        FishingTrawler.trawlerObject.nonBlockingPause = 0;
                        __result = true;
                        return;
                    }
                    __result = false;
                    return;
                case "boat_depart":
                    if (first_run)
                    {
                        FishingTrawler.trawlerObject._boatDirection = 1;
                    }
                    if (FishingTrawler.trawlerObject._boatOffset >= 150)
                    {
                        __result = true;
                        return;
                    }
                    __result = false;
                    return;
                case "close_gate":
                    FishingTrawler.trawlerObject._closeGate = true;
                    __result = true;
                    return;
                case "despawn_murphy":
                    if (FishingTrawler.murphyNPC != null)
                    {
                        FishingTrawler.murphyNPC = null;
                    }
                    __result = true;
                    return;
                case "warp_to_cabin":
                    __result = true;
                    return;
            }
        }

        internal static void PerformTouchActionPatch(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            if (Game1.eventUp || !IsValidLocation(__instance))
            {
                return;
            }

            if (fullActionString == "FishingTrawler_AttemptBoard")
            {
                Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.speak_to_captain"));
            }

            if (fullActionString == "FishingTrawler_NoMurphy")
            {
                Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.walk_the_plank"));
            }
        }

        internal static void IsActionableTilePatch(GameLocation __instance, ref bool __result, int xTile, int yTile, Farmer who)
        {
            if (__result || !IsValidLocation(__instance))
            {
                return;
            }

            string actionProperty = __instance.doesTileHaveProperty(xTile, yTile, "CustomAction", "Buildings");
            if (actionProperty != null && actionProperty == "TrawlerRewardStorage")
            {
                if (!Enumerable.Range((int)(who.Tile.X - 1), 3).Contains(xTile) || !Enumerable.Range((int)(who.Tile.Y - 1), 3).Contains(yTile))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                __result = true;
            }

            if (actionProperty != null && actionProperty == "TrawlerNote")
            {
                if (!Enumerable.Range((int)(who.Tile.X - 1), 3).Contains(xTile) || !Enumerable.Range((int)(who.Tile.Y - 1), 3).Contains(yTile))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                __result = true;
            }

            if (FishingTrawler.murphyNPC != null && FishingTrawler.murphyNPC.Tile.X == xTile && FishingTrawler.murphyNPC.Tile.Y == yTile)
            {
                if (!Utility.tileWithinRadiusOfPlayer(xTile, yTile, 1, who))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                Game1.isSpeechAtCurrentCursorTile = true;
                __result = true;
            }
        }
    }
}
