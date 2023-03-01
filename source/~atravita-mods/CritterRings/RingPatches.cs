/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.Objects;

namespace CritterRings;

#warning - this will need to be refactored for 1.6

/// <summary>
/// Adds the other effects.
/// </summary>
[HarmonyPatch(typeof(Ring))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
internal static class RingPatches
{
    #region delegates
    private static readonly Lazy<Func<Ring, int?>> lightIDSourceGetter = new(() =>
        typeof(Ring).GetCachedField("_lightSourceID", ReflectionCache.FlagTypes.InstanceFlags)
                    .GetInstanceFieldGetter<Ring, int?>());

    private static readonly Lazy<Action<Ring, int?>> lightIDSourceSetter = new(() =>
        typeof(Ring).GetCachedField("_lightSourceID", ReflectionCache.FlagTypes.InstanceFlags)
                    .GetInstanceFieldSetter<Ring, int?>());
    #endregion

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Ring.onUnequip))]
    private static void PostfixUnequip(Ring __instance, Farmer who, GameLocation location)
    {
        if (__instance.ParentSheetIndex < 0)
        {
            return;
        }

        try
        {
            if (__instance.ParentSheetIndex == ModEntry.ButterflyRing)
            {
                who.MagneticRadius -= 128;
            }
            else if (__instance.ParentSheetIndex == ModEntry.FireFlyRing)
            {
                RemoveLightFrom(__instance, location);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to dequip ring!\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Ring.onNewLocation))]
    private static void PostfixNewLocation(Ring __instance, Farmer who, GameLocation environment)
    {
        if (__instance.ParentSheetIndex < 0)
        {
            return;
        }

        try
        {
            if (__instance.ParentSheetIndex == ModEntry.FireFlyRing)
            {
                __instance.onEquip(who, environment);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to deal with new location!\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Ring.onLeaveLocation))]
    private static void PostfixLeaveLocation(Ring __instance, GameLocation environment)
    {
        if (__instance.ParentSheetIndex < 0)
        {
            return;
        }

        try
        {
            if (__instance.ParentSheetIndex == ModEntry.FireFlyRing)
            {
                RemoveLightFrom(__instance, environment);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to leave location!\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Ring.onEquip))]
    private static void PostfixEquip(Ring __instance, Farmer who, GameLocation location)
    {
        if (__instance.ParentSheetIndex < 0)
        {
            return;
        }

        try
        {
            if (__instance.ParentSheetIndex == ModEntry.ButterflyRing)
            {
                who.MagneticRadius += 128;
            }
            else if (__instance.ParentSheetIndex == ModEntry.FireFlyRing)
            {
                int startingID = __instance.uniqueID.Value + (int)who.UniqueMultiplayerID;
                while (location.sharedLights.ContainsKey(startingID))
                {
                    startingID++;
                }
                lightIDSourceSetter.Value(__instance, startingID);

                location.sharedLights[startingID] = new LightSource(
                    textureIndex: 1,
                    new Vector2(who.Position.X + 21f, who.Position.Y + 64f),
                    radius: 10f,
                    new Color(0, 80, 0),
                    identifier: __instance.uniqueID.Value + (int)who.UniqueMultiplayerID,
                    light_context: LightSource.LightContext.None,
                    playerID: who.UniqueMultiplayerID);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to equip ring!\n\n{ex}", LogLevel.Error);
        }
    }

    private static void RemoveLightFrom(Ring __instance, GameLocation location)
    {
        int? lightID = lightIDSourceGetter.Value(__instance);
        if (lightID.HasValue)
        {
            location.removeLightSource(lightID.Value);
            lightIDSourceSetter.Value(__instance, null);
        }
    }
}
