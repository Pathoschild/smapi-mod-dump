/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley.Menus;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Events;

internal static class GlobalCarpenterChanger {
    
    private const string HarmonySuffix = ".GlobalCarpenterChanger";
    
    private static Harmony? _harmony;
    private static readonly IList<string> ExcludedBluePrintNames = new List<string>();

    public static void AddExcludedBluePrintNames(IEnumerable<string> excludedBluePrintNames) {
        if (_harmony == null) {
            _harmony = new Harmony(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony.Patch(
                original: AccessTools.Constructor(
                    typeof(CarpenterMenu),
            new[] {
                        typeof(bool),
                    }
                ),
                postfix: new HarmonyMethod(typeof(GlobalCarpenterChanger), nameof(ChangeBluePrints))
            );
        }
        foreach (var excludedBluePrintName in excludedBluePrintNames) {
            ExcludedBluePrintNames.Add(excludedBluePrintName);
        }
    }

    public static void ChangeBluePrints(CarpenterMenu __instance) {
        var blueprintsField = Traverse.Create(__instance).Field<List<BluePrint>>("blueprints");
        var blueprints = blueprintsField.Value;
        var newBlueprints = blueprints.Where(b => !ExcludedBluePrintNames.Contains(b.name)).ToList();
        blueprintsField.Value = newBlueprints;

        __instance.setNewActiveBlueprint();
    }

    public static void RemoveExcludedBluePrintNames(IEnumerable<string> excludedBluePrintNames) {
        foreach (var excludedBluePrintName in excludedBluePrintNames) {
            ExcludedBluePrintNames.Remove(excludedBluePrintName);
        }

        if (ExcludedBluePrintNames.Count == 0) {
            _harmony?.UnpatchAll(ChallengerMod.Instance.ModManifest.UniqueID + HarmonySuffix);
            _harmony = null;
        }
    }
}