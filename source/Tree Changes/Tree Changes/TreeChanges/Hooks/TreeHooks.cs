using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using TreeChanges.TreeChanges.Config;

namespace TreeChanges.TreeChanges.Hooks
{
    public class TreeHooks
    {
        #region Transpliers
        public static void patchCode(List<CodeInstruction> ans, int idx, OpCode code, object value)
        {
            if (idx < ans.Count && idx >= 0 && ans[idx].opcode == code)
            {
                ans[idx].operand = value;
            }
        }

        public static void patchCodeChangeOP(List<CodeInstruction> ans, int idx, OpCode code, OpCode newCode, object value)
        {
            if (idx < ans.Count && idx >= 0 && ans[idx].opcode == code)
            {
                ans[idx] = new CodeInstruction(newCode, value);
            }
        }

        public static IEnumerable<CodeInstruction> dayUpdateTranspiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            List<CodeInstruction> ans = (List<CodeInstruction>)insns;
            patchCode(ans, 131, OpCodes.Ldc_R8, Mod.config.treeParameters.treeGrowthChance);
            patchCode(ans, 186, OpCodes.Ldc_R8, Mod.config.treeParameters.treeSpreadChance);
            patchCode(ans, 279, OpCodes.Ldc_R8, Mod.config.treeParameters.treeHasSeedChance);
            return ans;
        }

        public static IEnumerable<CodeInstruction> shakeTranspiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            List<CodeInstruction> ans = (List<CodeInstruction>)insns;
            MethodInfo trans = typeof(TreeHooks).GetMethod("seedGenReplacer");
            if (trans == null)
            {
                Mod.instance.Monitor.Log("Could not find the seed Replacer", LogLevel.Error);
                return ans;
            }
            patchCode(ans, 237, OpCodes.Call, trans);
            return ans;
        }

        public static void addTrans(Mod mod, HarmonyInstance hInstance)
        {
            MethodInfo dayUpdate = typeof(StardewValley.TerrainFeatures.Tree).GetMethod("dayUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (dayUpdate == null)
            {
                mod.Monitor.Log("Could not find dayUpdate", LogLevel.Error);
                return;
            }
            MethodInfo trans = typeof(TreeHooks).GetMethod("dayUpdateTranspiler");
            if (trans == null)
            {
                mod.Monitor.Log("Could not find the dayUpdate patcher", LogLevel.Error);
                return;
            }
            hInstance.Patch(dayUpdate, null, null, new HarmonyMethod(trans));

            MethodInfo shake = typeof(StardewValley.TerrainFeatures.Tree).GetMethod("shake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (shake == null)
            {
                mod.Monitor.Log("Could not find shake", LogLevel.Error);
                return;
            }
            trans = typeof(TreeHooks).GetMethod("shakeTranspiler");
            if (trans == null)
            {
                mod.Monitor.Log("Could not find the shake patcher", LogLevel.Error);
                return;
            }
            hInstance.Patch(shake, null, null, new HarmonyMethod(trans));
        }
        #endregion

        public static void seedGenReplacer(int objectIndex, int xTile, int yTile, int groundLevel = -1, int itemQuality = 0, float velocityMultiplyer = 1f, GameLocation location = null)
        {
            if(Mod.config.seedChanges == null || Mod.config.seedChanges.Count <= 0)
            {
                Game1.createObjectDebris(objectIndex, xTile, yTile, groundLevel, itemQuality, velocityMultiplyer, location);
                return;
            }
            foreach (SeedChanges sc in Mod.config.seedChanges)
            {
                if (sc.seedIndex == -1000 || sc.seedIndex == objectIndex)
                {
                    if (sc.seedChances != null && sc.seedChances.Count > 0)
                    {
                        foreach (SeedChances newSeed in sc.seedChances)
                        {
                            int seedIndex = newSeed.getSeedIndex();
                            if(seedIndex >= 0 && Game1.random.NextDouble() < newSeed.applyProfessionChances(Game1.player))
                            {
                                int totalSeeds = newSeed.applyProfessionCount(Game1.player);
                                if(totalSeeds > 0)
                                {
                                    Game1.createMultipleObjectDebris(seedIndex, xTile, yTile, totalSeeds);
                                    if(sc.replaceOldSeed)
                                        return;
                                }
                            }
                        }
                    }
                }
            }
            Game1.createObjectDebris(objectIndex, xTile, yTile, groundLevel, itemQuality, velocityMultiplyer, location);
            return;

        }
    }
}
