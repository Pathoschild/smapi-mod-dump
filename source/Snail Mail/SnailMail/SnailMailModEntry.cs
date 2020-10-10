/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Snail-Mail
**
*************************************************/

using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using Bookcase;
using Bookcase.Events;
using Bookcase.Lib;
using Bookcase.Utils;
using Bookcase.Mail;
using Bookcase.Registration;
using Microsoft.Xna.Framework;

namespace SnailMail {

    public class SnailMailModEntry : Mod {

        internal static Log logger;

        internal static Letter letterBlacksmithFinished;
        internal static Letter letterSecretWoodsHideAndSeek;
        internal static Letter letterDishOfTheDay;
        internal static Letter letterDishOfTheDaySample;
        internal static Letter letterLegendarySpring;
        internal static Letter letterLegendarySummer;
        internal static Letter letterLegendaryFall;
        internal static Letter letterLegendaryWinter;
        internal static Letter letterLegendarySewer;
        internal static Letter letterBills;

        public override void Entry(IModHelper helper) {

            logger = new Log(this);

            TimeEvents.AfterDayStarted += OnDayChanged;

            letterBlacksmithFinished = new Letter("snailmail:blacksmith_update", "Hey there @^^I've finished the work on your %tool%. Make sure you pick it up the next time you're in town.^^-Clint");
            letterBlacksmithFinished.PreProcessor = ProcessToolUpgrade;
            Registries.Mail.Register(letterBlacksmithFinished);

            letterSecretWoodsHideAndSeek = new Letter("snailmail:secret_woods_hide_and_seek", "Hi @^^My friends and I would play hide and seek in a small part of the woods near your farm. There was a big storm one day and an old tree fell down and blocked the path. I know you have all sorts of tools on your farm, do you think you can help clear the path for us?^^-Jas");
            Registries.Mail.Register(letterSecretWoodsHideAndSeek);

            letterDishOfTheDay = new Letter("snailmail:dish_of_the_day", "Hey @^^I made some of my special %dish_of_day% today. Stop by the Stardrop Saloon later on if you want some.^^-Gus");
            letterDishOfTheDay.PreProcessor = ProcessDishOfTheDay;
            Registries.Mail.Register(letterDishOfTheDay);

            letterDishOfTheDaySample = new Letter("snailmail:dish_of_day_sample", "Hey @^^I made some of my special %dish_of_day% today. Here is a fresh sample. Stop by the Stardrop Saloon later on if you want some more.^^-Gus");
            letterDishOfTheDaySample.PreProcessor = ProcessDishOfTheDay;
            letterDishOfTheDaySample.Callback = CallbackDishOfTheDay;
            Registries.Mail.Register(letterDishOfTheDaySample);

            letterLegendarySpring = new Letter("snailmail:legendary_fish_spring", "Dear @^^When I was a young boy, my pappy would tell stories about large fish in the mountain lake. I've never caught one, but I have seen some massive shadows in the lake during the rain.^^-Willy");
            Registries.Mail.Register(letterLegendarySpring);

            letterLegendarySummer = new Letter("snailmail:legendary_fish_summer", "Hey @^^I was sitting at the eastern docks today and saw strange red fish. You should check it out some time.^-Elliot");
            Registries.Mail.Register(letterLegendarySummer);

            letterLegendaryFall = new Letter("snailmail:legendary_fish_fall", "Dear @^^I was walking by the JojaMart last night and noticed some weird lights in the river. Do you think that could be some sort of fish?^-Willy");
            Registries.Mail.Register(letterLegendaryFall);

            letterLegendaryWinter = new Letter("snailmail:legendary_fish_winter", "Dear @^^I was walking down the river into Cinder sap forest and noticed something strange moving in the water. I thought it was a chunk of ice at first but then I saw it dart into the deeper water. It may be worth having a look.^^-Willy");
            Registries.Mail.Register(letterLegendaryWinter);

            // letterBills = new Letter("snailmail:bills", "Dear @^^It is tax season. The amount due is %taxes%. This will be taken from your account. Residents of Stardew Valley who have more than 10k gold must pay 5% of their gold in taxes, up to 5k gold. Taxes will be used for community events and upkeeping the town.^^Thank you for your support.^-Mayor Lewis", preProcessor: ProcessTaxes);
            // Registries.Mail.Register(letterBills);
        }

