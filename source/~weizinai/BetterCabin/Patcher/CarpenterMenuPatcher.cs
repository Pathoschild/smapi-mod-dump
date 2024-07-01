/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class CarpenterMenuPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public CarpenterMenuPatcher(ModConfig config)
    {
        CarpenterMenuPatcher.config = config;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<CarpenterMenu>(nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
            this.GetHarmonyMethod(nameof(ReturnToCarpentryMenuAfterSuccessfulBuildPrefix))
        );
    }

    private static bool ReturnToCarpentryMenuAfterSuccessfulBuildPrefix(CarpenterMenu __instance)
    {
        if (!config.BuildCabinContinually) return true;
        
        // __instance.isCabin 无法判断自定义小屋
        // __instance.GetIndoors() is Cabin 此时室内还未生成，无法判断是否为小屋
        if (__instance.Blueprint.Data.IndoorMapType == "StardewValley.Locations.Cabin" && __instance.CanBuildCurrentBlueprint())
        {
            __instance.freeze = false;
            return false;
        }
        
        return true;
    }
}