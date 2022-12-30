/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanThisBeAttachedPatcher"/> class.</summary>
    internal SlingshotCanThisBeAttachedPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached));
    }

    #region harmony patches

    /// <summary>Patch to allow Piper equipping Slime ammo.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        __result = __result || (o is { bigCraftable.Value: false, ParentSheetIndex: Constants.SlimeIndex } &&
                                __instance.getLastFarmerToUse().HasProfession(Profession.Rascal));
    }

    #endregion harmony patches
}