        private String ProcessTaxes(Letter letter, String contents) {

            int taxMoney = (int) Math.Floor(Game1.player.money * 0.05d);
            return contents.Replace("%taxes%", $"{taxMoney}");
        }

        private String ProcessDishOfTheDay(Letter letter, String contents) {

            return contents.Replace("%dish_of_day%", Game1.dishOfTheDay.DisplayName);
        }

        private void CallbackDishOfTheDay(Letter letter, LetterViewerMenu gui) {

            if (Game1.dishOfTheDay != null) {

                gui.itemsToGrab.Add(new ClickableComponent(new Rectangle(gui.xPositionOnScreen + gui.width / 2 - 48, gui.yPositionOnScreen + gui.height - 32 - 96, 96, 96), (Item)Game1.dishOfTheDay.getOne()) {
                    myID = 104,
                    leftNeighborID = 101,
                    rightNeighborID = 102
                });
            }
        }

        private String ProcessToolUpgrade(Letter letter, String contents) {

            if (Game1.player != null) {

                Tool tool = Game1.player.toolBeingUpgraded.Value;
                String toolName = tool != null ? tool.DisplayName : "ERROR";
                return contents.Replace("%tool%", toolName);
            }

            return contents;
        }

        private void OnDayChanged(object obj, EventArgs args) {

            Farmer player = Game1.player;

            if (player != null) {

                String season = Game1.currentSeason;
                int fishingLevel = player.FishingLevel;

                switch (season) {

                    case ("spring"):

                        if (fishingLevel >= 10 && !letterLegendarySpring.HasRecieved(player)) {
                            letterLegendarySpring.DeliverMail(player, immediately: true);
                        }

                        break;

                    case ("summer"):

                        if (fishingLevel >= 5 && !letterLegendarySummer.HasRecieved(player)) {
                            letterLegendarySummer.DeliverMail(player, immediately: true);
                        }

                        break;

                    case ("winter"):

                        if (fishingLevel >= 6 && !letterLegendaryWinter.HasRecieved(player)) {
                            letterLegendaryWinter.DeliverMail(player, immediately: true);
                        }

                        break;

                    case ("fall"):

                        if (fishingLevel >= 3 && !letterLegendaryFall.HasRecieved(player)) {
                            letterLegendaryFall.DeliverMail(player, immediately: true);
                        }
                        break;

                    default:
                        logger.Error($"Unknown season {season}");
                        break;
                }

                // 10% chance to deliver the dish of the day notice.
                if (MathsUtils.TryPercentage(0.10)) {

                    // 5% chance for the dish of the day to include a sample.
                    if (MathsUtils.TryPercentage(0.05)) {

                        letterDishOfTheDaySample.DeliverMail(Game1.player, allowDuplicates: true, immediately: true);
                    }

                    // Otherwise the player just gets the notification.
                    else {

                        letterDishOfTheDay.DeliverMail(Game1.player, allowDuplicates: true, immediately: true);
                    }
                }

                // Deliver secret woods letter if the player doesn't have it, and has an axe that can break the log.
                if (!letterSecretWoodsHideAndSeek.HasRecieved(Game1.player) && DoesFarmerHaveToolTier(Game1.player, "Axe", 2)) {

                    letterSecretWoodsHideAndSeek.DeliverMail(Game1.player, immediately: true);
                }

                Tool tool = Game1.player.toolBeingUpgraded.Value;
                int daysLeft = Game1.player.daysLeftForToolUpgrade.Value;

                // If Clint has a tool ready for delivery, send the notif letter.
                if (tool != null && daysLeft == 0) {

                    letterBlacksmithFinished.DeliverMail(Game1.player, allowDuplicates: true, immediately: true);
                }
            }
        }

        private static bool DoesFarmerHaveToolTier(Farmer farmer, String name, int tier) {

            Tool toolFromName = farmer.getToolFromName(name);

            if (toolFromName != null) {

                return toolFromName.UpgradeLevel >= tier;
            }

            return false;
        }
    }
}