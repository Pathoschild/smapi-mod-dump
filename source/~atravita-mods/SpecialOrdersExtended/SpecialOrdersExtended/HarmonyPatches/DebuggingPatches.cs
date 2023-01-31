/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtraCore.Framework.ReflectionManager;

using HarmonyLib;

namespace SpecialOrdersExtended.HarmonyPatches;
internal static class DebuggingPatches
{
    internal static void Apply(Harmony harmony)
    {
#if !DEBUG
        if (!ModEntry.Config.Verbose)
        {
            return;
        }
#endif

        harmony.Patch(
            original: typeof(FishObjective).GetCachedMethod(nameof(FishObjective.OnFishCaught), ReflectionCache.FlagTypes.InstanceFlags),
            prefix: new HarmonyMethod(typeof(DebuggingPatches), nameof(PrefixFishMethod)));
    }

    private static void PrefixFishMethod(Item fish_item)
    {
        if (fish_item is null)
        {
            ModEntry.ModMonitor.Log("null fish?", LogLevel.Info);
            return;
        }

        ModEntry.ModMonitor.Log($"Checking fish {fish_item.Name} with context tags: {string.Join(", ", fish_item.GetContextTags())}", LogLevel.Info);
    }
}
