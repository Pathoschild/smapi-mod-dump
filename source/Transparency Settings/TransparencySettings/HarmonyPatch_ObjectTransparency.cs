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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace TransparencySettings
{
    public static class HarmonyPatch_ObjectTransparency
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

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ObjectTransparency)}\": prefixing SDV method \"Object.draw(SpriteBatch, int, int, float)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                    prefix: new HarmonyMethod(typeof(HarmonyPatch_ObjectTransparency), nameof(Object_draw))
                );

                Applied = true;
            }
        }

        /// <summary>Modifies the alpha (transparency) value passed to this instance's draw method.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        private static void Object_draw(StardewValley.Object __instance, int x, int y, ref float alpha)
        {
            try
            {
                if (!ModEntry.Config.ObjectSettings.Enable && !ModEntry.Config.CraftableSettings.Enable) //if both object types have transparency disabled
                    return; //do nothing (note: this is slightly faster than checking a net field first)

                ObjectSettings settings; //the settings to use, depending on what kind of object this is

                if (__instance.bigCraftable.Value)
                    settings = ModEntry.Config.CraftableSettings;
                else
                    settings = ModEntry.Config.ObjectSettings;

                if (settings.Enable && InputManager.DisableTransparency.Value == false) //if this type of custom transparency is enabled
                {
                    if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                        (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                        (settings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < y) && //and the object is below the player's Y level OR that option is disabled
                        Utility.distance(x, CacheManager.CurrentPlayerTile.X, y, CacheManager.CurrentPlayerTile.Y) < settings.TileDistance)) //and if the player is within range
                    {
                        alpha = CacheManager.GetAlpha(__instance, -0.05f, settings.MinimumOpacity); //make this object MORE transparent
                    }
                    else
                    {
                        alpha = CacheManager.GetAlpha(__instance, 0.05f, settings.MinimumOpacity); //make this object LESS transparent
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_ObjectTransparency)}\" has encountered an error. Custom object and craftable transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}
