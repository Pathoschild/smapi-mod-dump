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
internal sealed class SlingshotGetAmmoCollisionSoundPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetAmmoCollisionSoundPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotGetAmmoCollisionSoundPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetAmmoCollisionSound));
    }

    #region harmony patches

    /// <summary>Patch to set Rascal Slime ammo CollisionSound.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAmmoCollisionSoundPostfix(Slingshot __instance, ref string __result, SObject ammunition)
    {
        if (ammunition.QualifiedItemId == QualifiedObjectIds.Slime)
        {
            __result = "slimedead";
        }
    }

    #endregion harmony patches
}
