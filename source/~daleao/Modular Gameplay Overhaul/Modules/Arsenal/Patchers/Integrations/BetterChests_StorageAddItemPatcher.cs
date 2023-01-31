/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("furyx639.BetterChests")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class StorageAddItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="StorageAddItemPatcher"/> class.</summary>
    internal StorageAddItemPatcher()
    {
        this.Target = "StardewMods.BetterChests.StorageHandlers.BaseStorage"
            .ToType()
            .RequireMethod("AddItem");
    }

    #region harmony patches

    /// <summary>Prevent depositing Dark Sword.</summary>
    [HarmonyPrefix]
    private static bool StorageAddItemPrefix(ref Item __result, Item item)
    {
        if (item is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex })
        {
            return true; // run original logic
        }

        __result = item;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
