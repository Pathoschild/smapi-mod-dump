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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ChangeFarmCaves
{
    internal static class Patches
    {
        private static Harmony harmony;
        private static Dialogue lastDialogue;

        internal static void Patch(IModHelper helper)
        {
            harmony = new(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new(typeof(Patches), nameof(checkActionPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
                prefix: new(typeof(Patches), nameof(receiveLeftClickPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogue)),
                prefix: new(typeof(Patches), nameof(answerDialoguePrefix)),
                postfix: new(typeof(Patches), nameof(answerDialoguePostfix))
            );
        }

        internal static void Transpile()
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmCave), nameof(FarmCave.setUpMushroomHouse)),
                transpiler: new(typeof(Patches), nameof(setUpMushroomHouseTranspiler))
            );
        }

        private static bool isChangingFarmCave = false;

        public static bool checkActionPrefix(NPC __instance, Farmer who, GameLocation l)
        {
            try
            {
                if (Game1.IsMasterGame && __instance.Name == "Demetrius" && (!__instance.CurrentDialogue.TryPeek(out var dialogue) || dialogue == lastDialogue) && (who.CurrentItem is null || !CanGift(__instance, who.CurrentItem, who)) && Game1.activeClickableMenu is null)
                {
                    var responses = Game1.currentLocation.createYesNoResponses();
                    Game1.currentLocation.createQuestionDialogue(ModEntry.ITranslations.Get("Question"), responses, (who, answer) =>
                    {
                        if (answer == "Yes")
                        {
                            Game1.activeClickableMenu = null;
                            isChangingFarmCave = true;
                            new Event().tryEventCommand(l, Game1.currentGameTime, ["Cave"]);
                        }
                    }, __instance);
                    return false;
                }
                if (__instance.CurrentDialogue.TryPeek(out var next))
                    lastDialogue = next;
                return true;
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(NPC.checkAction)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static bool receiveLeftClickPrefix(DialogueBox __instance, int x, int y, bool playSound = true)
        {
            try
            {
                if (!isChangingFarmCave)
                    return true;
                if (!ModEntry.IConfig.Instant)
                    ModEntry.FarmCave.modData["ChangeFarmCaves.ShouldChange"] = $"{Game1.currentLocation.lastQuestionKey},{__instance.selectedResponse}";
                else
                    new Event().answerDialogue(Game1.currentLocation.lastQuestionKey, __instance.selectedResponse);
                isChangingFarmCave = false;
                __instance.closeDialogue();
                return false;
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(DialogueBox.receiveLeftClick)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static void answerDialoguePrefix(string questionKey)
        {
            try
            {
                if (questionKey != "cave")
                    return;
                ModEntry.FarmCave.Objects.Clear();
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(Event.answerDialogue)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return; }
        }

        public static void answerDialoguePostfix(string questionKey, int answerChoice)
        {
            if (questionKey != "cave")
                return;
            if (answerChoice == 0 && !ModEntry.FarmCave.modData.ContainsKey("ChangeFarmCaves.HasGottenDehydrator"))
            {
                ModEntry.FarmCave.modData.Add("ChangeFarmCaves.HasGottenDehydrator", "true");
                Transpile();
            }
        }

        public static IEnumerable<CodeInstruction> setUpMushroomHouseTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            if (!ModEntry.FarmCave.modData.ContainsKey("ChangeFarmCaves.HasGottenDehydrator"))
                return instructions;

            List<CodeInstruction> res = [];
            int i = 0;
            foreach (var instruction in instructions)
            {
                if (i < 67)
                    res.Add(instruction);
                i++;
            }
            res.Add(new(OpCodes.Ret));
            return res;
        }

        private static bool CanGift(NPC who, Item what, Farmer player)
        {
            if (!what.canBeGivenAsGift())
                return false;
            if ((player.friendshipData[who.Name].GiftsToday == 1 || player.friendshipData[who.Name].GiftsThisWeek == 2))
                return false;
            return true;
        }
    }
}
