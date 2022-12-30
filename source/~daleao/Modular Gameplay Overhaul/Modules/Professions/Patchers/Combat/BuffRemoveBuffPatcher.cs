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

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffRemoveBuffPatcher : HarmonyPatcher
{
    private static readonly int PiperBuffId = (Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffRemoveBuffPatcher"/> class.</summary>
    internal BuffRemoveBuffPatcher()
    {
        this.Target = this.RequireMethod<Buff>(nameof(Buff.removeBuff));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void BuffRemoveBuffPrefix(Buff __instance)
    {
        if (__instance.which == PiperBuffId && __instance.millisecondsDuration <= 0)
        {
            Array.Clear(ProfessionsModule.State.PiperBuffs, 0, 12);
        }
    }

    #endregion harmony patches
}
