/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TransparencySettings
{
    public static class HarmonyPatch_BushTransparency
    {
        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The SMAPI helper instance to use for events and other API access.</summary>
        private static IModHelper Helper { get; set; } = null;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="HarmonyInstance"/> created with this mod's ID.</param>
        /// <param name="helper">The <see cref="IModHelper"/> provided to this mod by SMAPI. Used for events and other API access.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(HarmonyInstance harmony, IModHelper helper, IMonitor monitor)
        {
            if (!Applied && helper != null && monitor != null) //if NOT already applied AND valid tools were provided
            {
                Helper = helper; //store helper
                Monitor = monitor; //store monitor

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\": postfixing SDV method \"Bush.tickUpdate(GameTime, Vector2, GameLocation)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Bush), nameof(Bush.tickUpdate), new[] { typeof(GameTime), typeof(Vector2), typeof(GameLocation) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_BushTransparency), nameof(Bush_tickUpdate))
                );

                Applied = true;
            }
        }

        /// <summary>Modify the alpha (transparency) value of this instance after its per-tick update.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        private static void Bush_tickUpdate(Bush __instance, Vector2 tileLocation)
        {
            try
            {
                if (ModEntry.Config.BushSettings.Enable) //if this type of custom transparency is enabled
                {
                    if (__instance.size.Value != 3) //if this bush is NOT a tea bush
                    {
                        var alpha = Helper.Reflection.GetField<float>(__instance, "alpha", true); //get this bush's alpha field
                        Point centerPixel = __instance.getRenderBounds(__instance.tilePosition.Value).Center; //get the center pixel of the bush's visible area

                        if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                            (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                            (ModEntry.Config.BushSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < __instance.tilePosition.Value.Y) && //AND if the tree is below the player's Y level OR that option is disabled,
                            Vector2.Distance(new Vector2(centerPixel.X, centerPixel.Y), CacheManager.CurrentPlayerTile * Game1.tileSize) < (ModEntry.Config.BushSettings.TileDistance * Game1.tileSize))) //AND if the player is within range of this bush's center pixel
                        {
                            alpha.SetValue(CacheManager.GetAlpha(__instance, -0.05f)); //make this bush MORE transparent
                        }
                        else
                        {
                            alpha.SetValue(CacheManager.GetAlpha(__instance, 0.05f)); //make this bush LESS transparent
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\" has encountered an error. Custom bush transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}
