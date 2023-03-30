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

using AtraCore.Framework.ReflectionManager;

using FastExpressionCompiler.LightExpression;

using GrowableGiantCrops.Framework;

using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.Compat;

/// <summary>
/// Patches for FTM's burial spot.
/// </summary>
internal static class FTMArtifactSpotPatch
{
    #region delegates
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "Reviewed.")]
    private static readonly Lazy<Func<SObject, bool>?> isBuriedItem = new(() =>
    {
        Type? buriedItem = AccessTools.TypeByName("FarmTypeManager.ModEntry+BuriedItems");
        if (buriedItem is null)
        {
            return null;
        }

        ParameterExpression? obj = Expression.ParameterOf<SObject>("obj");
        TypeBinaryExpression? express = Expression.TypeIs(obj, buriedItem);
        return Expression.Lambda<Func<SObject, bool>>(express, obj).CompileFast();
    });

    /// <summary>
    /// Gets whether an item is a MoreGrassStarter grass starter.
    /// </summary>
    internal static Func<SObject, bool>? IsBuriedItem => isBuriedItem.Value;
    #endregion

    /// <summary>
    /// Applies the patches for this class.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatch(Harmony harmony)
    {
        Type? buriedItem = AccessTools.TypeByName("FarmTypeManager.ModEntry+BuriedItems");
        if (buriedItem is null)
        {
            ModEntry.ModMonitor.Log($"Farm Type Manager's buried items may not behave correctly if dug up with the shovel.", LogLevel.Error);
            return;
        }

        try
        {
            harmony.Patch(
                original: buriedItem.GetCachedMethod("performToolAction", ReflectionCache.FlagTypes.InstanceFlags),
                prefix: new HarmonyMethod(typeof(FTMArtifactSpotPatch).StaticMethodNamed(nameof(Prefix))));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to patch FTM to support artifact spots.\n\n{ex}", LogLevel.Error);
        }
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool Prefix(SObject __instance, Tool t, GameLocation location, ref bool __result)
    {
        if (t is not ShovelTool shovel)
        {
            return true;
        }

        try
        {
            __result = true;
            int count = shovel.hasEnchantmentOfType<GenerousEnchantment>() ? 2 : 1;
            MethodInfo method = __instance.GetType().GetCachedMethod("releaseContents", ReflectionCache.FlagTypes.InstanceFlags);

            do
            {
                method.Invoke(__instance, new[] { location });
            }
            while (count-- > 0);

            if (!location.terrainFeatures.ContainsKey(__instance.TileLocation))
            {
                location.makeHoeDirt(__instance.TileLocation);
            }
            location.playSound("hoeHit");
            return false;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to use shovel on FTM artifact spot:\n\n{ex}", LogLevel.Error);
        }

        return true;
    }
}
