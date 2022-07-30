/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCanThisBeAttachedPatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached));
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        if (o is null || o.bigCraftable.Value) return;

        switch (o.ParentSheetIndex)
        {
            case 909: // radioactive ore
            case 766 when __instance.getLastFarmerToUse().HasProfession(Profession.Piper): // slime
                __result = true;
                break;
        }
    }

    #endregion harmony patches
}