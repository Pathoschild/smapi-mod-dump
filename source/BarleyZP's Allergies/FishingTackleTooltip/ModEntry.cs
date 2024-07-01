/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley.TerrainFeatures;

namespace FishingTackleTooltip
{
    internal sealed class ModEntry : Mod
    {
        public static ITranslationHelper Translation;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Translation = helper.Translation;

            Harmony harmony = new(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(doDoneFishing_Transpiler))
            );

            //helper.Events.GameLoop.DayStarted += OnDayStarted;

            helper.ConsoleCommands.Add("tackle_tooltip", "Get the bait/tackle you recently ran out of from the fishing rod you're holding.", GetLastUsed);
        }

        //private void OnDayStarted(object? sender, DayStartedEventArgs e)
        //{
        //    // DELETE THIS FOR RELEASE
        //    StardewValley.Object brokenTrapBobber = ItemRegistry.Create<StardewValley.Object>("(O)694");
        //    brokenTrapBobber.uses.Value = FishingRod.maxTackleUses - 1;
        //    Game1.player.Items.Add(brokenTrapBobber);

        //    StardewValley.Object treasure = ItemRegistry.Create<StardewValley.Object>("(O)693");
        //    treasure.uses.Value = FishingRod.maxTackleUses - 1;
        //    Game1.player.Items.Add(treasure);
        //}

        public static IEnumerable<CodeInstruction> doDoneFishing_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> inputInstrList = new(instructions);
            List<CodeInstruction> result = new();

            string baitStr = "Strings\\StringsFromCSFiles:FishingRod.cs.14085";
            int baitStrIdx = -1;

            string tackleStr = "Strings\\StringsFromCSFiles:FishingRod.cs.14086";
            int tackleStrIdx = -1;

            for (int i=0; i < inputInstrList.Count; i++)
            {
                // find the "Strings\\StringsFromCSFiles:FishingRode.cs.14085" ldstr call
                CodeInstruction instr = inputInstrList[i];
                if (instr.opcode == OpCodes.Ldstr && instr.operand is string s && s == baitStr)
                {
                    result.AddRange(inputInstrList.Take(i - 1));
                    baitStrIdx = i + 3;

                    result.Add(new CodeInstruction(OpCodes.Ldloc_2));
                    result.Add(new CodeInstruction(OpCodes.Ldarg_0));

                    MethodInfo mine = AccessTools.Method(typeof(ModEntry), nameof(ShowBaitRunOutMessage));
                    result.Add(new CodeInstruction(OpCodes.Call, mine));
                    i += 2;
                }
                else if (instr.opcode == OpCodes.Ldstr && instr.operand is string s1 && s1 == tackleStr)
                {
                    result.AddRange(inputInstrList.Skip(baitStrIdx).Take(i - baitStrIdx - 1));
                    tackleStrIdx = i + 3;

                    result.Add(new CodeInstruction(OpCodes.Ldloc_S, 6));
                    result.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    result.Add(new CodeInstruction(OpCodes.Ldloc_3));

                    MethodInfo mine = AccessTools.Method(typeof(ModEntry), nameof(ShowTackleRunOutMessage));
                    result.Add(new CodeInstruction(OpCodes.Call, mine));
                    break;
                }
            }

            result.AddRange(inputInstrList.Skip(tackleStrIdx));

            return result;
        }

        public static void ShowBaitRunOutMessage(StardewValley.Object bait, FishingRod rod)
        {
            // bait is attachment slot 0
            rod.modData["BarleyZP.FishingTackleTooltip.0"] = bait.DisplayName;

            if (bait.QualifiedItemId == "(O)703")
            {
                Game1.showGlobalMessage(Translation.Get("baitGoneMagnet"));
            }
            else
            {
                Game1.showGlobalMessage(Translation.Get("baitGone", new { baitName = bait.DisplayName }));
            }
        }

        public static void ShowTackleRunOutMessage(StardewValley.Object tackle, FishingRod rod, int i)
        {
            rod.modData["BarleyZP.FishingTackleTooltip." + i] = tackle.DisplayName;
            Game1.showGlobalMessage(Translation.Get("tackleGone", new { tackleName = tackle.DisplayName }));
        }

        private void GetLastUsed(string command, string[] args)
        {
            Item? item = Game1.player.CurrentItem;
            if (item is not null && item is FishingRod rod)
            {
                bool found = false;
                foreach (var pair in rod.modData.Pairs)
                {
                    if (pair.Key.StartsWith("BarleyZP.FishingTackleTooltip.")) {
                        string bait = pair.Value;
                        Monitor.Log(bait, LogLevel.Info);
                        found = true;
                    }
                }

                if (!found)
                {
                    Monitor.Log("No known last-used bait.", LogLevel.Debug);
                }
                
            }
            else
            {
                Monitor.Log("You need to be holding a fishing rod.", LogLevel.Debug);
            }
        }
    }
}
