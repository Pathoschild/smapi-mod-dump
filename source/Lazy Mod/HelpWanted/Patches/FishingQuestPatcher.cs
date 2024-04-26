/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Netcode;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class FishingQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static bool hasLoadQuestInfo;

    public FishingQuestPatcher(ModConfig config)
    {
        FishingQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<FishingQuest>(nameof(FishingQuest.loadQuestInfo)),
            prefix: GetHarmonyMethod(nameof(LoadQuestInfoPrefix)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }
    
    private static bool LoadQuestInfoPrefix(FishingQuest __instance)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null)
        {
            hasLoadQuestInfo = false;
            return false;
        }

        hasLoadQuestInfo = true;
        return true;
    }

    private static void LoadQuestInfoPostfix(NetInt ___reward, ref NetDescriptionElementList ___parts, NetString ___target)
    {
        if (hasLoadQuestInfo) return;
        
        ___reward.Value = (int)(___reward.Value * config.FishingRewardMultiplier);
        ___parts[^2].substitutions = new List<object> { ___reward.Value };
        if (___target.Value is "Willy") ___parts[^3].substitutions[0] = ___reward.Value;
    }
}