/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8600

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantAssignment

namespace Calcifer.Features;

[HarmonyPatch]
class CategoryNameOverridePatches
{
    internal static readonly ConditionalWeakTable<string, string> CachedCategoryNameOverrides = new();
    internal const string CategoryCustomField = "sophie.Calcifer/Category";

    [HarmonyPatch(typeof(Object), nameof(Object.getCategoryName))]
    [HarmonyPrefix]
    public static bool getCategoryName_Prefix(ref Object __instance, ref string __result, ref bool __state)
    {
        if (CachedCategoryNameOverrides.TryGetValue(__instance.QualifiedItemId, out string cachedValue))
        {
            __result = cachedValue;
            __state = true;
            return false;
        }
        
        if (Game1.objectData.TryGetValue(__instance.ItemId, out ObjectData objData) && objData.CustomFields is not null && objData.CustomFields.TryGetValue(CategoryCustomField, out string categoryOverride))
        {
            __result = categoryOverride;
            __state = false;
            return false;
        }

        __state = false;
        return true;
    }

    [HarmonyPatch(typeof(Object), nameof(Object.getCategoryName))]
    [HarmonyPostfix]
    public static void getCategoryName_Postfix(ref Object __instance, ref string __result, ref bool __state)
    {
        // if state is true, we don't need to do anything
        if (__state)
            return;

        // otherwise add to cached values
        CachedCategoryNameOverrides.Add(__instance.QualifiedItemId, __result);
    }
}

class CategoryNameOverrideHooks
{
    private static readonly IAssetName ObjectsAssetName = Globals.GameContent.ParseAssetName("Data/Objects");

    internal static void InitializeEventHooks()
    {
        // flush cache when Data/Objects is reloaded
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetInvalidated;
    }

    private static void OnAssetInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Contains(ObjectsAssetName))
        {
            CategoryNameOverridePatches.CachedCategoryNameOverrides.Clear();
        }
    }
}

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
