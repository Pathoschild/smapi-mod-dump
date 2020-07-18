using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;

namespace WaterproofItems
{
    /// <summary>A Harmony patch that prevents item-containing debris sinking in water.</summary>
    public static class HarmonyPatch_FloatingItemBehavior
    {
        /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FloatingItemBehavior)}\": prefixing SDV method \"GameLocation.sinkDebris(Debris, Vector2, Vector2)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.sinkDebris), new[] { typeof(Debris), typeof(Vector2), typeof(Vector2) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_FloatingItemBehavior), nameof(sinkDebris_Prefix))
            );
        }

        /// <summary>Prevents debris sinking if it contains or represents an item.</summary>
        /// <param name="debris">The sinking debris.</param>
        /// <param name="__instance">The <see cref="GameLocation"/> on which this method was called.</param>
        /// <param name="__result">The original method's return value. True if the debris should sink, false otherwise.</param>
        public static bool sinkDebris_Prefix(Debris debris, GameLocation __instance, ref bool __result)
        {
            try
            {
                if (debris != null) //if the debris exists
                {
                    if (debris.IsAnItem()) //if this debris represents an item
                    {
                        if (ModEntry.Config.TeleportItemsOutOfWater) //if this item should teleport out of water
                            TeleportDebrisOutOfWater(__instance, debris);

                        __result = false; //return false instead of the original result
                        return false; //skip the rest of the original method (note: this also skips any other patches on the method, depending on order)
                    }
                }
                
                return true; //run the original method

            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(sinkDebris_Prefix)}\" has encountered an error:\n{ex.ToString()}", LogLevel.Error);
                return true; //run the original method
            }
        }

        /// <summary>Moves the provided <see cref="Debris"/> to the nearest player.</summary>
        /// <param name="debris">The debris to move out of water.</param>
        private static void TeleportDebrisOutOfWater(GameLocation location, Debris debris)
        {
            if (location == null || debris == null) //if the location and/or debris is null
                return; //do nothing

            foreach (Chunk chunk in debris.Chunks) //for each chunk of this debris
            {
                Farmer nearestPlayer = null;
                float nearestPlayerDistance = 0;

                foreach (Farmer player in location.farmers) //for each player at this location
                {
                    float distance = (player.Position - chunk.position.Value).LengthSquared(); //get the distance between this player and the chunk

                    if (nearestPlayer == null || distance < nearestPlayerDistance) //if this is the first player OR the nearest player so far
                    {
                        nearestPlayer = player; //update the nearest player
                        nearestPlayerDistance = distance; //update the nearest player's distance
                    }
                }

                if (nearestPlayer != null) //if any player was selected (i.e. there are any players at this location)
                {
                    chunk.position.Value = nearestPlayer.Position; //move this chunk to the nearest player's position
                    chunk.position.UpdateExtrapolation(chunk.getSpeed()); //update NetPosition data (note: this fixes behavioral issues in multiplayer & appears benign in other contexts)
                }
            }
        }
    }
}
