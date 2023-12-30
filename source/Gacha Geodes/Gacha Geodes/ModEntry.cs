/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-gacha-geodes
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using System.Collections.Generic;
using StardewValley.BellsAndWhistles;
using System.Reflection.Emit;
using System.Linq;
using StardewValley.GameData.Objects;
using StardewValley.Internal;
using StardewValley.GameData;

namespace Gacha_Geodes
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.getTreasureFromGeode)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.getTreasureFromGeode_Transpiler))
            );
        }

        public static IEnumerable<CodeInstruction> getTreasureFromGeode_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction last = null;
            CodeInstruction ldfld = null;

            foreach (var instruction in instructions) {
                if (instruction.opcode == OpCodes.Ldfld)
                    ldfld = instruction;

                if (instruction.opcode == OpCodes.Stloc_S
                    && last.Calls(
                        typeof(ItemQueryResolver).GetMethod(nameof(ItemQueryResolver.TryResolveRandomItem),
                        new[] { typeof(ISpawnItemData), typeof(ItemQueryContext), typeof(bool), typeof(HashSet<string>),
                                typeof(Func<string, string>), typeof(Item), typeof(Action<string, string>) }))) {

                    //We know this variable is declared right above Item so its LocalIndex is one less
                    //Weirdly enough this gets its own wrapper object, so we have to get our object out of the wrapper
                    var ldDropObj = new CodeInstruction(OpCodes.Ldloc_S, ((LocalBuilder)instruction.operand).LocalIndex - 1);
                    yield return ldDropObj;
                    yield return ldfld;

                    var call = new CodeInstruction(OpCodes.Call, typeof(ModEntry).GetMethod(nameof(getWeightedTreasure)));
                    yield return call;
                }


                last = instruction;
                yield return instruction;
            }
        }


        public static Item getWeightedTreasure(Item baseGameItem, ObjectGeodeDropData drop)
        {
            if (drop.RandomItemId is null || drop.RandomItemId.Count == 0)
                return baseGameItem;

            var sum = 0;
            var dic = new Dictionary<string, int>();

            foreach (var itemId in drop.RandomItemId) {
                var item = ItemRegistry.Create(itemId);

                //I am expecting only minerals or artifacts
                var i = Game1.player.mineralsFound.ContainsKey(item.ItemId) ? Game1.player.mineralsFound[item.ItemId] : 0;
                i = Game1.player.archaeologyFound.ContainsKey(item.ItemId) ? Game1.player.archaeologyFound[item.ItemId][0] : i;
                sum += i + Config.BaseFound;

                //Setting a base found to reduce weight impact per geode
                //Must be BaseFound > 0
                //The higher BaseFound, the less each found delta impacts weight
                dic.Add(itemId, i + Config.BaseFound);
            }

            var weightedSum = 0;
            foreach (var item in dic)
                weightedSum += dic[item.Key] = sum / item.Value;

            var r = Game1.random.NextDouble() * weightedSum;
            var k = 0;

            dic = dic = dic.OrderBy(x => Game1.random.Next()).ToDictionary(item => item.Key, item => item.Value);

            foreach (var item in dic)
                if (k + item.Value >= r)
                    return ItemRegistry.Create(item.Key);
                else
                    k += item.Value;

            return ItemRegistry.Create(dic.Last().Key);
        }
    }
}
