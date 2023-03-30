/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using DynamicDialogues.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Linq;


namespace DynamicDialogues
{
    internal class ModPatches
    {
        //for custom greetings
        public static bool SayHiTo_Prefix(ref NPC __instance, Character c)
        {
            var instancename = __instance.Name;
            var cname = (c as NPC).Name;
            var mainAndRef = (instancename, cname);
            var refAndMain = (cname, instancename);

            try
            {
                //if a (thisnpc, othernpc) key exists
                if (ModEntry.Greetings.ContainsKey((mainAndRef)))
                {
                    //log, then use previous key to find value
                    ModEntry.Mon.Log($"Found greeting patch for {__instance.Name}");
                    __instance.showTextAboveHead(ModEntry.Greetings[(mainAndRef)]);

                    //if that other npc has a key for thisnpc
                    if (ModEntry.Greetings.ContainsKey(refAndMain))
                    {
                        //same as before
                        ModEntry.Mon.Log($"Found greeting patch for {(c as NPC).Name}");
                        (c as NPC).showTextAboveHead(ModEntry.Greetings[(refAndMain)], -1, 2, 3000, 1000 + Game1.random.Next(500));
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error while applying patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }

            return true;
        }

        //for AddScene
        internal static bool PrefixTryGetCommand(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length < 2) //scene has optional parameters, so its 2 OR more
            {
                return true;
            }
            else if (split[0].Equals(ModEntry.AddScene, StringComparison.Ordinal))
            {
                EventScene.Add(__instance, location, time, split);
                return false;
            }
            else if (split[0].Equals(ModEntry.RemoveScene, StringComparison.Ordinal))
            {
                EventScene.Remove(__instance, location, time, split);
                return false;
            }
            return true;
        }

        //for starting events/quests from npc dialogue
        internal static void PostCurrentDialogue(ref Dialogue __instance)
        {
            try
            {
                //if theres no questions for this npc, ignore
                if (ModEntry.Questions.ContainsKey(__instance.speaker.Name) == false)
                {
                    return;
                }

                /* 
                 * when setting new dialogue, its pushed atop the stack. so we only need to check first value
                //var thistext = __instance.speaker.CurrentDialogue.First();
                 * this isn't needed anymore because we check Dialogue directly (instead of getting speaker's)
                 */

                //return if its not last text
                if (__instance.isOnFinalDialogue() == false)
                {
                    return;
                }

                //check each value
                foreach (var raw in ModEntry.Questions[__instance.speaker.Name])
                {
                    //if NPC dialogues contains npc's answer
                    if (__instance.dialogues.Contains(raw.Answer))
                    {
                        //get the quest and event

                        EventStarter.Call(new string[] { raw.QuestToStart, raw.EventToStart });
                    }
                }
            }
            catch(Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
            }
        }

        internal static void PostDialogueAction(ref GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            foreach (var data in ModEntry.Questions)
            {
                //if the npc isnt in map, dont check its data
                if(__instance.characters.Any(npc => npc.Name == data.Key) == false)
                {
                    continue;
                }

                foreach (var raw in data.Value)
                {
                    //QnA contains our exact question string
                    if (questionAndAnswer.Contains(raw.Question))
                    {
                        EventStarter.Call(new string[] { raw.QuestToStart, raw.EventToStart });
                    }
                }
            }
        }
    }
}