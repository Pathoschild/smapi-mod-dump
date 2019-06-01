using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Mine_Changes.MineChanges.Config;
using StardewModdingAPI;
using StardewValley;

namespace Mine_Changes.MineChanges.Hook
{
    public class LocationHooks
    {
        public static void breakStone(GameLocation __instance, ref bool __result, int indexOfStone, int x, int y, Farmer who, Random r)
        {
            List<BreakStone> config = Mod.config.breakStones;
            if(config == null || config.Count == 0)
            {
                return;
            }
            foreach (BreakStone br in config)
            {
                if (br.getStoneIndex() == indexOfStone)
                {
                    if (br.oreChances != null && br.oreChances.Count > 0)
                    {
                        foreach (OreChances ore in br.oreChances)
                        {
                            int oreIdx = ore.getOreIndex();
                            if (oreIdx >= 0) {
                                double trueOreChance = ore.applyChances(who);
                                if (r.NextDouble() < trueOreChance)
                                {
                                    int oreCount = ore.getCount(r, who);
                                    if (oreCount > 0)
                                    {
                                        Game1.createMultipleObjectDebris(oreIdx, x, y, oreCount, who.UniqueMultiplayerID, __instance);
                                        __result = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void addBreak(Mod mod, HarmonyInstance hInstance)
        {
            MethodInfo breakStone = typeof(StardewValley.GameLocation).GetMethod("breakStone", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (breakStone == null)
            {
                mod.Monitor.Log("Could not find breakStone", LogLevel.Error);
                return;
            }
            MethodInfo ourBreak = typeof(LocationHooks).GetMethod("breakStone");
            if (ourBreak == null)
            {
                mod.Monitor.Log("Could not find the breakStone postfix", LogLevel.Error);
                return;
            }
            hInstance.Patch(breakStone, null, new HarmonyMethod(ourBreak), null);
        }
    }
}
