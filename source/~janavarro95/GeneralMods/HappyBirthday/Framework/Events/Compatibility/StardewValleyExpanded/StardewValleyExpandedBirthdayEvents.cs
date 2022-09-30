/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.HappyBirthday.Framework.Constants;
using Omegasis.HappyBirthday.Framework.Events.EventPreconditions;
using Omegasis.StardustCore.Events;
using Omegasis.StardustCore.Events.Preconditions;
using Omegasis.StardustCore.Events.Preconditions.NPCSpecific;
using Omegasis.StardustCore.Events.Preconditions.TimeSpecific;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events.Compatibility
{
    public class StardewValleyExpandedBirthdayEvents
    {

        /// <summary>
        /// Birthday event for when the player is dating Leah.
        /// Finished.
        /// </summary>
        /// <returns></returns>
        public static HappyBirthdayEventHelper DatingBirthday_Leah()
        {
            NPC leah = Game1.getCharacterFromName("Leah");

            List<EventPrecondition> conditions = new List<EventPrecondition>();
            conditions.Add(new FarmerBirthdayPrecondition());
            conditions.Add(new GameLocationPrecondition(Game1.getLocationFromName("LeahHouse")));
            conditions.Add(new TimeOfDayPrecondition(600, 2600));
            conditions.Add(new DatingNPCEventPrecondition(leah));
            conditions.Add(new IsStardewValleyExpandedInstalledPrecondition(true));

            HappyBirthdayEventHelper e = new HappyBirthdayEventHelper(EventIds.BirthdayDatingLeah, 19954, 2, conditions, new EventStartData("playful", 12, 7, new EventStartData.FarmerData(7, 9, EventHelper.FacingDirection.Up), new List<EventStartData.NPCData>() {
                new EventStartData.NPCData(leah,11,14, EventHelper.FacingDirection.Up),
            }));
            e.addObject(11, 12, 220);
            e.globalFadeIn();
            e.moveFarmerUp(2, EventHelper.FacingDirection.Up, false);
            e.npcFaceDirection(leah, EventHelper.FacingDirection.Left);
            e.speakWithTranslatedMessage(leah, "DatingLeahBirthday_Leah:0"); //0

            e.moveFarmerRight(3, EventHelper.FacingDirection.Down, false);
            e.moveFarmerDown(7, EventHelper.FacingDirection.Right, false);

            e.speakWithTranslatedMessage(leah, "DatingLeahBirthday_Leah:1"); //1
            e.emoteFarmer_Happy();
            e.speakWithTranslatedMessage(leah, "DatingLeahBirthday_Leah:2");//2
            e.speakWithTranslatedMessage(leah, "DatingLeahBirthday_Leah:3");//3
            e.speakWithTranslatedMessage(leah, "DatingLeahBirthday_Leah:4");//4


            e.emoteFarmer_Heart();
            e.emote_Heart("Leah");
            e.globalFadeOut(0.010);
            e.setViewportPosition(-100, -100);
            e.addTranslatedMessageToBeShown("DatingLeahBirthday_Finish:0"); //maru party finish 0
            e.addTranslatedMessageToBeShown("DatingLeahBirthday_Finish:1"); //maru party finish 0
            e.addObjectToPlayersInventory(220, 1, false);
            e.addTranslatedMessageToBeShown("PartyOver");
            e.end();
            return e;
        }

        /// <summary>
        /// Birthday event for when the player is dating Penny.
        /// Status: Completed.
        /// </summary>
        /// <returns></returns>
        public static HappyBirthdayEventHelper DatingBirthday_Penny()
        {

            NPC penny = Game1.getCharacterFromName("Penny");
            NPC pam = Game1.getCharacterFromName("Pam");

            List<EventPrecondition> conditions = new List<EventPrecondition>();
            conditions.Add(new FarmerBirthdayPrecondition());
            conditions.Add(new GameLocationPrecondition(Game1.getLocationFromName("Trailer")));
            conditions.Add(new TimeOfDayPrecondition(600, 2600));
            conditions.Add(new DatingNPCEventPrecondition(penny));
            conditions.Add(new IsStardewValleyExpandedInstalledPrecondition(true));

            //conditions.Add(new StardustCore.Events.Preconditions.NPCSpecific.DatingNPC(Game1.getCharacterFromName("Penny"));
            HappyBirthdayEventHelper e = new HappyBirthdayEventHelper(EventIds.BirthdayDatingPennyTrailer, 19951, 2, conditions, new EventStartData("playful", 12, 8, new EventStartData.FarmerData(12, 9, EventHelper.FacingDirection.Up), new List<EventStartData.NPCData>() {
                new EventStartData.NPCData(penny,12,7, EventHelper.FacingDirection.Up),
                new EventStartData.NPCData(pam,15,4, EventHelper.FacingDirection.Down)
            }));

            e.globalFadeIn();

            e.moveFarmerUp(1, EventHelper.FacingDirection.Up, false);

            e.actorFaceDirection("Penny", EventHelper.FacingDirection.Down);

            //starting = starting.Replace("@", Game1.player.Name);
            e.speakWithTranslatedMessage(penny, "DatingPennyBirthday_Penny:0");
            e.speakWithTranslatedMessage(pam, "DatingPennyBirthday_Pam:0");
            e.speakWithTranslatedMessage(penny, "DatingPennyBirthday_Penny:1");
            e.speakWithTranslatedMessage(pam, "DatingPennyBirthday_Pam:1");
            e.emote_Angry("Penny");
            e.speakWithTranslatedMessage(penny, "DatingPennyBirthday_Penny:2"); //penny2
            e.speakWithTranslatedMessage(penny, "DatingPennyBirthday_Penny:3"); //penny3

            e.moveActorLeft("Penny", 3, EventHelper.FacingDirection.Up, true);
            e.moveFarmerRight(1, EventHelper.FacingDirection.Up, false);
            e.moveFarmerUp(3, EventHelper.FacingDirection.Right, false);
            e.moveFarmerRight(1, EventHelper.FacingDirection.Up, false);
            e.moveActorRight("Penny", 5, EventHelper.FacingDirection.Up, true);
            e.moveActorUp("Penny", 1, EventHelper.FacingDirection.Up, true);
            e.speakWithTranslatedMessage(pam, "DatingPennyBirthday_Pam:2"); //pam2
            e.speakWithTranslatedMessage(penny, "DatingPennyBirthday_Penny:4");//penny4

            e.emoteFarmer_Heart();
            e.emote_Heart("Penny");
            e.globalFadeOut(0.010);
            e.setViewportPosition(-100, -100);
            e.addTranslatedMessageToBeShown("DatingPennyBirthday_Finish:0"); //penny party finish 0
            e.addTranslatedMessageToBeShown("DatingPennyBirthday_Finish:1");// penny party finish 1
            e.addObjectToPlayersInventory(220, 1, false);
            e.addObjectToPlayersInventory(346, 1, false);

            e.addTranslatedMessageToBeShown("PartyOver");

            e.end();

            return e;
        }
    }
}


