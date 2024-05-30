/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanThisBeAttachedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotCanThisBeAttachedPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached), [typeof(SObject), typeof(int)]);
    }

    #region harmony patches

    /// <summary>Patch to allow Rascal special ammo.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        if (__result || o is null || o.bigCraftable.Value)
        {
            return;
        }

        var lastUser = __instance.getLastFarmerToUse();
        __result = (o.QualifiedItemId == QualifiedObjectIds.Slime && lastUser.HasProfession(Profession.Rascal)) ||
                   (o.QualifiedItemId == QualifiedObjectIds.MonsterMusk && lastUser.HasProfession(Profession.Rascal, true));
    }

    #endregion harmony patches
}
