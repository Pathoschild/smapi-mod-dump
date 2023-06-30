/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;
using DaLion.Shared.Attributes;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
// Deprecated
internal sealed class ToolGetMaxForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetMaxForgesPatcher"/> class.</summary>
    internal ToolGetMaxForgesPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.GetMaxForges));
    }

    #region harmony patches

    /// <summary>Custom forge slots for slingshots.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPrefix(Tool __instance, ref int __result)
    {
        if (__instance is not Slingshot slingshot || !SlingshotsModule.Config.EnableEnchantments ||
            ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is not null)
        {
            return true; // run original logic
        }

        if (!SlingshotsModule.Config.EnableRebalance)
        {
            __result = 3;
            return false; // don't run original logic
        }

        try
        {
            __result = slingshot.InitialParentTileIndex switch
            {
                ItemIDs.BasicSlingshot => 1,
                ItemIDs.MasterSlingshot => 2,
                ItemIDs.GalaxySlingshot => 3,
                ItemIDs.InfinitySlingshot when WeaponsModule.ShouldEnable && WeaponsModule.Config.InfinityPlusOne => 4,
                ItemIDs.InfinitySlingshot => 3,
                _ => 0,
            };

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
