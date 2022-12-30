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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFarmCaves
{
    internal static class Patcher
    {
        internal static void Patch(IModHelper helper)
        {
            var harmony = new Harmony(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new(typeof(Patches), nameof(Patches.checkActionPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
                prefix: new(typeof(Patches), nameof(Patches.receiveLeftClickPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogue)),
                prefix: new(typeof(Patches), nameof(Patches.answerDialoguePrefix)) { priority = Priority.First }
            );
        }
    }

    internal static class Patches
    {
        internal static bool isChangingFarmCave = false;

        public static bool checkActionPrefix(NPC __instance, Farmer who, GameLocation l)
        {
            try
            {
                if (__instance.Name == "Demetrius" && Game1.IsMasterGame)
                {
                    if (__instance.CurrentDialogue.Count <= 0 && who.CurrentItem is null)
                    {
                        var responses = Game1.currentLocation.createYesNoResponses();
                        if (Game1.activeClickableMenu is null)
                        {
                            Game1.currentLocation.createQuestionDialogue(ModEntry.ITranslations.Get("Question"), responses, (who, answer) =>
                            {
                                if (answer == "Yes")
                                {
                                    Game1.activeClickableMenu = null;
                                    isChangingFarmCave = true;
                                    new Event().command_cave(l, Game1.currentGameTime, new[] { "" });
                                }
                            }, __instance);
                        }
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(NPC.checkAction)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static bool receiveLeftClickPrefix(DialogueBox __instance, int x, int y, bool playSound = true)
        {
            try
            {
                if (isChangingFarmCave)
                {
                    new Event().answerDialogue(Game1.currentLocation.lastQuestionKey, __instance.selectedResponse);
                    isChangingFarmCave = false;
                    __instance.closeDialogue();
                    return false;
                }
                return true;
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(DialogueBox.receiveLeftClick)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static bool answerDialoguePrefix(Event __instance, string questionKey, int answerChoice)
        {
            try
            {
                if (questionKey == "cave")
                {
                    FarmCave fc = Game1.getLocationFromName("FarmCave") as FarmCave;
                    fc.Objects.Clear();
                    if (ModEntry.IHelper.ModRegistry.IsLoaded("aedenthorn.FarmCaveFramework")) return true;
                    if (answerChoice == 0)
                    {
                        Game1.MasterPlayer.caveChoice.Value = 2;
                        fc.setUpMushroomHouse();
                    }
                    else Game1.MasterPlayer.caveChoice.Value = 1;
                    return false;
                }
                return true;
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching {nameof(Event.answerDialogue)}", LogLevel.Error); ModEntry.IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }
    }
}
