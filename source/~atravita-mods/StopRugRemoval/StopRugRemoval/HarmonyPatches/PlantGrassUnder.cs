/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using AtraBase.Toolkit.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Class to hold patches to place grass.
/// </summary>
[HarmonyPatch]
internal static class PlantGrassUnder
{
    private static Func<bool>? isSmartBuildingInBuildMode = null;

    /// <summary>
    /// Gets the methods to patch.
    /// </summary>
    /// <returns>An IEnumerable of methods to patch.</returns>
    public static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (Type t in typeof(SObject).GetAssignableTypes(publiconly: true, includeAbstract: false))
        {
            if (t != typeof(CrabPot) // does not make sense to place under crab pots.
                && t.DeclaredInstanceMethodNamedOrNull(nameof(SObject.performObjectDropInAction), new Type[] { typeof(Item), typeof(bool), typeof(Farmer) }) is MethodBase method
                && method.DeclaringType == t)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Postfixes Perform ObjectDropInAction to allow for grass starters to placed under things.
    /// </summary>
    /// <param name="__instance">Object being placed under.</param>
    /// <param name="__0">Item to drop in.</param>
    /// <param name="__1">"Probe": just checking? (Zero clue).</param>
    /// <param name="__2">The farmer doing the placing.</param>
    /// <param name="__result">The result to substitute in.</param>
    [HarmonyPostfix]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style preferred by Harmony")]
    public static void PostfixPerformObjectDropInAction(SObject __instance, Item __0, bool __1, Farmer __2, ref bool __result)
    {
        if (__result // Placed something already
           || __1 // just checking!
           || __2.currentLocation is null
           || !ModEntry.Config.Enabled
           || !ModEntry.Config.PlaceGrassUnder)
        {
            return;
        }
        try
        {
            // TODO: fix this with MoreGrassStarters.

            // Grass starter = 297
            if (Utility.IsNormalObjectAtParentSheetIndex(__0, 297) && !(isSmartBuildingInBuildMode?.Invoke() == true))
            {
                GameLocation location = __2.currentLocation;
                Vector2 placementTile = __instance.TileLocation;

                if (!location.terrainFeatures.ContainsKey(placementTile) && !location.isWaterTile((int)placementTile.X, (int)placementTile.Y))
                {
                    location.terrainFeatures.Add(placementTile, new Grass(Grass.springGrass, 4));
                    location.playSound("dirtyHit");
                    __result = true;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Rain into errors attempting to place grass under object at {__instance.TileLocation}.\n\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Grabs a reference to Smart Building's CurrentlyInBuildMode.
    /// </summary>
    /// <param name="registry">ModRegistry.</param>
    internal static void GetSmartBuildingBuildMode(IModRegistry registry)
    {
        if (registry.Get("DecidedlyHuman.SmartBuilding") is not IModInfo info || info.Manifest.Version.IsOlderThan("1.3.2"))
        {
            ModEntry.ModMonitor.Log("SmartBuilding not installed, no need to adjust for that", LogLevel.Trace);
        }
        else if (Type.GetType("SmartBuilding.HarmonyPatches.Patches, SmartBuilding") is Type type
            && AccessTools.DeclaredPropertyGetter(type, "CurrentlyInBuildMode") is MethodInfo method)
        {
            ModEntry.ModMonitor.Log("SmartBuilding found! " + method.FullDescription(), LogLevel.Trace);
            isSmartBuildingInBuildMode = method.CreateDelegate<Func<bool>>();
        }
        else
        {
            ModEntry.ModMonitor.Log("SmartBuilding is installed BUT compat unsuccessful. You may see issues, please bring this log to atravita!", LogLevel.Info);
        }
    }
}