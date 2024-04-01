/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static CrabPotQuality.CrabPotQualityLogic;
using StardewObject = StardewValley.Object;

namespace CrabPotQuality
{
    internal class Patcher
    {
        private static CrabPotQuality mod;

        public static void PatchAll(CrabPotQuality mod, CrabPotQualityConfig config)
        {
            Config = config;
            Patcher.mod = mod;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(UpdateHeldItem)));

            harmony.Patch(
               original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction)),
               transpiler: new HarmonyMethod(typeof(Patcher), nameof(FixQuantityOverrideBug)));
        }

        public static int GetHeldObjectQuantity(CrabPot pot)
        {
            if (pot.heldObject.Value != null)
            {
                return pot.heldObject.Value.Stack;
            }
            else
            {
                return 1;
            }
        }

        public static IEnumerable<CodeInstruction> FixQuantityOverrideBug(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                for (int i = 0; i < instructionsList.Count; i++)
                {
                    if (instructionsList[i].opcode == OpCodes.Ldstr
                        && (string)instructionsList[i].operand == "Book_Crabbing")
                    {
                        for (int j = i; j >= 0; j--)
                        {
                            if (instructionsList[j].opcode == OpCodes.Ldc_I4_1
                                && j + 2 < instructionsList.Count
                                && instructionsList[j + 2].opcode == OpCodes.Ldsfld
                                && (FieldInfo)instructionsList[j + 2].operand == AccessTools.Field(typeof(Game1), nameof(Game1.uniqueIDForThisGame)))
                            {
                                // insert 'this'
                                instructionsList[j].opcode = OpCodes.Ldarg_0;
                                instructionsList[j].operand = null;

                                instructionsList.Insert(j + 1, new CodeInstruction(OpCodes.Call, typeof(Patcher).GetMethod(nameof(GetHeldObjectQuantity))));
                                break;
                            }
                        }

                        break;
                    }
                }

                return instructionsList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        public static void UpdateHeldItem(CrabPot __instance)
        {
            if (__instance.heldObject.Value != null)
            {
                // do quality calculation and assignment in two steps in case the object gets replaced with a special item like a rainbow shell
                int quality = DeterminePotQuality(__instance);

                if (!Config.DisableAllQualityEffects)
                {
                    __instance.heldObject.Value.Quality = quality;
                }
            }
        }

        private static int DeterminePotQuality(CrabPot pot)
        {
            Farmer farmer = Game1.getFarmer(pot.owner.Value) ?? Game1.MasterPlayer; // set to host if owner somehow doesn't exist

            // if luremaster, ignore all bait effects (since you can't technically put bait into a crabpot as luremaster, so this must be a bugged pot from an automation mod)
            if (farmer.professions.Contains(Farmer.baitmaster))
            {
                return Config.LuremasterPerkForcesIridiumQuality && !IsTrash(pot.heldObject.Value) ? StardewObject.bestQuality : StardewObject.lowQuality;
            }

            if (!Config.DisableAllNonQualityEffects && !pot.heldObject.Value.modData.ContainsKey(chancesAlreadyCheckedKey))
            {
                if (pot.bait.Value != null && pot.heldObject.Value.Stack == 2 && pot.bait.Value.QualifiedItemId == wildBaitQID)
                {
                    // revert base game chance
                    pot.heldObject.Value.Stack = 1;
                }

                if (ReplaceWithSpecialItem(pot))
                {
                    return StardewObject.lowQuality;
                }

                float doubleItemAmountChance = GetBaitDoubleItemAmountChance(pot);

                Random rD = Utility.CreateDaySaveRandom(pot.TileLocation.X * 77f + pot.TileLocation.Y * 77f, Game1.stats.DaysPlayed * 77f);
                if (pot.heldObject.Value.Stack <= 1 && rD.NextDouble() < doubleItemAmountChance)
                {
                    pot.heldObject.Value.Stack = 2;
                }

                // this needs to be done after ReplaceWithSpecialItem, so it's added to the new item
                pot.heldObject.Value.modData[chancesAlreadyCheckedKey] = "true";
            }

            if (IsTrash(pot.heldObject.Value))
            {
                return StardewObject.lowQuality;
            }

            float qualityMultiplier = GetBaitQualityModifier(pot);

            // let quality modifier override iridium perks
            if (qualityMultiplier <= 0f)
            {
                return StardewObject.lowQuality;
            }

            if (Config.MarinerPerkForcesIridiumQuality && farmer.professions.Contains(Farmer.mariner))
            {
                return StardewObject.bestQuality;
            }

            Random r = Utility.CreateDaySaveRandom(pot.TileLocation.X, pot.TileLocation.Y * 777f);
            // foraging formula
            if (r.NextDouble() < farmer.FishingLevel / 30f * qualityMultiplier)
            {
                return StardewObject.highQuality;
            }
            else if (r.NextDouble() < farmer.FishingLevel / 15f * qualityMultiplier)
            {
                return StardewObject.medQuality;
            }
            else
            {
                return StardewObject.lowQuality;
            }
        }
    }
}