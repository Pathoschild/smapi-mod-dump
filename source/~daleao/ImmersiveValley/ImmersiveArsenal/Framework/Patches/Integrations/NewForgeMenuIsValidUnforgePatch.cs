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
using Common.Integrations.SpaceCore;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidUnforgePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuIsValidUnforgePatch()
    {
        Target = "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("IsValidUnforge");
    }

    #region harmony patches

    /// <summary>Allow unforge Holy Blade.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidUnforgePostfix(IClickableMenu __instance, ref bool __result)
    {
        var item = ExtendedSpaceCoreAPI.GetNewForgeMenuLeftIngredientSpot.Value(__instance).item;
        __result = item switch
        {
            Slingshot slingshot when slingshot.GetTotalForgeLevels() > 0 => true,
            MeleeWeapon { InitialParentTileIndex: Constants.HOLY_BLADE_INDEX_I } => true,
            _ => __result
        };
    }

    #endregion harmony patches
}