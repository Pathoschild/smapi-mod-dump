/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHitWithHoePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CropHitWithHoePatch()
    {
        Target = RequireMethod<Crop>(nameof(Crop.hitWithHoe));
    }

    #region harmony patches

    /// <summary>Apply Botanist/Ecologist perk to wild ginger.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CropHitWithHoeTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: SetGingerQuality(@object);
        /// Between: @object = new SObject(829, 1);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(CropHitWithHoePatch).RequireMethod(nameof(AddGingerQuality)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while apply Ecologist/Botanist perk to hoed ginger.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutine

    private static SObject AddGingerQuality(SObject ginger)
    {
        if (!ModEntry.Config.ProfessionalForagingInGingerIsland || !Game1.player.professions.Contains(Farmer.botanist)) return ginger;

        ginger.Quality = ModEntry.ProfessionsAPI is null
            ? SObject.bestQuality
            : ModEntry.ProfessionsAPI.GetEcologistForageQuality(Game1.player);
        return ginger;
    }

    #endregion injected subroutine
}