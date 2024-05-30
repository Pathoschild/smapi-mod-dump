/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Buildings;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TransparencySettings
{
    public static class HarmonyPatch_BuildingTransparency
    {
        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The SMAPI helper instance to use for events and other API access.</summary>
        private static IModHelper Helper { get; set; } = null;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="helper">The <see cref="IModHelper"/> provided to this mod by SMAPI. Used for events and other API access.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IModHelper helper, IMonitor monitor)
        {
            if (!Applied && helper != null && monitor != null) //if NOT already applied AND valid tools were provided
            {
                Helper = helper; //store helper
                Monitor = monitor; //store monitor

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BuildingTransparency)}\": postfixing SDV method \"Building.Update(GameTime)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Building), nameof(Building.Update), new[] { typeof(GameTime) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_BuildingTransparency), nameof(Building_Update))
                );

                Applied = true;
            }
        }

        /// <summary>Modify the alpha (transparency) value of this instance after its per-tick update.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        private static void Building_Update(Building __instance)
        {
            try
            {
                if (ModEntry.Config.BuildingSettings.Enable)  //if this type of custom transparency is enabled
                {
                    if (__instance.fadeWhenPlayerIsBehind.Value == true) //if SDV normally allows this building to be transparent
                    {
                        var alpha = Helper.Reflection.GetField<float>(__instance, "alpha", true); //get the building's alpha field

                        if (ShouldBeTransparent(__instance)) //if this building should be MORE transparent this tick
                            alpha.SetValue(CacheManager.GetAlpha(__instance, -0.05f, ModEntry.Config.BuildingSettings.MinimumOpacity));
                        else //if this building should be LESS transparent this tick
                            alpha.SetValue(CacheManager.GetAlpha(__instance, 0.05f, ModEntry.Config.BuildingSettings.MinimumOpacity));
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BuildingTransparency)}\" has encountered an error. Custom building transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Determines whether the provided instance should be made more transparent this tick.</summary>
        private static bool ShouldBeTransparent(Building building)
        {
            if (InputManager.FullTransparency.Value) //if full transparency is toggled on
                return true;
            else if (InputManager.DisableTransparency.Value) //if transparency is toggled off
                return false;

            Rectangle buildingRect = new Rectangle(building.tileX.Value, building.tileY.Value, building.tilesWide.Value, building.tilesHigh.Value); //get the building's area
            int adjustedTileDistance = ModEntry.Config.BuildingSettings.TileDistance + ((buildingRect.Width + buildingRect.Height) / 2); //get the transparency distance setting, adjusted for overall building size

            if ((ModEntry.Config.BuildingSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < buildingRect.Top) && //if the building's bottom edge is below the player's Y level OR that option is disabled
                Vector2.Distance(new Vector2(buildingRect.Center.X, buildingRect.Center.Y), CacheManager.CurrentPlayerTile) < adjustedTileDistance) //AND if the player is within range of this building
                return true;

            return false;
        }
    }
}
