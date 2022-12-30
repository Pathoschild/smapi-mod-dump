/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanUnderstandDwarvesSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanUnderstandDwarvesSetterPatcher"/> class.</summary>
    internal FarmerCanUnderstandDwarvesSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.canUnderstandDwarves));
    }

    #region harmony patches

    /// <summary>Try to patch in Clint's event.</summary>
    [HarmonyPostfix]
    private static void FarmerStaminaSetterPostfix()
    {
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
    }

    #endregion harmony patches
}
