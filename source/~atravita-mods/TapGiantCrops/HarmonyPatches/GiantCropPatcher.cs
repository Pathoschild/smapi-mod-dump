/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace TapGiantCrops.HarmonyPatches;

/// <summary>
/// Holds patches to remove the tapper before the big crop is destroyed.
/// </summary>
[HarmonyPatch(typeof(GiantCrop))]
internal static class GiantCropPatcher
{
    [HarmonyPatch(nameof(GiantCrop.performToolAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool Prefix(GiantCrop __instance, Tool t)
    {
        if (t.isHeavyHitter() && t is not MeleeWeapon)
        {
            try
            {
                for (int x = (int)__instance.tile.X; x < (int)__instance.tile.X + __instance.width.Value; x++)
                {
                    for (int y = (int)__instance.tile.Y; y < (int)__instance.tile.Y + __instance.width.Value; y++)
                    {
                        Vector2 tile = new(x, y);
                        if (Game1.currentLocation.objects.TryGetValue(tile, out StardewValley.Object? obj)
                            && obj.Name.Contains("Tapper", StringComparison.OrdinalIgnoreCase))
                        {
                            obj.performRemoveAction(obj.TileLocation, Game1.currentLocation);
                            Game1.createItemDebris(obj, tile * 64f, -1);
                            Game1.currentLocation.objects.Remove(tile);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed while popping the tapper off {ex}", LogLevel.Error);
            }
        }
        return true;
    }
}
