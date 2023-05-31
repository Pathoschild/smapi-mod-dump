/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace SpousesIsland.ModContent
{
    internal class ModStatus
    {
        public string Name { get; }
        public bool DayVisit { get; set; }
        public (bool, int) WeekVisit { get; set; } = (false, 0);
        public List<string> Who { get; set; } = new();

        public ModStatus()
        {

        }
        public ModStatus(ModStatus m)
        {
            Name = m.Name;
            DayVisit = m.DayVisit;
            WeekVisit = m.WeekVisit;
            Who = m.Who;
        }
        public ModStatus(Farmer player, bool includeSpouses)
        {
            Name = player.Name;
            DayVisit = player.mailReceived?.Contains("VisitTicket_day") ?? false;
            WeekVisit = (player.mailReceived?.Contains("VisitTicket_week") ?? false, 0);
            Who = includeSpouses ? Information.PlayerSpouses(player) : new List<string>();
        }
    }

    internal class Patches
    {
        /// <summary>
        /// Handles game's actions when receiving island ticket.
        /// </summary>
        /// <param name="__instance"> NPC receiving item.</param>
        /// <param name="who">player</param>
        internal static bool tryToReceiveTicket(ref NPC __instance, Farmer who)
        {
            TryGetObjectId(who, out var isDay, out var isWeek);
            
            if (who?.ActiveObject == null || (!isDay && !isWeek))
            {
                return true;
            }

            //if a visit is already going on
            if(ModEntry.IslandToday && __instance.Name != "Willy")
            {
                //tell player
		        string alreadyongoing = ModEntry.TL.Get("AlreadyOngoing.Visit");
                //Game1.drawDialogueBox(Game1.parseText(alreadyongoing));
                Game1.addHUDMessage(new HUDMessage(alreadyongoing,HUDMessage.newQuest_type));
                return false;
            }
            //if festival tomorrow
            else if(Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
            {
                //tell player theres festival tmrw
                var festivalnotice = Game1.parseText(ModEntry.TL.Get("FestivalTomorrow"));
                Game1.drawDialogueBox(festivalnotice);
                return false;
            }
            //if not, call method that handles NPC's reaction (+etc).
            else
            {
                TicketActions(__instance, who, isDay, isWeek);
                return false;
            }

        }

        private static void TryGetObjectId(Farmer who, out bool b, out bool b1)
        {
            /* 1.6 compat: 
                b = who?.ActiveObject?.QualifiedItemId == "(O)mistyspring.spousesislandCP_ticketD"; //old: who?.ActiveObject?.Name == "Island ticket (day)";
                b1 = who?.ActiveObject?.QualifiedItemId == "(O)mistyspring.spousesislandCP_ticketW"; //old: who?.ActiveObject?.Name == "Island ticket (week)";
            */
            b = who?.ActiveObject?.Name == "Island ticket (day)";
            b1 = who?.ActiveObject?.Name == "Island ticket (week)";
        }

        /// <summary>
        /// Handles NPC's reaction to ticket.
        /// </summary>
        /// <param name="__instance"> NPC receiving item.</param>
        /// <param name="who">player</param>
        /// <param name="isDay">If it's a day invite.</param>
        /// <param name="isWeek">If it's a week invite.</param>
        /// <exception cref="ArgumentException">If there's any error with the item, this is sent (shouldn't happen but still added as preemptive measure).</exception>
        private static void TicketActions(NPC __instance, Farmer who, bool isDay, bool isWeek)
        {
            __instance.Halt();
            __instance.faceGeneralDirection(who.getStandingPosition(), 0, opposite: false, useTileCalculations: false);

            var npcdata = who.friendshipData[__instance.Name]; //to simplify text below and make more understandable

            if (npcdata.IsMarried() || npcdata.IsRoommate())
            {
                //get invited list
                var inviteds = ModEntry.Status.Who;
                var hasInvites = inviteds.Count != 0;

                //if: already scheduled for a week
                var scheduledWeek = isDay && who.mailbox.Contains("VisitTicket_week");
                //if: already scheduled for a day
                var scheduledDay = isWeek && who.mailbox.Contains("VisitTicket_day");


                //if already invited
                if (hasInvites && inviteds.Contains(__instance.Name))
                {
                    //tell player about it
		            var alreadyinvited = string.Format(ModEntry.TL.Get("AlreadyInvited"), __instance.displayName);
                    Game1.addHUDMessage(new HUDMessage(alreadyinvited, HUDMessage.error_type));
                }
                //if different than current invitation
                else if(scheduledDay || scheduledWeek)
                {
                    //log just in case.
                    ModEntry.Mon.Log($"Player {who.displayName} has already scheduled a visit for {(scheduledDay ? "tomorow" : "next week")}. Can't use a different ticket (current one : {who.ActiveObject.Name})");

                    //inform day/week visit has already been scheduled.
		            var scheduleType = scheduledDay ? ModEntry.TL.Get("AlreadyScheduled.Day") : ModEntry.TL.Get("AlreadyScheduled.Week");
                    Game1.drawDialogueBox(Game1.parseText(scheduleType));
                }
                //if none above apply, continue to inviting
                else
                {
                    //add a reasonable amount of friendship - temporarily removed
                    //who.changeFriendship(100,__instance);
                    Game1.drawDialogue(__instance, GetInviteDialogue(__instance));

                    //user will always have data in Status (created during SaveLoadedBasicInfo).
                    //so there's no worry about possible nulls
                    if (isDay)
                    {
                        ModEntry.Status.DayVisit = true;
                        who.mailbox.Add("VisitTicket_day");
                    }
                    else if (isWeek)
                    {
                        ModEntry.Status.WeekVisit = (true, 0);
                        who.mailbox.Add("VisitTicket_week");
                    }
                    else
                    {
                        throw new ArgumentException("Ticket is neither week nor day one. An error happened somewhere in the method.");
                    }

                    inviteds.Add(__instance.Name); //add name of spouse to allowed list

                    who.reduceActiveItemByOne();
                }
            }
            else if (__instance.Name == "Willy" && (who.currentLocation.Name == "Beach" || who.currentLocation.Name == "FishShop"))
            {
                var willytext = ModEntry.TL.Get("Willy.IslandTicket");
                Game1.drawDialogue(__instance, willytext);
                
                var yn = who.currentLocation.createYesNoResponses();
                who.currentLocation.createQuestionDialogue(ModEntry.TL.Get("IslandVisit.Question"), yn, GetWarped);
            }
            else
            {
                //send rejection dialogue like the pendant one
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", __instance.displayName)));
            }
        }

        /// <summary>
        /// Depending on answer, warps farmer to IslandSouth (or not).
        /// </summary>
        /// <param name="who">The player</param>
        /// <param name="answer">Answer chosen</param>
        private static void GetWarped(Farmer who, string answer)
        {
            var yn = who.currentLocation.createYesNoResponses();

            if(answer == yn[0].responseText || answer == "Yes") //answer is Yes, but responseText is localized
            {
                var willy = Game1.getCharacterFromName("Willy", false);

                willy.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:BoatTunnel_willyText_random" + Game1.random.Next(2)));
                willy.jump();
                who.reduceActiveItemByOne();

                DelayedAction.playSoundAfterDelay("ocean", 2000);
                DelayedAction.warpAfterDelay("IslandSouth", new Microsoft.Xna.Framework.Point(21,43), 500);
            }
            else
            {
                string rejected = ModEntry.TL.Get("IslandVisit.Rejected");
                Game1.drawObjectDialogue(rejected);
            }
        }

        /// <summary>
        /// Get the dialogue for a NPC, depending on name and personality.
        /// </summary>
        /// <param name="who"></param>
        /// <returns>The NPC's reply to being invited (to the island).</returns>
        private static string GetInviteDialogue(NPC who)
        {
            var vanilla = who.Name switch {

                "Abigail" => true, 
                "Alex" => true,
                "Elliott" => true,
                "Emily" => true,
                "Haley" => true,
                "Harvey" => true,
                "Krobus" => true,
                "Leah" => true,
                "Maru" => true,
                "Penny" => true,
                "Sam" => true,
                "Sebastian" => true,
                "Shane" => true,
                "Claire" => true,
                "Lance" => true,
                "Olivia" => true,
                "Sophia" => true,
                "Victor" => true,
                "Wizard" => true,
                _ => false, 
            };

            if(vanilla)
            {
                return ModEntry.TL.Get($"Invite_{who.Name}");
            }
            else
            {
                var r = Game1.random.Next(1, 4);
                return ModEntry.TL.Get($"Invite_generic_{who.Optimism}_{r}"); //1 polite, 2 rude, 0 normal?
            }
        }

        
    }
}
