using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that modifies the mouse cursor while hovering over a <see cref="PlacedItem"/>.</summary>
        public class HarmonyPatch_UpdateCursorOverPlacedItem
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(HarmonyInstance harmony)
            {
                Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_UpdateCursorOverPlacedItem)}\": postfixing SDV method \"Utility.canGrabSomethingFromHere(int, int, Farmer)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.canGrabSomethingFromHere), new[] { typeof(int), typeof(int), typeof(Farmer) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_UpdateCursorOverPlacedItem), nameof(canGrabSomethingFromHere_Postfix))
                );
            }

            /// <summary>Displays the "pick up object" mouse cursor type while the cursor hovers over a <see cref="PlacedItem"/>.</summary>
            /// <remarks>
            /// This method imitates the check performed in the original, but targets <see cref="PlacedItem"/> instead of harvestable crops within <see cref="HoeDirt"/>.
            /// </remarks>
            /// <param name="who">The player </param>
            public static void canGrabSomethingFromHere_Postfix(int x, int y, Farmer who, ref bool __result)
            {
                try
                {
                    if (!__result && Game1.mouseCursor <= 0) //if the original result is false & the cursor appears to be in its default state
                    {
                        if (who.IsLocalPlayer && Context.IsPlayerFree && Game1.currentLocation != null) //if "who" is the local player, free to act, and at a known location
                        {
                            //imitate the original "harvestable crops" check, but just look for PlacedItem instead
                            Vector2 index = new Vector2(x / 64, y / 64); //get the tile position from the provided pixel position
                            if (Game1.currentLocation.terrainFeatures.ContainsKey(index) && Game1.currentLocation.terrainFeatures[index] is PlacedItem) //if there's a placed item at the tile
                            {
                                Game1.mouseCursor = 6; //set the "grab" cursor
                                if (StardewValley.Utility.withinRadiusOfPlayer(x, y, 1, who)) //if the player is within reach of the tile
                                    __result = true; //overwrite the result
                                else //if the player is NOT within reach the tile
                                    Game1.mouseCursorTransparency = 0.5f; //make the cursor semi-transparent
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce("A Harmony patch encountered an error. Your cursor might not change when hovering over spawned items. The auto-generated error message is displayed below:\n" +
                        "----------\n" +
                        ex.ToString(), LogLevel.Warn);
                }
            }
        }
    }
}
