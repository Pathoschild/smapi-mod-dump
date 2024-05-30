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
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace BZP_Allergies.HarmonyPatches
{
    internal class SkillBook_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), "readBook"),
                transpiler: new HarmonyMethod(typeof(SkillBook_Patches), nameof(ReadBook_Transpiler)),
                postfix: new HarmonyMethod(typeof(SkillBook_Patches), nameof(ReadBook_Postfix))
            );
        }

        public static IEnumerable<CodeInstruction> ReadBook_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // find the first DelayedAction.functionAfterDelay that comes after the ldstr "Strings\\1_6_Strings:QoS_Cookbook"
            bool done = false;
            bool foundStr = false;
            MethodInfo delayedAction = AccessTools.Method(typeof(DelayedAction), nameof(DelayedAction.functionAfterDelay));

            List<CodeInstruction> inputCodes = new(instructions);
            List<CodeInstruction> codes = new();

            for (int i = 0; i < inputCodes.Count; i++)
            {
                CodeInstruction instr = inputCodes[i];
                if (instr.opcode == OpCodes.Ldstr && instr.operand is string str && str == "Strings\\1_6_Strings:QoS_Cookbook")
                {
                    foundStr = true;
                }

                if (!done && foundStr && instr.opcode == OpCodes.Call && instr.operand is MethodInfo mi && mi == delayedAction)
                {
                    // pop the args for the delayed action
                    for (int j = 0; j < 2; j++) codes.Add(new CodeInstruction(OpCodes.Pop));

                    // replace the delayed action with my method
                    codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    FieldInfo finfo = AccessTools.Field(typeof(Item), nameof(Item.itemId));
                    codes.Add(new CodeInstruction(OpCodes.Ldfld, finfo));

                    MethodInfo opImplicit = AccessTools.Method(typeof(Netcode.NetString), "op_Implicit");  // some weird netstsring thing idk
                    codes.Add(new CodeInstruction(OpCodes.Call, opImplicit));

                    MethodInfo mine = AccessTools.Method(typeof(SkillBook_Patches), nameof(ConditionallyRenderLearnedPower));
                    codes.Add(new CodeInstruction(OpCodes.Call, mine));

                    // skip over the "pop" for the delayed action's unused return
                    i++;
                    done = true;
                }
                else codes.Add(instr);
            }

            return codes.AsEnumerable();
        }

        public static void ConditionallyRenderLearnedPower(string bookId)
        {
            if (bookId == Constants.AllergyTeachBookId || bookId == Constants.AllergyCookbookId) return;

            // original vanilla code
            DelayedAction.functionAfterDelay(delegate
            {
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:LearnedANewPower"));
            }, 1000);
        }

        public static void ReadBook_Postfix(ref StardewValley.Object __instance)
        {
            try
            {
                string bookId = Traverse.Create(__instance).Property("ItemId").GetValue<string>();

                if (bookId == Constants.AllergyTeachBookId)
                {
                    if (!AllergenManager.ModDataGet(Game1.player, Constants.ModDataRandom, out string val) || val == "false")
                    {
                        return;  // we don't discover allergies
                    }

                    ISet<string> playerHas = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataHas);
                    ISet<string> playerDiscovered = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataDiscovered);

                    if (playerDiscovered.Count == playerHas.Count) return;

                    // pick a random one to discover
                    List<string> diff = playerHas.Except(playerDiscovered).ToList();
                    int discoverIdx = new Random().Next(0, diff.Count);
                    AllergenManager.DiscoverPlayerAllergy(diff[discoverIdx]);

                    if (!Game1.player.mailReceived.Contains("read_a_book"))
                    {
                        Game1.player.mailReceived.Add("read_a_book");
                    }

                    Game1.showGlobalMessage(ModEntry.Instance.Translation.Get("books.allergy-teach"));
                }
                else if (bookId == Constants.AllergyCookbookId)
                {
                    Game1.player.cookingRecipes.TryAdd(Constants.PlantMilkId, 0);
                    if (!Game1.player.mailReceived.Contains("read_a_book"))
                    {
                        Game1.player.mailReceived.Add("read_a_book");
                    }

                    Game1.showGlobalMessage(ModEntry.Instance.Translation.Get("books.allergy-cookbook"));

                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReadBook_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
