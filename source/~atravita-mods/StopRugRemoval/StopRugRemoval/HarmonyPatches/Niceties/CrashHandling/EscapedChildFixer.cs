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

using StardewValley.Characters;
using StardewValley.Locations;

namespace StopRugRemoval.HarmonyPatches.Niceties.CrashHandling;

/// <summary>
/// Returns escaped children.
/// </summary>
[HarmonyPatch(typeof(Child))]
internal static class EscapedChildFixer
{
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(nameof(Child.dayUpdate))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named For Harmony.")]
    private static bool Prefix(Child __instance)
    {
        try
        {
            if (__instance.currentLocation is not FarmHouse)
            {
                ModEntry.ModMonitor.Log($"Child {__instance.Name} seems to have escaped the farmhouse, sending them back.", LogLevel.Trace);

                Farmer parent = Game1.MasterPlayer;

                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    if (farmer.UniqueMultiplayerID == __instance.idOfParent.Value)
                    {
                        parent = farmer;
                        break;
                    }
                }

                if ((Utility.getHomeOfFarmer(parent) ?? Game1.getLocationFromName("FarmHouse")) is not FarmHouse house)
                {
                    ModEntry.ModMonitor.Log($"Failed to find farmhouse", LogLevel.Error);
                    return false;
                }

                // day update should fix the location if this succeeds.
                Game1.warpCharacter(__instance, house, Vector2.One);

                if (__instance.currentLocation is not FarmHouse)
                {
                    ModEntry.ModMonitor.Log($"Failed while trying to return child {__instance.Name} to Farmhouse.", LogLevel.Error);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to return child to farmhouse:\n\n{ex}", LogLevel.Error);
        }

        return true;
    }
}
