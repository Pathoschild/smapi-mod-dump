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
internal sealed class SlingshotGetAmmoDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetAmmoDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotGetAmmoDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetAmmoDamage));
    }

    #region harmony patches

    /// <summary>Patch to set Rascal Slime ammo damage.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAmmoDamagePostfix(Slingshot __instance, ref int __result, SObject ammunition)
    {
        if (ammunition.QualifiedItemId != QualifiedObjectIds.Slime)
        {
            return;
        }

        var user = __instance.lastUser;
        if (!user.HasProfession(Profession.Piper))
        {
            __result = 1;
            return;
        }

        __result = user.CountRaisedSlimes();
        if (!user.HasProfession(Profession.Piper, true))
        {
            __result /= 2;
        }
    }

    #endregion harmony patches
}
