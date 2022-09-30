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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.HappyBirthday.Framework.Constants;
using Omegasis.HappyBirthday.Framework.ContentPack;
using Omegasis.HappyBirthday.Framework.Events.Compatibility;
using Omegasis.HappyBirthday.Framework.Events.EventPreconditions;
using Omegasis.HappyBirthday.Framework.Menus;
using Omegasis.StardustCore.Events;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events
{
    public static class BirthdayEventUtilities
    {
        /// <summary>
        /// A flag used to gate specific dialogue parsing to ensure that the <see cref="HappyBirthdayEventHelper"/> can write the generic spouse data to a json string and parse it properly for when serving it to the game for events.
        /// </summary>
        public static bool NEED_TO_WRITE_DEFAULT_BIRTHDAY_EVENTS_TO_JSON;

        public static EventManager BirthdayEventManager;
        public static bool ShouldAskPlayerForBirthday;
        public static bool ShouldAskPlayerForFavoriteGift;

        public static void Player_Warped(object sender, WarpedEventArgs e)
        {
            StartEventAtLocationIfPossible(e.NewLocation);
        }

        public static void OnDayStarted()
        {
            StartEventAtLocationIfPossible(Game1.player.currentLocation);
        }

        public static void StartEventAtLocationIfPossible(GameLocation location)
        {
            BirthdayEventManager.startEventsAtLocationIfPossible();
        }

        public static void ClearEventsFromFarmer()
        {
            foreach (KeyValuePair<string, EventHelper> v in BirthdayEventManager.events)
                BirthdayEventManager.clearEventFromFarmer(v.Key);
        }

        public static void InitializeBirthdayEventCommands()
        {
            //Dialogue commands.
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.ShowTranslatedMessage", BirthdayEventCommands.showTranslatedMessage);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.ShowTranslatedMessage", BirthdayEventCommands.showTranslatedMessage);
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.SpeakWithBirthdayMessageIncluded", BirthdayEventCommands.speakWithBirthdayIncluded);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.SpeakWithBirthdayMessageIncluded", BirthdayEventCommands.speakWithBirthdayIncluded);
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.SpeakWithTranslatedMessage", BirthdayEventCommands.speakWithTranslatedMessage);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.SpeakWithTranslatedMessage", BirthdayEventCommands.speakWithTranslatedMessage);
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.SpeakIfTodayIsPlayersBirthday", BirthdayEventCommands.speakIfTodayIsPlayersBirthday);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.SpeakIfTodayIsPlayersBirthday", BirthdayEventCommands.speakIfTodayIsPlayersBirthday);

            //Menu commands.
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.ShowBirthdaySelectionMenu", BirthdayEventCommands.setShouldShowChooseBirthdayMenu);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.ShowBirthdaySelectionMenu", BirthdayEventCommands.setShouldShowChooseBirthdayMenu);
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.ShowFavoriteGiftSelectionMenu", BirthdayEventCommands.setShouldShowChooseFavoriteGiftMenu);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.ShowFavoriteGiftSelectionMenu", BirthdayEventCommands.setShouldShowChooseFavoriteGiftMenu);

            //Utility Commands
            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.SkipNextCommand", BirthdayEventCommands.skipNextCommand);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.SkipNextCommand", BirthdayEventCommands.skipNextCommand);

            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.GivePlayerFavoriteGift", BirthdayEventCommands.givePlayerFavoriteGift);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.GivePlayerFavoriteGift", BirthdayEventCommands.givePlayerFavoriteGift);

            BirthdayEventManager.addCustomEventLogic("Omegasis.HappyBirthday.Events.MakeObjectsTemporarilyInvisible", BirthdayEventCommands.makeObjectsTemporarilyInvisible);
            StardustCore.Compatibility.SpaceCore.SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.HappyBirthday.Events.MakeObjectsTemporarilyInvisible", BirthdayEventCommands.makeObjectsTemporarilyInvisible);


            //Additional Preconditions
            BirthdayEventManager.eventPreconditionParsingMethods.Add(FarmerBirthdayPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseFarmerBirthdayPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(SpouseBirthdayPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseSpouseBirthdayPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(HasChosenBirthdayPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseHasChosenBirthdayPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(HasChosenFavoriteGiftPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseHasChosenFavoriteGiftPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(IsMarriedToPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseIsMarriedToPrecondition);

            BirthdayEventManager.eventPreconditionParsingMethods.Add(IsMarriedPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseIsMarriedPrecondition);

            BirthdayEventManager.eventPreconditionParsingMethods.Add(GameLocationIsHomePrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseGameLocationIsHomePrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(FarmHouseLevelPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseFarmHouseLevelPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(YearPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseYearGreaterThanOrEqualToPrecondition);
            BirthdayEventManager.eventPreconditionParsingMethods.Add(VillagersHaveEnoughFriendshipBirthdayPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseVillagersHaveEnoughFriendshipBirthdayPrecondition);

            //Compatibility event preconditions
            BirthdayEventManager.eventPreconditionParsingMethods.Add(IsStardewValleyExpandedInstalledPrecondition.EventPreconditionId, HappyBirthdayPreconditionParsingMethods.ParseIsStardewValleyExpandedInstalledPrecondition);
        }

        public static void InitializeBirthdayEvents()
        {
            foreach(HappyBirthdayContentPack contentPack in HappyBirthdayModCore.Instance.happyBirthdayContentPackManager.getHappyBirthdayContentPacksForCurrentLanguageCode())
            {
                contentPack.loadBirthdayEvents();
            }
        }

        public static void UpdateEventManager()
        {
            if (BirthdayEventManager != null)
                BirthdayEventManager.update();
        }




        /// <summary>
        /// Gets a string to be displayed during the event.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetEventString(string Key)
        {
            string eventString = HappyBirthdayModCore.Instance.translationInfo.getEventString(Key);
            return ReplaceSpecialDialogueTokens(eventString);
        }

        public static string ReplaceSpecialDialogueTokens(string originalString)
        {
            string modifiedString = originalString;
            modifiedString = modifiedString.Replace("{AffectionateSpouseWord}", HappyBirthdayModCore.Instance.birthdayMessages.getAffectionateSpouseWord());
            modifiedString = modifiedString.Replace("{TimeOfDay}", HappyBirthdayModCore.Instance.birthdayMessages.getTimeOfDayString());
            modifiedString = modifiedString.Replace("@", Game1.player.Name);
            return modifiedString;
        }

    }
}
