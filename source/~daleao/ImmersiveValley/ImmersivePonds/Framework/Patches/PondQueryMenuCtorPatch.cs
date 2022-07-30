/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Data;
using Common.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuCtorPatch()
    {
        Target = RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Handle invalid data on menu open.</summary>
    [HarmonyPrefix]
    private static void PondQueryMenuCtorPrefix(FishPond fish_pond)
    {
        try
        {
            ModDataIO.ReadFrom(fish_pond, "FishQualities").ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FishQualities data is invalid. {ex}\nThe data will be reset");
            ModDataIO.WriteTo(fish_pond, "FishQualities", $"{fish_pond.FishCount},0,0,0");
            ModDataIO.WriteTo(fish_pond, "FamilyQualities", null);
            ModDataIO.WriteTo(fish_pond, "FamilyLivingHere", null);
        }

        try
        {
            ModDataIO.ReadFrom(fish_pond, "FamilyQualities").ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FamilyQuality data is invalid. {ex}\nThe data will be reset");
            ModDataIO.WriteTo(fish_pond, "FishQualities", $"{fish_pond.FishCount},0,0,0");
            ModDataIO.WriteTo(fish_pond, "FamilyQualities", null);
            ModDataIO.WriteTo(fish_pond, "FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}