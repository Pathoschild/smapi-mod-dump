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
    public static class HarmonyPatch_TreeTransparency
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

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_TreeTransparency)}\": postfixing SDV method \"Tree.tickUpdate(GameTime, Vector2, GameLocation)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Tree), nameof(Tree.tickUpdate), new[] { typeof(GameTime), typeof(Vector2), typeof(GameLocation) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_TreeTransparency), nameof(Tree_updateTick))
                );

                Applied = true;
            }
        }

        /// <summary>Modify the alpha (transparency) value of this instance after its per-tick update.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        private static void Tree_updateTick(Tree __instance, Vector2 tileLocation)
        {
            try
            {
                if (ModEntry.Config.TreeSettings.Enable && InputManager.DisableTransparency.Value == false) //if this type of custom transparency is enabled
                {
                    if (__instance.growthStage.Value >= 5 && __instance.stump.Value == false) //if this tree is fully grown and NOT cut down
                    {
                        var alpha = Helper.Reflection.GetField<float>(__instance, "alpha", true); //get this tree's alpha field
                        Vector2 treeTile = new Vector2(tileLocation.X, tileLocation.Y - 2); //adjust toward the center of the tree's sprite

                        if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                            (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                            (ModEntry.Config.TreeSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < tileLocation.Y) && //OR if the tree is below the player's Y level OR that option is disabled,
                            Vector2.Distance(treeTile, CacheManager.CurrentPlayerTile) < ModEntry.Config.TreeSettings.TileDistance)) //AND if the player is within range of this tree
                        {
                            alpha.SetValue(CacheManager.GetAlpha(__instance, -0.05f)); //make this tree MORE transparent
                        }
                        else
                        {
                            alpha.SetValue(CacheManager.GetAlpha(__instance, 0.05f)); //make this tree LESS transparent
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_TreeTransparency)}\" has encountered an error. Custom tree transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}
