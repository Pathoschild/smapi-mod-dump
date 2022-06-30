/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AxeBeginUsingPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal AxeBeginUsingPatch()
    {
        Target = RequireMethod<Axe>("beginUsing");
    }

    #region harmony patches

    /// <summary>Enable Axe power level increase.</summary>
    [HarmonyPrefix]
    private static bool AxeBeginUsingPrefix(Tool __instance, Farmer who)
    {
        if (!ModEntry.Config.AxeConfig.EnableCharging ||
            ModEntry.Config.RequireModkey && !ModEntry.Config.Modkey.IsDown() ||
            __instance.UpgradeLevel < (int)ModEntry.Config.AxeConfig.RequiredUpgradeForCharging)
            return true; // run original logic

        who.Halt();
        __instance.Update(who.FacingDirection, 0, who);
        switch (who.FacingDirection)
        {
            case 0:
                who.FarmerSprite.setCurrentFrame(176);
                __instance.Update(0, 0, who);
                break;

            case 1:
                who.FarmerSprite.setCurrentFrame(168);
                __instance.Update(1, 0, who);
                break;

            case 2:
                who.FarmerSprite.setCurrentFrame(160);
                __instance.Update(2, 0, who);
                break;

            case 3:
                who.FarmerSprite.setCurrentFrame(184);
                __instance.Update(3, 0, who);
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}