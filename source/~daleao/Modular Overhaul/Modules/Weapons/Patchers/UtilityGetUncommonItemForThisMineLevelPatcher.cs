/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetUncommonItemForThisMineLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetUncommonItemForThisMineLevelPatcher"/> class.</summary>
    internal UtilityGetUncommonItemForThisMineLevelPatcher()
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getUncommonItemForThisMineLevel));
    }

    #region harmony patches

    /// <summary>Randomize Mine drops.</summary>
    [HarmonyPostfix]
    private static void UtilityGetUncommonItemForThisMineLevelPostfix(Item __result)
    {
        if (WeaponsModule.Config.EnableRebalance && __result is MeleeWeapon weapon)
        {
            weapon.RandomizeDamage();
        }
    }

    #endregion harmony patches
}
