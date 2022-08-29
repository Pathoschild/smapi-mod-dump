/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmVisitors
{
    internal class Actions
    {
        /* regular visit *
         * the one used by non-scheduled NPCs */
        internal static void AddToFarmHouse(NPC visitor, FarmHouse farmHouse, bool HadConfirmation)
        {
            try
            {
                if (!Values.IsVisitor(visitor.Name))
                {
                    ModEntry.Mon.Log($"{visitor.displayName} is not a visitor!");
                    return;
                }

                bool isanimating = visitor.doingEndOfRouteAnimation.Value;

                if(isanimating == true || (visitor.doingEndOfRouteAnimation is not null && visitor.doingEndOfRouteAnimation.Value is true))
                {
                    RemoveAnimation(visitor);
                }
                /* not needed anymore since we exclude hospital days. however i like it and i restore the dialogues afterwards so it's fine*/
                
                if(visitor.CurrentDialogue.Any())
                {
                    visitor.CurrentDialogue.Clear();
                }
                
                visitor.ignoreScheduleToday = true;
                visitor.temporaryController = null;

                if(HadConfirmation == false)
                {
                    //Game1.drawDialogue(npcv, Values.GetIntroDialogue(npcv));
                    Game1.drawDialogue(visitor, Values.GetDialogueType(visitor, "Introduce"));
                }

                var position = farmHouse.getEntryLocation();
                position.Y--;
                visitor.faceDirection(0);
                Game1.warpCharacter(visitor, "FarmHouse", position);

                //npcv.showTextAboveHead(string.Format(Values.GetTextOverHead(npcv),Game1.MasterPlayer.Name));
                visitor.showTextAboveHead(string.Format(Values.GetDialogueType(visitor, "WalkIn"),Game1.MasterPlayer.Name));

                //set before greeting because "Push" leaves dialogues at the top
                if (Game1.MasterPlayer.isMarried())
                {
                    if (ModEntry.InLawDialogue is not "None")
                        InLawActions(visitor);
                }

                //npcv.CurrentDialogue.Push(new Dialogue(string.Format(Values.StringByPersonality(npcv), Values.GetSeasonalGifts()), npcv)); <- doesnt work

                var enterDialogue = Values.GetDialogueType(visitor, "Greet"); //string.Format(Values.StringByPersonality(npcv), Values.GetSeasonalGifts());
                if(ModEntry.ReceiveGift)
                {
                    var withGift = $"{enterDialogue}#$b#{Values.GetGiftDialogue(visitor)}";
#if DEBUG
                    ModEntry.Mon.Log($"withGift: {withGift}");
#endif
                    enterDialogue = string.Format(withGift, Values.GetSeasonalGifts());
                }
#if DEBUG
                ModEntry.Mon.Log($"enterDialogue: {enterDialogue}");
                visitor.setNewDialogue("testing if dialogue works via setNewDialogue.", true, true);
                visitor.CurrentDialogue.Push(new Dialogue("this is a new Dialogue being pushed to CurrentDialogue.", visitor));
                visitor.CurrentDialogue.Push(new Dialogue($"TESTING, {enterDialogue}", visitor));
#endif
                visitor.setNewDialogue($"{enterDialogue}", true, true);
                visitor.CurrentDialogue.Push(new Dialogue(Values.GetDialogueType(visitor, "Thanking"), visitor));

                if (Game1.currentLocation == farmHouse)
                {
                    Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
                }

                position.Y--;
                visitor.controller = new PathFindController(visitor, farmHouse, position, 0);
            }
            catch(Exception ex)
            {
                ModEntry.Mon.Log($"Error while adding to farmhouse: {ex}", LogLevel.Error);
            }

        }

        internal static void ReturnToNormal(NPC c, int currentTime)
        {
            //special NPCs (locked by conditions in game)
            if(c.Name.Equals("Dwarf") || c.Name.Equals("Kent") || c.Name.Equals("Leo"))
            {
                try
                {
                    c.CurrentDialogue?.Clear();
                    //c.Dialogue?.Clear(); as said before this is counterproductive and only lags / doesnt do anything for the mod

                    if (c.Name.Equals("Dwarf"))
                    {
                        var mine = Game1.getLocationFromName("Mine");
                        Game1.warpCharacter(c, mine, new Vector2(43, 6));
                    }
                    if (c.Name.Equals("Kent"))
                    {
                        var samhouse = Game1.getLocationFromName("SamHouse");
                        Game1.warpCharacter(c, samhouse, new Vector2(22, 5));
                    }    
                    if (c.Name.Equals("Leo"))
                    {
                        var islandhut = Game1.getLocationFromName("IslandHut");
                        Game1.warpCharacter(c, islandhut, new Vector2(5, 6));
                    }

                    return;
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"Error while returning special NPC ({c.Name}): {ex}", LogLevel.Error);
                }
            }

            //everyone else
            try
            {
                var timeOfDay = Game1.timeOfDay;

                c.Halt();
                c.controller = null;

                c.ignoreScheduleToday = false;
                c.followSchedule = true;

                if (c.DefaultMap.Equals(ModEntry.VisitorData?.CurrentLocation))
                {
                    Point PositionFixed = new((int)(c.DefaultPosition.X / 64), (int)(c.DefaultPosition.Y / 64));

                    Game1.warpCharacter(c, c.DefaultMap, PositionFixed);
                }
                else
                {
                    Game1.warpCharacter(c, ModEntry.VisitorData?.CurrentLocation, ModEntry.VisitorData.Position);
                }

                c.Sprite = ModEntry.VisitorData.Sprite;
                c.endOfRouteMessage.Value = ModEntry.VisitorData.AnimationMessage;

                if (c.Schedule is null)
                {
                    c.InvalidateMasterSchedule();

                    var sched = c.getSchedule(Game1.dayOfMonth);
                    c.Schedule.Clear();
                    foreach (KeyValuePair<int, SchedulePathDescription> pair in sched)
                    {
                        c.Schedule.Add(pair.Key, pair.Value);
                    }
                }
                
                c.checkSchedule(currentTime);
                c.moveCharacterOnSchedulePath();
                c.warpToPathControllerDestination();

                //change facing direction
                c.FacingDirection = ModEntry.VisitorData.Facing;
                c.facingDirection.Value = ModEntry.VisitorData.Facing;
                c.faceDirection(ModEntry.VisitorData.Facing);


                var currentLocation = c.currentLocation;

                if (c.CurrentDialogue.Any() || c.CurrentDialogue is not null) // || c.Dialogue is not null
                {
                    c.CurrentDialogue.Clear();
                    c.resetCurrentDialogue();

                    //c.Dialogue.Clear(); this would clear ALL dialogues (even ones not being used). bad!

                    c.update(Game1.currentGameTime, currentLocation);

                    /*apparently this causes problems for international players (something about key not found).
                     * int heartlvl = Game1.MasterPlayer.friendshipData[c.displayName].Points / 250;
                     * fix: take it out since im not using this anyway lol 
                     * c.checkForNewCurrentDialogue(heartlvl);*/

                    //restores (pre-visit) current dialogue
                    c.CurrentDialogue = ModEntry.VisitorData.CurrentPreVisit;

                    /*restores all pre visit dialogues
                    foreach (KeyValuePair<string,string> pair in ModEntry.VisitorData.AllPreVisit)
                    {
                        c.Dialogue.Add(pair.Key, pair.Value);
                    }
                    taken out because again we dont need this */

                    //if above doesnt work, try using the dialogues with CurrentDialogue.Push
                }

                c.performTenMinuteUpdate(timeOfDay, currentLocation);
                c.update(Game1.currentGameTime, currentLocation);
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error while returning NPC: {ex}", LogLevel.Error);
            }
        }

        private static void RemoveAnimation(NPC npcv)
        {
            try
            {
                ModEntry.Mon.Log($"Stopping animation for {npcv.displayName} and resizing...");

                npcv.Sprite.StopAnimation();
                npcv.Sprite.SpriteWidth = 16;
                npcv.Sprite.SpriteHeight = 32;
                npcv.reloadSprite();

                npcv.Sprite.CurrentFrame = 0;
                npcv.faceDirection(0);

                if(npcv.endOfRouteMessage.Value is not null)
                {
                    npcv.endOfRouteMessage?.Value.Remove(0);
                    npcv.endOfRouteMessage.Value = null;
                }
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error while stopping {npcv.displayName} animation: {ex}", LogLevel.Error);
            }
        }

        internal static void Retire(NPC c, int currentTime, FarmHouse farmHouse)
        {
            /*!Game1.player.currentLocation.name.Value.StartsWith("Cellar")
            
            string currentloc = Game1.currentLocation.Name;
            string cellar = farmHouse.GetCellarName();
            !currentloc.Equals(cellar)*/

            if (Game1.currentLocation == farmHouse && Game1.currentLocation.Name.StartsWith("FarmHouse"))
            {
                try
                {
                    if(c.controller is not null)
                    {
                        c.Halt();
                        c.controller = null;
                    }
                    Game1.fadeScreenToBlack();
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"An error ocurred when pathing to entry: {ex}", LogLevel.Error);
                }
                finally
                {
                    //Game1.drawDialogue(c, Values.GetRetireDialogue(c));
                    Game1.drawDialogue(c, Values.GetDialogueType(c, "Retiring"));
                    ReturnToNormal(c, currentTime);
                    Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
                }
            }
            else
            {
                try
                {
                    Leave(c, currentTime);
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"An error ocurred when pathing to entry: {ex}", LogLevel.Error);
                }
            }
        }
        
        /* extra *
         * if in-law, will have dialogue about it */
        private static void InLawActions(NPC visitor)
        {
            bool addedAlready = false;
            var name = visitor.Name;

            if (!ModEntry.ReplacerOn && Moddeds.IsVanillaInLaw(name))
            {
                if (Vanillas.InLawOfSpouse(name) is true)
                {
                    visitor.setNewDialogue(Vanillas.GetInLawDialogue(name), true, false);
                    addedAlready = true;
                }
            }

            if(ModEntry.InLawDialogue is "VanillaAndMod" || ModEntry.ReplacerOn)
            {
                var spouse = Moddeds.GetRelativeName(name);
                if (spouse is not null && !addedAlready)
                {
                    string formatted = string.Format(Moddeds.GetDialogueRaw(), spouse);
                    visitor.setNewDialogue(formatted, true, false);
                    addedAlready = true;
                }
            }

            if (Game1.MasterPlayer.getChildrenCount() > 0 && addedAlready)
            {
                visitor.setNewDialogue(Vanillas.AskAboutKids(Game1.MasterPlayer), true, false);
            }
        }

        /* customized visits *
         * ones set by user via ContentPatcher*/
        internal static void AddCustom(NPC c, FarmHouse farmHouse, ScheduleData data, bool HadConfirmation)
        {
            try
            {
                if (!Values.IsVisitor(c.Name))
                {
                    ModEntry.Mon.Log($"{c.displayName} is not a visitor!");
                    return;
                }

                NPC npcv = c;

                bool isanimating = npcv.doingEndOfRouteAnimation.Value; // || npcv.goingToDoEndOfRouteAnimation.Value;
                if(isanimating == true || (npcv.doingEndOfRouteAnimation is not null && npcv.doingEndOfRouteAnimation.Value is true))
                {
                    RemoveAnimation(npcv);
                }
                if (npcv.CurrentDialogue.Any())
                {
                    npcv.CurrentDialogue.Clear();
                }

                npcv.ignoreScheduleToday = true;
                npcv.temporaryController = null;

                if(HadConfirmation == false)
                {
                    if (!string.IsNullOrWhiteSpace(data.EntryQuestion))
                    {
                        Game1.drawDialogue(npcv, data.EntryQuestion);
                    }
                    else
                    {
                        //Game1.drawDialogue(npcv, Values.GetIntroDialogue(npcv));
                        Game1.drawDialogue(npcv, Values.GetDialogueType(npcv, "Introduce"));
                    }
                }
                var position = farmHouse.getEntryLocation();
                position.Y--;
                npcv.faceDirection(0);
                Game1.warpCharacter(npcv, "FarmHouse", position);

                if (!string.IsNullOrWhiteSpace(data.EntryBubble))
                {
                    npcv.showTextAboveHead(string.Format(data.EntryBubble, Game1.MasterPlayer.Name));
                }
                else
                {
                    npcv.showTextAboveHead(string.Format(Values.GetDialogueType(npcv, "WalkIn"), Game1.MasterPlayer.Name));
                }

                if (!string.IsNullOrWhiteSpace(data.EntryDialogue))
                {
                    npcv.setNewDialogue(data.EntryDialogue, true, false);
                }
                else
                {
                    var enterDialogue = Values.GetDialogueType(npcv, "Greet");
                    if (ModEntry.ReceiveGift)
                    {
                        enterDialogue += $"#$b#" + Values.GetGiftDialogue(npcv);
                        enterDialogue = String.Format(enterDialogue, Values.GetSeasonalGifts());
                    }
                }

                if (Game1.currentLocation == farmHouse)
                {
                    Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
                }

                position.Y--;
                npcv.controller = new PathFindController(npcv, farmHouse, position, 0);
            }
            catch(Exception ex)
            {
                ModEntry.Mon.Log($"Error while adding to farmhouse: {ex}", LogLevel.Error);
            }

        }
        internal static void RetireCustom(NPC c, int currentTime, FarmHouse farmHouse, string text)
        {
            if (Game1.currentLocation == farmHouse)
            {
                try
                {
                    if(c.controller is not null)
                    {
                        c.Halt();
                        c.controller = null;
                    }
                    Game1.fadeScreenToBlack();
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"An error ocurred when pathing to entry: {ex}", LogLevel.Error);
                }
                finally
                {
                    Game1.drawDialogue(c, text);

                    ReturnToNormal(c, currentTime);
                    Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
                }
            }
            else
            {
                try
                {
                    Leave(c, currentTime);
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"An error ocurred when pathing to entry: {ex}", LogLevel.Error);
                }
            }
        }

        //for both
        internal static void Leave(NPC c, int currentTime)
        {
            if (c.controller is not null)
            {
                c.Halt();
                c.controller = null;
            }
            ReturnToNormal(c, currentTime);

            Game1.drawObjectDialogue(string.Format(Values.GetNPCGone(Game1.currentLocation.Name.StartsWith("Cellar")), c.displayName));
        }

        //turn list to string
        internal static string TurnToString(List<string> list)
        {
            string result = "";

            foreach (string s in list)
            {
                if(s.Equals(list[0]))
                {
                    result = $"{s}";
                }
                else
                {
                    result = $"{result}, {s}";
                }
            }
            return result;
        }
    }
}