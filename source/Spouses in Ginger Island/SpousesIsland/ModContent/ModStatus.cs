/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpousesIsland
{
    internal class ModStatus
    {
        public string Name { get; set; }
        public bool DayVisit { get; set; } = false;
        public (bool, int) WeekVisit { get; set; } = (false, 0);

        public ModStatus()
        {

        }
        public ModStatus(ModStatus m)
        {
            Name = m.Name;
            DayVisit = m.DayVisit;
            WeekVisit = m.WeekVisit;
        }
        public ModStatus(Farmer player)
        {
            Name = player.Name;
            DayVisit = player?.mailReceived?.Contains("VisitTicket_day") ?? false;
            WeekVisit = (player?.mailReceived?.Contains("VisitTicket_week") ?? false, 0);
        }
    }

    internal class Patches
    {
        //patch here
        internal static void tryToReceiveTicket(ref NPC __instance, Farmer who)
        {
            bool isDay = who?.ActiveObject?.ParentSheetIndex == ModEntry.jsonAssets?.GetObjectId("Island ticket - day");
            bool isWeek = who?.ActiveObject?.ParentSheetIndex == ModEntry.jsonAssets?.GetObjectId("Island ticket - week");

            if (who.ActiveObject == null || (!isDay && !isWeek))
            {
                return;
            }

            who.Halt();
            who.faceGeneralDirection(__instance.getStandingPosition(), 0, opposite: false, useTileCalculations: false);

            if (who.friendshipData[__instance.Name].IsMarried())
            {
                Game1.drawDialogue(__instance, GetInviteDialogue(__instance));
                //var ID = Utility.IsNormalObjectAtParentSheetIndex(who.ActiveObject, who.ActiveObject.ParentSheetIndex) ?? false;

                if (isDay)
                {
                    //ModEntry.status[who.userID.Value].DayVisit = true;
                    who.mailbox.Add("VisitTicket_day");
                }
                else if (isWeek)
                {
                    //ModEntry.status[who.userID.Value].WeekVisit = (true, 0);
                    who.mailbox.Add("VisitTicket_week");
                }
                else
                {
                    throw new ArgumentException();
                }

                who.reduceActiveItemByOne();
            }
            else
            {
                //send rejection dialogue like the pendant one
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", __instance.displayName)));
            }
        }

        private static string GetInviteDialogue(NPC who)
        {
            bool vanilla = who.Name switch {

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
                int r = Game1.random.Next(1, 4);
                return ModEntry.TL.Get($"Invite_generic_{who.Optimism}_{r}"); //1 polite, 2 rude, 0 normal?
            }
        }
    }
}
