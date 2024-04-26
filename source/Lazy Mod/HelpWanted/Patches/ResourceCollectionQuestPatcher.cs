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

public class ResourceCollectionQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static bool hasLoadQuestInfo;

    public ResourceCollectionQuestPatcher(ModConfig config)
    {
        ResourceCollectionQuestPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<ResourceCollectionQuest>(nameof(ResourceCollectionQuest.loadQuestInfo)),
            prefix: GetHarmonyMethod(nameof(LoadQuestInfoPrefix)),
            postfix: GetHarmonyMethod(nameof(LoadQuestInfoPostfix))
        );
    }
    
    private static bool LoadQuestInfoPrefix(ResourceCollectionQuest __instance)
    {
        if (__instance.target.Value is not null && __instance.ItemId.Value is not null)
        {
            hasLoadQuestInfo = false;
            return false;
        }

        hasLoadQuestInfo = true;
        return true;
    }
    
    private static void LoadQuestInfoPostfix(ref NetInt ___reward, ref NetDescriptionElementList ___parts)
    {
        if (hasLoadQuestInfo) return;
        ___reward.Value = (int)(___reward.Value * config.ResourceCollectionRewardMultiplier);
        ___parts[^2].substitutions = new List<object> { ___reward.Value };
    }
}