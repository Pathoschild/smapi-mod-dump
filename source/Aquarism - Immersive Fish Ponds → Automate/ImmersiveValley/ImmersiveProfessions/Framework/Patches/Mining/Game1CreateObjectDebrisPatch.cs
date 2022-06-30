/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using DaLion.Common;
using DaLion.Common.Data;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1CreateObjectDebrisPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1CreateObjectDebrisPatch()
    {
        Target = RequireMethod<Game1>(nameof(Game1.createObjectDebris),
            new[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
    [HarmonyPrefix]
    private static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer,
        GameLocation location)
    {
        try
        {
            var who = Game1.getFarmer(whichPlayer);
            if (!who.HasProfession(Profession.Gemologist) || !new SObject(objectIndex, 1).IsGemOrMineral())
                return true; // run original logic

            location.debris.Add(new(objectIndex, new(xTile * 64 + 32, yTile * 64 + 32),
                who.getStandingPosition())
            {
                itemQuality = who.GetGemologistMineralQuality()
            });

            ModDataIO.IncrementData<uint>(who, ModData.GemologistMineralsCollected.ToString());
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}