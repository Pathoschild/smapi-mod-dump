/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Patcher;
using HarmonyLib;
using StardewValley.Locations;
using weizinai.StardewValleyMod.TestMod.Framework;

namespace weizinai.StardewValleyMod.TestMod.Patches;

internal class VolcanoDungeonPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public VolcanoDungeonPatcher(ModConfig config)
    {
        VolcanoDungeonPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<VolcanoDungeon>(nameof(VolcanoDungeon.GenerateLevel)), this.GetHarmonyMethod(nameof(GenerateLevelPrefix)));
    }

    private static bool GenerateLevelPrefix(VolcanoDungeon __instance)
    {
        if (__instance.level.Value is 0 or 5 or 9) return true;
        __instance.layoutIndex.Value = config.VolcanoDungeonMap;
        return true;
    }
}