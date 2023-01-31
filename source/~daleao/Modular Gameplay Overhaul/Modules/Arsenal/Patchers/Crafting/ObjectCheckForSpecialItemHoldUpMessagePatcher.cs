/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForSpecialItemHoldUpMessagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForSpecialItemHoldUpMessagePatcher"/> class.</summary>
    internal ObjectCheckForSpecialItemHoldUpMessagePatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add Dwarvish Blueprint obtain message.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForSpecialItemHoldUpPrefix(SObject? __instance, ref string? __result)
    {
        if (__instance is null || !Globals.DwarvishBlueprintIndex.HasValue ||
            __instance.ParentSheetIndex != Globals.DwarvishBlueprintIndex.Value)
        {
            return;
        }

        var found = Game1.player.Read(DataFields.BlueprintsFound).ParseList<int>();
        if (found.Count == 1)
        {
            var type = ((WeaponType)new MeleeWeapon(found[0]).type.Value).ToStringFast();
            if (type.Contains("Sword"))
            {
                type = type.SplitCamelCase()[1];
            }

            __result = I18n.Get("blueprint.found.first", new { type });
        }
        else
        {
            __result = I18n.Get("blueprint.found.local");
        }
    }

    #endregion harmony patches
}
