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
using Microsoft.Xna.Framework;
using Omegasis.StardustCore.Events;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events
{
    public static class EventHelperExtentions
    {
        /// <summary>
        /// Adds the event command to show the translated message.
        /// </summary>
        /// <param name="eventHelper"></param>
        /// <param name="MessageKey"></param>
        public static void addTranslatedMessageToBeShown(this EventHelper eventHelper, string MessageKey)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.ShowTranslatedMessage ");
            b.Append(MessageKey);
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Adds the event command to ask the player for their birthday.
        /// </summary>
        /// <param name="eventHelper"></param>
        public static void addAskForBirthday(this EventHelper eventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.ShowBirthdaySelectionMenu");
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Adds the event command to ask the player for their favorite gift.
        /// </summary>
        /// <param name="eventHelper"></param>
        public static void addAskForFavoriteGift(this EventHelper eventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.ShowFavoriteGiftSelectionMenu");
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Adds the event command to show specific dialogue if it is the players birthday. Otherwise, show an alternative message.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="SpeakerName"></param>
        /// <param name="PreconditionString"></param>
        /// <param name="TrueMessage"></param>
        /// <param name="FalseMessage"></param>
        public static void speakIfTodayIsPlayersBirthday(this EventHelper helper, string SpeakerName, string TrueMessageKey, string FalseMessageKey)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.SpeakIfTodayIsPlayersBirthday");
            helper.addEventData(b);
            speakWithTranslatedMessage(helper, SpeakerName, TrueMessageKey);
            skipNextCommand(helper);
            speakWithBirthdayIncluded(helper, SpeakerName, FalseMessageKey); //TODO: CHange this to GetEventString()


        }

        /// <summary>
        /// Adds the event command to skip the next command in the current event. Used for event branching.
        /// </summary>
        /// <param name="eventHelper"></param>
        public static void skipNextCommand(this EventHelper eventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.SkipNextCommand");
            eventHelper.addEventData(b);
        }

        public static void speakWithBirthdayIncluded(this EventHelper eventHelper, string SpeakerName, string OriginalMessageKey)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.SpeakWithBirthdayMessageIncluded");
            b.Append(" ");
            b.Append(SpeakerName);
            b.Append(" ");
            b.Append(OriginalMessageKey);
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Creates the command to get a specific translation string and have a given npc speak that returned string.
        /// </summary>
        /// <param name="eventHelper"></param>
        /// <param name="SpeakerName"></param>
        /// <param name="MessageKey"></param>
        public static void speakWithTranslatedMessage(this EventHelper eventHelper, string SpeakerName, string MessageKey)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.SpeakWithTranslatedMessage");
            b.Append(" ");
            b.Append(SpeakerName);
            b.Append(" ");
            b.Append(MessageKey);
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Gives the player their favorite gift.
        /// </summary>
        /// <param name="eventHelper"></param>
        public static void givePlayerFavoriteGift(this EventHelper eventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.GivePlayerFavoriteGift");
            eventHelper.addEventData(b);
        }

        /// <summary>
        /// Creates the command to get a specific translation string and have a given npc speak that returned string.
        /// </summary>
        /// <param name="eventHelper"></param>
        /// <param name="SpeakerName"></param>
        /// <param name="MessageKey"></param>
        public static void speakWithTranslatedMessage(this EventHelper eventHelper, NPC Speaker, string MessageKey)
        {
            speakWithTranslatedMessage(eventHelper, Speaker.Name, MessageKey);
        }

        /// <summary>
        /// Makes all objects at a game location temporarily invisible.
        /// </summary>
        /// <param name="eventHelper"></param>
        public static void makeAllObjectsTemporarilyInvisible(this EventHelper eventHelper, List<Vector2> TilePositions)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.HappyBirthday.Events.MakeObjectsTemporarilyInvisible ");

            for(int i = 0; i < TilePositions.Count; i++)
            {
                Vector2 tile = TilePositions[i];
                b.Append(tile.X);
                b.Append(" ");
                b.Append(tile.Y);
                if (i != TilePositions.Count - 1)
                {
                    b.Append(" ");
                }
            }


            eventHelper.addEventData(b);

        }

    }
}
