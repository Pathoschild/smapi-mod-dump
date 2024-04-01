/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BushBloomMod.Patches.Integrations {
    internal static class Automate {
        private static Config Config = null!;
        private static IMonitor Monitor = null!;

        public static void Register(IManifest manifest, IModHelper helper, IMonitor monitor, Config config) {
            Config = config;
            Monitor = monitor;
            var harmony = new Harmony(helper.ModContent.ModID);
            try {
                harmony.Patch(
                    original: AccessTools.Method("Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine:GetOutput"),
                    postfix: new HarmonyMethod(typeof(Automate), nameof(Postfix_GetOutput))
                );
                Monitor.Log($"Injected logic for Automate integration.", LogLevel.Debug);
            } catch { }
        }

        private static readonly PropertyInfo BaseMachine_Machine = AccessTools.Property("Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine:Machine");
        private static readonly FieldInfo TrackedItem_OnReduced = AccessTools.Field("Pathoschild.Stardew.Automate.TrackedItem:OnReduced");

        private static void Postfix_GetOutput(IMachine __instance, ref ITrackedStack __result) {
            try {
                if (Config.EnableAutomateIntegration) {
                    var bush = BaseMachine_Machine.GetValue(__instance) as Bush;
                    if (bush is not null) {
                        var item = (SObject)__result.Sample;
                        if (item.ParentSheetIndex == 410 || item.ParentSheetIndex == 296) {
                            var reduce = (Action<Item>?)TrackedItem_OnReduced.GetValue(__result);
                            if (reduce is not null) {
                                __result = new TrackedItem(
                                    new SObject(
                                        bush.GetShakeOffItem(),
                                        initialStack: __result.Count,
                                        quality: item.Quality
                                    ), onReduced: reduce
                                );
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"Failed to add TrackedItem via Automate integration. Message: {ex.Message}", LogLevel.Error);
            }
        }
    }
}