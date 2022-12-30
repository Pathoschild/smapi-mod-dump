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

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BluePrintCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BluePrintCtorPatcher"/> class.</summary>
    internal BluePrintCtorPatcher()
    {
        this.Target = this.RequireConstructor<BluePrint>(typeof(string));
    }

    #region harmony patches

    /// <summary>Remove Dragon Tooth from Obelisk blueprint.</summary>
    [HarmonyPostfix]
    private static void BluePrintCtorPostfix(BluePrint __instance)
    {
        if (!ArsenalModule.Config.DwarvishCrafting || __instance.name != "Island Obelisk" ||
            !__instance.itemsRequired.Remove(Constants.DragonToothIndex))
        {
            return;
        }

        __instance.itemsRequired[Constants.PineappleIndex] = 10;
        __instance.itemsRequired[Constants.MangoIndex] = 10;
        __instance.itemsRequired[Constants.RadioactiveBarIndex] = 5;
        __instance.itemsRequired.Remove(SObject.iridiumBar);
    }

    #endregion harmony patches
}
