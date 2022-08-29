/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Attributes;
using Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftIngredient : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuIsValidCraftIngredient()
    {
        Target = "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("IsValidCraftIngredient");
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (item.IsHeroSoul()) __result = true;
    }

    #endregion harmony patches
}