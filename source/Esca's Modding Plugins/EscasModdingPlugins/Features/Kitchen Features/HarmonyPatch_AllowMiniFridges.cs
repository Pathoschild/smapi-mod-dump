/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;

namespace EscasModdingPlugins
{
    /// <summary>Allows mods to designate areas where Mini-Fridges can be placed by players. Uses map properties.</summary>
    public static class HarmonyPatch_AllowMiniFridges
    {
        /// <summary>The name of the map property used by this patch.</summary>
        public static string MapPropertyName { get; set; } = null;

        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                return;

            //store args
            Monitor = monitor;

            //initialize assets/properties
            MapPropertyName = ModEntry.PropertyPrefix + "AllowMiniFridges";

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_AllowMiniFridges)}\": prefixing method \"Object.placementAction(GameLocation, int, int, Farmer)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_AllowMiniFridges), nameof(Object_placementAction))
            );

            Applied = true;
        }

        /// <summary>Intercepts mini-fridge placement logic and allows placement based on this patch's map property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="location">The location where the object is being placed.</param>
        /// <param name="x">The horizontal pixel position where the object is being placed.</param>
        /// <param name="y">The vertical pixel position where the object is being placed.</param>
        /// <param name="who">The farmer trying to place the object. May be null.</param>
        /// <param name="__result">True if an action was performed; false otherwise.</param>
        /// <returns>True if the original method should be skipped; false otherwise.</returns>
        [HarmonyPriority(Priority.LowerThanNormal)] //run this prefix after most others to reduce interference with other mods
        private static bool Object_placementAction(StardewValley.Object __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
        {
            try
            {
                Vector2 placementTile = new Vector2(x / 64, y / 64);

				if (__instance.QualifiedItemId == "(BC)216") //if this object is a Mini-Fridge

				{
					if (!location.objects.ContainsKey(placementTile) && ShouldAllowMiniFridgesHere(location)) //if this tile is unobstructed (original check) AND this patch should allow placement 
					{
						//apply changes normally made at the start of the original method
						__instance.setHealth(10);
						if (who != null)
							__instance.owner.Value = who.UniqueMultiplayerID;
						else
							__instance.owner.Value = Game1.player.UniqueMultiplayerID;

						//imitate the original method's code for successful placement
						Chest fridge = new Chest("216", placementTile, 217, 2)
						{
							shakeTimer = 50
						};
						fridge.fridge.Value = true;
						location.objects.Add(placementTile, fridge);
						location.playSound("hammer");
						
						__result = true; //return true
						return false; //skip the original method
					}					
				}

                return true; //default result: run the original method
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_AllowMiniFridges)}..{nameof(Object_placementAction)}\" has encountered an error. Mini-Fridge placement might not be allowed at custom locations. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return true; //run the original method
            }
        }

        /// <summary>Determines whether this mod's settings allow Mini-Fridge placement at the given location.</summary>
        /// <param name="location">The location being checked.</param>
        /// <returns>True if Mini-Fridge placement should be allowed here; false otherwise.</returns>
        private static bool ShouldAllowMiniFridgesHere(GameLocation location)
        {
            if (ModConfig.Instance.AllowMiniFridgesEverywhere) //if config allows placement
            {
                if (Monitor?.IsVerbose == true)
                    Monitor.LogOnce($"Allowing Mini-Fridge placement due to config.json settings.", LogLevel.Trace);
                return true;
            }

            if (location.Map.Properties.TryGetValue(MapPropertyName, out var mapPropertyObject)) //if no tile property exists for this tile, but the location has a non-null map property
            {
                string mapProperty = mapPropertyObject?.ToString() ?? ""; //get the map property as a string

                bool result = !mapProperty.Trim().StartsWith("F", StringComparison.OrdinalIgnoreCase); //true if the property's value is NOT "false"

                if (Monitor?.IsVerbose == true)
                {
                    if (result)
                        Monitor.Log($"Allowing Mini-Fridge placement. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                    else
                        Monitor.Log($"NOT allowing Mini-Fridge placement. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                }

                return result;
            }

            if (Monitor?.IsVerbose == true)
                Monitor.Log($"NOT allowing Mini-Fridge placement; no relevant map property. Location: {location?.Name}.", LogLevel.Trace);

            return false; //default to false
        }
    }
}
