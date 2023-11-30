/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponRecalculateAppliedForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponRecalculateAppliedForgesPatcher"/> class.</summary>
    internal MeleeWeaponRecalculateAppliedForgesPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.RecalculateAppliedForges));
    }

    #region harmony patches

    /// <summary>Apply custom stats if necessary.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponRecalculateAppliedForgedPrefix(MeleeWeapon __instance, bool force)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul)
        {
            return true; // run original logic
        }

        if (__instance.enchantments.Count == 0 && !force)
        {
            return false; // don't run original logic
        }

        try
        {
            __instance.RefreshStats();
            __instance.description = null;
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
