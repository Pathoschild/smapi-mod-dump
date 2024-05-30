/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System.Reflection.Emit;

namespace FairyFix
{
    internal static class Patches
    {
        internal static string ModDataKey => $"{ModEntry.Instance.ModManifest.UniqueID}/NoFairy";

        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(FairyEvent), "ChooseCrop"),
                prefix: new(typeof(Patches), nameof(FairyEvent_ChooseCrop_Prefix)) //Could transpile, but based on the code, I want to try my hand at a re-write
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FairyEvent), nameof(FairyEvent.makeChangesToLocation)),
                transpiler: new(typeof(Patches), nameof(FairyEvent_MakeChangesToLocation_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "OnNewSeason"),
                postfix: new(typeof(Patches), nameof(Game1_OnNewSeason_Postfix))
            );
        }

        internal static bool FairyEvent_ChooseCrop_Prefix(FairyEvent __instance, Farm ___f, ref Vector2 __result)
        {
            try
            {
                Farm farm = ___f;
                FairyEvent evt = __instance;

                List<Vector2> crops = [];
                foreach (var item in farm.terrainFeatures.Pairs)
                {
                    if (item.Value is not HoeDirt soil || soil.modData.ContainsKey(ModDataKey) || soil.crop is not Crop crop || crop.modData.ContainsKey(ModDataKey) || (crop.dead.Value && !ModEntry.Config.ReviveDeadCrops) || crop.isWildSeedCrop() || crop.currentPhase.Value >= crop.phaseDays.Count - 1)
                        continue;
                    crops.Add(item.Key);
                }
                crops = new(crops.OrderBy(x => x.X).ThenBy(x => x.Y)); //Might be unnecessary \\Nah, keepin it
                __result = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed).ChooseFrom(crops);
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"An error occured while patching {nameof(FairyEvent)}.ChooseCrop", LogLevel.Error);
                ModEntry.Instance.Monitor.Log($"[{nameof(FairyEvent_ChooseCrop_Prefix)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        internal static IEnumerable<CodeInstruction> FairyEvent_MakeChangesToLocation_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            matcher.Start().MatchStartForward([ //Start insert custom avoid fairy growth
                new(OpCodes.Ldloca_S),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Conv_R4),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Conv_R4),
                new(OpCodes.Call)
            ]).MatchStartForward([
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Stloc_1)
            ]).CreateLabel(out var l1).MatchStartBackwards([
                new(OpCodes.Ldloc_S),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HoeDirt), nameof(HoeDirt.crop))),
                new(OpCodes.Brfalse_S),
                new(OpCodes.Ldloc_S)
            ]).InsertAndAdvance([
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(shouldAvoidFairyGrowth))),
                new(OpCodes.Brtrue_S, l1)
            ]).MatchStartForward([ //Start insert revive crop
                new(OpCodes.Ldloc_S),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HoeDirt), nameof(HoeDirt.crop))),
                new(OpCodes.Callvirt, AccessTools.Method(typeof(Crop), nameof(Crop.growCompletely)))
            ]).CreateLabel(out var l2).InsertAndAdvance([
                new(OpCodes.Ldsfld, AccessTools.Field(typeof(ModEntry), nameof(ModEntry.Config))),
                new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Config), nameof(Config.ReviveDeadCrops))),
                new(OpCodes.Brfalse_S, l2),
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HoeDirt), nameof(HoeDirt.crop))),
                new(OpCodes.Ldfld, AccessTools.Field(typeof(Crop), nameof(Crop.dead))),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(NetBool), nameof(NetBool.Value)))
            ]);

            return matcher.Instructions();
        }

        internal static void Game1_OnNewSeason_Postfix()
        {
            if (!ModEntry.Config.ResetOnSeasonChange)
                return;
            foreach (var tf in Game1.getFarm().terrainFeatures.Values)
            {
                if (tf is not HoeDirt hd)
                    continue;
                hd.crop?.modData.Remove(ModDataKey);
                hd.modData.Remove(ModDataKey);
            }
        }

        private static bool shouldAvoidFairyGrowth(HoeDirt dirt) => dirt.modData.ContainsKey(ModDataKey) || (dirt.crop?.modData.ContainsKey(ModDataKey) ?? false);
    }
}
