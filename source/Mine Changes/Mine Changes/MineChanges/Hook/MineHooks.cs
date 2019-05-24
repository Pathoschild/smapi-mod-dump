
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Mine_Changes.MineChanges.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace Mine_Changes.MineChanges.Hook
{
    public class MineHooks
    {
        #region Postfixes
        public static void chooseStoneTypePostfix(MineShaft __instance, ref StardewValley.Object __result, double chanceForPurpleStone, double chanceForMysticStone, double gemStoneChance, Vector2 tile)
        {
            if(Mod.config.stoneReplacements != null && Mod.config.stoneReplacements.Count > 0)
            {
                foreach(StoneReplace sr in Mod.config.stoneReplacements){
                    __result = sr.tryAndReplaceObject(__result, __instance, tile);
                }
            }
        }
        public static void getRandomItemForThisLevelPostfix(MineShaft __instance, ref StardewValley.Object __result, int level)
        {
            if (Mod.config.itemReplacements != null && Mod.config.itemReplacements.Count > 0)
            {
                foreach (ItemReplace ir in Mod.config.itemReplacements)
                {
                    __result = ir.tryAndReplaceObject(__result, __instance, level);
                }
            }
        }
        #endregion

        #region Transpliers
        public static void patchCode(List<CodeInstruction> ans, int idx, OpCode code,  object value)
        {
            if (idx < ans.Count && idx >= 0 && ans[idx].opcode == code)
            {
                ans[idx].operand = value;
            }
        }

        public static void patchCodeChangeOP(List<CodeInstruction> ans, int idx, OpCode code,OpCode newCode, object value)
        {
            if (idx < ans.Count && idx >= 0 && ans[idx].opcode == code)
            {
                ans[idx] = new CodeInstruction(newCode, value);
            }
        }

        public static IEnumerable<CodeInstruction> StoneChangeTranspiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            List<CodeInstruction> ans = (List<CodeInstruction>)insns;
            Mod.instance.Monitor.Log("Changing: 63 = " + Mod.config.stoneChances.CopperChanceUpToLevel40);
            patchCode(ans, 63, OpCodes.Ldc_R8, Mod.config.stoneChances.CopperChanceUpToLevel40);
            Mod.instance.Monitor.Log("Applied change: 63 = " + ans[63].operand);
            patchCode(ans, 97, OpCodes.Ldc_R8, Mod.config.stoneChances.IronChanceFromLevel40To80);
            patchCode(ans, 166, OpCodes.Ldc_R8, Mod.config.stoneChances.GoldChanceFromLevel80To120);
            patchCode(ans, 208, OpCodes.Ldc_R8, Mod.config.stoneChances.SkullMineBaseOreChance);
            patchCode(ans, 214, OpCodes.Ldc_R8, Mod.config.stoneChances.SkullMineBaseOreChancePerLevel);
            patchCode(ans, 277, OpCodes.Ldc_R8, Mod.config.stoneChances.IridiumBaseChance);
            patchCode(ans, 282, OpCodes.Ldc_R8, Mod.config.stoneChances.GoldChanceSkullMine);
            patchCode(ans, 288, OpCodes.Ldc_R8, Mod.config.stoneChances.GoldChanceSkullMinePerLevelBoost);
            patchCode(ans, 292, OpCodes.Ldc_R8, Mod.config.stoneChances.SkullMineMaxIronChance);
            patchCode(ans, 299, OpCodes.Ldc_R8, Mod.config.stoneChances.IronChanceSkullMinePerLevelBoost);
            patchCode(ans, 417, OpCodes.Ldc_R8, Mod.config.stoneChances.GemChancePerLevelDenominator);
            patchCode(ans, 447, OpCodes.Ldc_R8, Mod.config.stoneChances.ChanceForPurpleStonePerMiningLevel);
            patchCode(ans, 472, OpCodes.Ldc_R8, Mod.config.stoneChances.ChanceForMysthicStonePerMiningLevel);
            

            return ans;
        }

        public static IEnumerable<CodeInstruction> MapGenTranspiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            List<CodeInstruction> ans = (List<CodeInstruction>)insns;
            //patchCode(ans, 23 ,OpCodes.Ldc_I4_S,Mod.config.mapLayout.minStonePerDenominator);
            // try this first
            patchCodeChangeOP(ans, 23, OpCodes.Ldc_I4_S, OpCodes.Ldc_I4, Mod.config.mapLayout.minStonePerDenominator);
            patchCodeChangeOP(ans, 24, OpCodes.Ldc_I4_S, OpCodes.Ldc_I4, Mod.config.mapLayout.maxStonePerDenominator);
            patchCode(ans, 27, OpCodes.Ldc_R8, Mod.config.mapLayout.stoneDenominator);

            patchCode(ans, 30, OpCodes.Ldc_R8, Mod.config.mapLayout.minMonsterChance);
            patchCode(ans, 33, OpCodes.Ldc_I4, Mod.config.mapLayout.maxRandomMonsterPerDenominator);
            patchCode(ans, 36, OpCodes.Ldc_R8, Mod.config.mapLayout.maxRandomMonsterDenominator);
            patchCode(ans, 40, OpCodes.Ldc_R8, Mod.config.mapLayout.itemOnFloorChance);
            patchCode(ans, 42, OpCodes.Ldc_R8, Mod.config.mapLayout.gemStoneChanceDoubled);


            patchCode(ans, 81, OpCodes.Ldc_R8, Mod.config.mapLayout.barrelLuckMultiplier);
            patchCode(ans, 264, OpCodes.Ldc_R8, Mod.config.mapLayout.chanceForIceCrystals);
            patchCode(ans, 296, OpCodes.Ldc_R8, Mod.config.mapLayout.chanceForMushrooms);

            patchCode(ans, 501, OpCodes.Ldc_R8, Mod.config.mapLayout.chanceMonsterHasSpecialItem);
            patchCode(ans, 547, OpCodes.Ldc_R8, Mod.config.mapLayout.chanceForBigBolder);
            
            return ans;
        }

        public static void addTrans(Mod mod, HarmonyInstance hInstance)
        {
            MethodInfo chooseStone = typeof(StardewValley.Locations.MineShaft).GetMethod("chooseStoneType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (chooseStone == null)
            {
                mod.Monitor.Log("Could not find chooseStoneType", LogLevel.Error);
                return;
            }
            MethodInfo trans = typeof(MineHooks).GetMethod("StoneChangeTranspiler");
            if (trans == null)
            {
                mod.Monitor.Log("Could not find the chooseStoneType patcher", LogLevel.Error);
                return;
            }
            MethodInfo post = typeof(MineHooks).GetMethod("chooseStoneTypePostfix");
            if (post == null)
            {
                mod.Monitor.Log("Could not find the chooseStoneType postfix", LogLevel.Error);
                return;
            }
            hInstance.Patch(chooseStone, null, new HarmonyMethod(post), new HarmonyMethod(trans));

            MethodInfo populate = typeof(StardewValley.Locations.MineShaft).GetMethod("populateLevel", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (populate == null)
            {
                mod.Monitor.Log("Could not find populateLevel", LogLevel.Error);
                return;
            }
            trans = typeof(MineHooks).GetMethod("MapGenTranspiler");
            if (trans == null)
            {
                mod.Monitor.Log("Could not find the populateLevel patcher", LogLevel.Error);
                return;
            }
            hInstance.Patch(populate, null, null, new HarmonyMethod(trans));

            MethodInfo randItem = typeof(StardewValley.Locations.MineShaft).GetMethod("getRandomItemForThisLevel", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (randItem == null)
            {
                mod.Monitor.Log("Could not find getRandomItemForThisLevel", LogLevel.Error);
                return;
            }
            post = typeof(MineHooks).GetMethod("getRandomItemForThisLevelPostfix");
            if (post == null)
            {
                mod.Monitor.Log("Could not find the getRandomItemForThisLevel postfix", LogLevel.Error);
                return;
            }
            hInstance.Patch(randItem, null, new HarmonyMethod(post), null);
        }

        #endregion
    }
}
