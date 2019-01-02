using System;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using System.Linq;

namespace StardewNotification
{
    public class GeneralNotification
    {
        public void DoNewDayNotifications(ITranslationHelper Trans)
        {
            CheckForBirthday(Trans);
            CheckForFestival(Trans);
            CheckForMaxLuck(Trans);
            CheckForQueenOfSauce(Trans);
            CheckForToolUpgrade(Trans);
            CheckForTravelingMerchant(Trans);
            CheckForHayLevel(Trans);
        }

        public void CheckForHayLevel(ITranslationHelper Trans)
        {
            if (!StardewNotification.Config.NotifyHay || Game1.getFarm().buildings.Count(b => b.buildingType.Value == "Silo") == 0)
                return;

            int hayAmt = Game1.getFarm().piecesOfHay.Value;
            if (hayAmt > 0)
                Util.ShowMessage(Trans.Get("hayMessage", new { hayAmt = Game1.getFarm().piecesOfHay.Value}));
            else if (StardewNotification.Config.ShowEmptyhay)
                Util.ShowMessage(Trans.Get("noHayMessage"));
        }
        
        public void DoBirthdayReminder(ITranslationHelper Trans)
        {
            var character = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            if (!(character is null) && Game1.player.friendshipData[character.Name].GiftsToday != 1)
            {
                Util.ShowMessage(Trans.Get("birthdayReminder", new { charName = character.displayName }));
            }
        }

        private void CheckForBirthday(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyBirthdays)
            {
                var character = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
                if (character is null) return;
                Util.ShowMessage(Trans.Get("birthday", new { charName = character.displayName }));
            }
        }

        private void CheckForTravelingMerchant(ITranslationHelper Trans)
        {
            Forest f = Game1.getLocationFromName("Forest") as Forest;
            if (!StardewNotification.Config.NotifyTravelingMerchant || !f.travelingMerchantDay)
            {
                return;
            }

            Util.ShowMessage(Trans.Get("travelingMerchant"));
        }

        private void CheckForToolUpgrade(ITranslationHelper Trans)
        {
            if (!StardewNotification.Config.NotifyToolUpgrade) return;
            if (!(Game1.player.toolBeingUpgraded.Value is null) && Game1.player.daysLeftForToolUpgrade.Value <= 0)
                Util.ShowMessage(Trans.Get("toolPickup", new { toolName = Game1.player.toolBeingUpgraded.Value.Name }));
        }

        private void CheckForMaxLuck(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyMaxLuck && Game1.dailyLuck > 0.07)
                Util.ShowMessage(Trans.Get("luckyDay"));
            else if (StardewNotification.Config.NotifyMinLuck && Game1.dailyLuck < -.07)
                Util.ShowMessage(Trans.Get("unluckyDay"));
        }


        private void CheckForQueenOfSauce(ITranslationHelper Trans)
        {
            if (!StardewNotification.Config.NotifyQueenOfSauce) return;
            var dayName = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (!dayName.Equals("Sun")) return;
            Util.ShowMessage(Trans.Get("queenSauce"));
        }

        private void CheckForFestival(ITranslationHelper Trans)
        {
            if (!StardewNotification.Config.NotifyFestivals || !Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason)) return;
            var festivalName = GetFestivalName(Trans);
            Util.ShowMessage(Trans.Get("fMsg", new { fest = festivalName }));

            if (!festivalName.Equals(Trans.Get("WinterStar"))) return;
            Random r = new Random((int)(Game1.uniqueIDForThisGame / 2uL) ^ Game1.year ^ (int)Game1.player.UniqueMultiplayerID);
            var santa = Utility.getRandomTownNPC(r).displayName;

            Util.ShowMessage(Trans.Get("SecretSantaReminder", new { charName = santa })); 
        }

        private string GetFestivalName(ITranslationHelper Trans)
        {
            var season = Game1.currentSeason;
            var day = Game1.dayOfMonth;
            switch (season)
            {
                case "spring":
                    if (day == 13) return Trans.Get("EggFestival");
                    if (day == 24) return Trans.Get("FlowerDance");
                    break;
                case "summer":
                    if (day == 11) return Trans.Get("Luau");
                    if (day == 28) return Trans.Get("MoonlightJellies");
                    break;
                case "fall":
                    if (day == 16) return Trans.Get("ValleyFair");
                    if (day == 27) return Trans.Get("SpiritsEve");
                    break;
                case "winter":
                    if (day == 8) return Trans.Get("IceFestival");
                    if (day == 14) return Trans.Get("NightFestival");
                    if (day == 15) return Trans.Get("NightFestival");
                    if (day == 16) return Trans.Get("NightFestival");
                    if (day == 25) return Trans.Get("WinterStar");
                    break;
                default:
                    break;
            }
            return Trans.Get("festival");
        }
    }
}
