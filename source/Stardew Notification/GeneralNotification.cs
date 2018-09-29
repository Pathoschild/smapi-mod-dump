using System;

using StardewValley;

namespace StardewNotification
{
    public class GeneralNotification
    {
        public void DoNewDayNotifications()
        {
            CheckForBirthday();
            CheckForFestival();
            CheckForMaxLuck();
            CheckForQueenOfSauce();
            CheckForToolUpgrade();
            CheckForTravelingMerchant();
        }

        public void DoBirthdayReminder()
        {
            var character = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            if (!ReferenceEquals(character, null) && Game1.player.friendships[character.name][3] != 1)
            {
                Util.showMessage(string.Format(Constants.BIRTHDAY_REMINDER, character.name));
            }
        }

        private void CheckForBirthday()
        {
            if (!Util.Config.notifyBirthdays) return;
            var character = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            if (ReferenceEquals(character, null)) return;
            Util.showMessage(string.Format(Constants.BIRTHDAY, character.name));

        }

        private void CheckForTravelingMerchant()
        {
            if (!Util.Config.notifyTravelingMerchant || Game1.dayOfMonth % 7 % 5 != 0) return;
            Util.showMessage(Constants.TRAVELING_MERCHANT);
        }

        private void CheckForToolUpgrade()
        {
            if (!Util.Config.notifyToolUpgrade) return;
            if (!ReferenceEquals(Game1.player.toolBeingUpgraded, null) && Game1.player.daysLeftForToolUpgrade <= 0)
                Util.showMessage(string.Format(Constants.TOOL_PICKUP, Game1.player.toolBeingUpgraded.name));
        }

        private void CheckForMaxLuck()
        {
            if (!Util.Config.notifyMaxLuck || Game1.dailyLuck < 0.07) return;
            Util.showMessage(Constants.LUCKY_DAY);
        }


        private void CheckForQueenOfSauce()
        {
            if (!Util.Config.notifyQueenOfSauce) return;
            var dayName = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (!dayName.Equals(Constants.SUN)) return;
            Util.showMessage(Constants.QUEEN_OF_SAUCE);
        }

        private void CheckForFestival()
        {
            if (!Util.Config.notifyFestivals || !Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason)) return;
            var festivalName = GetFestivalName();
            Util.showMessage(string.Format(Constants.FESTIVAL_MSG, festivalName));
            if (!festivalName.Equals(Constants.WINTER_STAR)) return;
            var rng = new Random((int)(Game1.uniqueIDForThisGame / 2UL) - Game1.year);
            var santa = Utility.getRandomTownNPC(rng, Utility.getFarmerNumberFromFarmer(Game1.player)).name;
            Util.showMessage(string.Format(Constants.SECRET_SANTA_REMINDER, santa));
        }

        private string GetFestivalName()
        {
            var season = Game1.currentSeason;
            var day = Game1.dayOfMonth;
            switch (season)
            {
                case Constants.SPRING:
                    if (day == 13) return Constants.EGG_FESTIVAL;
                    if (day == 24) return Constants.FLOWER_DANCE;
                    break;
                case Constants.SUMMER:
                    if (day == 11) return Constants.LUAU;
                    if (day == 28) return Constants.MOONLIGHT_JELLIES;
                    break;
                case Constants.FALL:
                    if (day == 16) return Constants.VALLEY_FAIR;
                    if (day == 27) return Constants.SPIRIT_EVE;
                    break;
                case Constants.WINTER:
                    if (day == 8) return Constants.ICE_FESTIVAL;
                    if (day == 25) return Constants.WINTER_STAR;
                    break;
                default:
                    break;
            }
            return Constants.FESTIVAL;
        }
    }
}
