/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/PolyamorySweetLove
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolyamorySweetLove
{
    public static class EventPatches
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        public static bool startingLoadActors = false;
        public static Vector2 fuckaway;
 

        public static bool Event_answerDialogueQuestion_Prefix(Event __instance, NPC who, string answerKey)
        {
            try
            {
                string accept = $"Characters\\Dialogue\\{who.Name}:FlowerDance_Accept_Spouse";
                string acceptbackup = $"Strings\\StringsFromCSFiles:Event.cs.1634";

                if (answerKey == "danceAsk" && !who.HasPartnerForDance && Game1.player.friendshipData[who.Name].IsMarried())
                {
                    //string accept = "You want to be my partner for the flower dance?#$b#Okay! I'd love to dance with you! <$h";
                   
                    var gender = who.Gender;
               
                    try
                    {
                        Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.Name, true));
                    }
                    catch
                    {
                    }
                    Game1.player.dancePartner.Value = who;

                    if (gender == Gender.Female)
                    {
                        if(who.Dialogue.ContainsKey("FlowerDance_Accept_Spouse")) 
                        {
                            who.setNewDialogue(accept, false, false);
                            //$"Characters\\Dialogue\\{who.Name}:FlowerDance_Accept_Spouse"
                        }
                        else
                        {
                            who.setNewDialogue(acceptbackup, false, false);
                        }
                    }
                    else 
                    {
                        if (who.Dialogue.ContainsKey("FlowerDance_Accept_Spouse"))
                        {
                            who.setNewDialogue(accept, false, false);
                            //who.setNewDialogue(Game1.content.LoadString($"Characters\\Dialogue\\{who.Name}:FlowerDance_Accept_Spouse"), false, false);
                            //$"Characters\\Dialogue\\{who.Name}:FlowerDance_Accept_Spouse"
                        }
                        else
                        {
                            who.setNewDialogue(acceptbackup, false, false);
                        }
                    }


                    using (List<NPC>.Enumerator enumerator = __instance.actors.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            NPC j = enumerator.Current;
                            if (j.CurrentDialogue != null && j.CurrentDialogue.Count > 0 && j.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                            {
                                j.CurrentDialogue.Clear();
                            }
                        }
                    }
                    Game1.drawDialogue(who);
                    who.immediateSpeak = true;
                    who.facePlayer(Game1.player);
                    who.Halt();
                    return false;
                }
            }

            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Event_answerDialogueQuestion_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }
        public static void Event_command_loadActors_Prefix()
        {
            try
            {
                startingLoadActors = true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Event_command_loadActors_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void Event_command_loadActors_Postfix(NPC __instance)
        {
            try
            {
                var fuckaway = new Vector2((int)(500), (int)(500));
              
                startingLoadActors = false;
                Game1Patches.lastGotCharacter = null;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Event_command_loadActors_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}