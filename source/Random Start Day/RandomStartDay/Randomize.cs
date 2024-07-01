/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
namespace RandomStartDay
{
    internal static class Randomize
    {
        internal static Season[] allowedSeasons;

        internal static int randomizedDayOfMonth = 0;
        internal static Season randomizedSeason = Season.Spring;
        internal static bool isWinter28 = false;
        internal static bool needWheatSeeds = false;

        // randomizing
        public static void RandomizeDate(bool useLegacyRandom)
        {
            // get random seed
            Random random = useLegacyRandom ? new((int)Game1.startingGameSeed) : new();

            // randomize
            // if use legacy random, other random options are disabled
            if (useLegacyRandom)
            {
                randomizedSeason = (Season)random.Next(4);
                randomizedDayOfMonth = random.Next(28) + 1;
            }
            // if not, randomize until it doesn't conflict with config.
            else
            {
                bool conflicts;
                SDate tomorrow;

                do
                {
                    conflicts = false;

                    // set date
                    randomizedSeason = (Season)random.Next(4);
                    if (!ModEntry.config.AlwaysStartAt1st)
                        randomizedDayOfMonth = random.Next(28) + 1;
                    else
                        randomizedDayOfMonth = 28; // set dayOfMonth to 28 to make next day to 1st
                    tomorrow = new SDate(randomizedDayOfMonth, randomizedSeason).AddDays(1);

                    // conflict check
                    if (!allowedSeasons.Contains(tomorrow.Season))
                        conflicts = true;

                    // AlwaysStartAt1st ignores AvoidFestivalDay
                    if (!ModEntry.config.AlwaysStartAt1st)
                    {
                        if (ModEntry.config.AvoidFestivalDay && Utility.isFestivalDay(tomorrow.Day, tomorrow.Season))
                            conflicts = true;
                        else if (ModEntry.config.AvoidPassiveFestivalDay && Utility.IsPassiveFestivalDay(tomorrow.Day, tomorrow.Season, null))
                            conflicts = true;
                    }
                } while (conflicts);
            }

            SDate d = new(randomizedDayOfMonth, randomizedSeason);
            ModEntry.monitor.Log("Randomized Season: " + d.Season + ", Day: " + d.Day + ", as moving day");

            // check if the date is winter 28th, if the option is used
            if (randomizedSeason == Season.Winter && randomizedDayOfMonth == 28 && ModEntry.config.UseWinter28toYear1)
                isWinter28 = true;
        }

        public static bool NeedToGiveWheatSeeds()
        {
            Season s = new SDate(randomizedDayOfMonth, randomizedSeason).AddDays(4 + 1).Season; // season expected to harvest if cultivate 4 days crop now
            // Parsnip * 15 : Winter, Spring
            // Wheat * 18 : Summer, Fall
            if (s == Season.Winter || s == Season.Spring)
                return false;
            else
                return true;
        }

        // applying
        internal static void Apply()
        {
            ApplyDate();
            // if it's need to place wheat seeds, put box
            if (ModEntry.config.UseWheatSeeds)
                PutSeasonalSeeds(NeedToGiveWheatSeeds());
        }

        private static void ApplyDate()
        {

            Game1.dayOfMonth = randomizedDayOfMonth;
            Game1.season = randomizedSeason;

            // refresh all locations
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                // there are initial objects, so call season update method
                location.seasonUpdate(false);
            }

            // make sure outside not dark, for Dynamic Night Time or something similar
            Game1.timeOfDay = 1200;
        }

        private static void PutSeasonalSeeds(bool wheatSeed)
        {
            // continue only if you need wheat seeds
            if (!wheatSeed)
                return;

            GameLocation farmHouse = Game1.RequireLocation<FarmHouse>("FarmHouse");
            Farm farm = Game1.getFarm();
            // set seedbox coordinate
            if (!farm.TryGetMapPropertyAs("FarmHouseStarterSeedsPosition", out Vector2 seedBoxLocation))
            {
                seedBoxLocation = Game1.whichFarm switch
                {
                    1 or 2 or 4 => new Vector2(4f, 7f),
                    3 => new Vector2(2f, 9f),
                    6 => new Vector2(8f, 6f),
                    _ => new Vector2(3f, 7f),
                };
            }

            Chest chest = new(null, Vector2.Zero, true, giftboxIsStarterGift: true);
            // put items in new chest
            if (farm.TryGetMapProperty("FarmHouseStarterGift", out string customStarterGiftString))
            {
                string[] splitedString = customStarterGiftString.Split(' ');
                for (var i = 0; i < splitedString.Length; i += 2)
                {
                    Item item;
                    if (splitedString.Length != i + 1)
                    {
                        // if the item is 15 Parsnip Seeds, replace it with 18 Wheat Seeds.
                        if (splitedString[i] == "(O)472" && splitedString[i + 1] == "15")
                            item = ItemRegistry.Create("(O)483", 18);
                        else
                            item = ItemRegistry.Create(splitedString[i], int.Parse(splitedString[i + 1]));
                    }
                    // if the quantity of the last item is omitted, it is considered 1
                    else
                        item = ItemRegistry.Create(splitedString[i], 1);
                    chest.Items.Add(item);
                }
            }
            else
                chest.Items.Add(ItemRegistry.Create("(O)483", 18));

            // change seed chest
            farmHouse.objects.Remove(seedBoxLocation);
            farmHouse.objects.Add(seedBoxLocation, chest);
        }

        public static void Harmony_Quest6ToWheatQuest(string questId)
        {
            if (ModEntry.config.DisableAll) { return; }

            if (needWheatSeeds && questId == "6")
            {
                Quest questFromId = Quest.getQuestFromId("idermailer.RandomStartDay.harvestWheat");
                if (questFromId != null)
                {
                    Game1.player.questLog.Add(questFromId);
                    Game1.player.removeQuest("6");
                }
                else
                {
                    ModEntry.monitor.Log("Quest \"idermailer.RandomStartDay.harvestWheat\" is not found.");
                }
            }
        }

        public static void Harmony_SetDateForIntro()
        {
            if (ModEntry.config.DisableAll) { return; }

            // RANDOMIZING
            Game1.startingGameSeed ??= new ulong?();
            RandomizeDate(Game1.UseLegacyRandom);
        }

        public static void Harmony_ChangeIntroSeason(ref Texture2D ___roadsideTexture, ref Texture2D ___treeStripTexture)
        {
            if (ModEntry.config.DisableAll) { return; }

            if (randomizedSeason != Season.Spring)
            {
                ___roadsideTexture = Game1.content.Load<Texture2D>("Maps/" + randomizedSeason + "_outdoorsTileSheet");
                ___treeStripTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/treestrip_" + randomizedSeason.ToString().ToLower() + ".png");
                Game1.changeMusicTrack(randomizedSeason.ToString().ToLower() + "_day_ambient");
            }
        }
        public static void Initialize()
        {
            randomizedDayOfMonth = 0;
            randomizedSeason = Season.Spring;
            isWinter28 = false;
            needWheatSeeds = false;
        }
    }
}
