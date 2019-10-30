using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using PyTK.CustomTV;
using PyTK.Extensions;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace AnimalHusbandryMod.animals
{
    public class LivingWithTheAnimalsChannel
    {
        private TemporaryAnimatedSprite _showSprite;

        private readonly Dictionary<int, Episode> _episodes;
        public static readonly string LivingWithTheAnimals = "LivingWithTheAnimals";

        public LivingWithTheAnimalsChannel()
        {
            this._episodes = new Dictionary<int, Episode>();

            //SPRING 1
            /*02*/_episodes.Add(1, new Episode("TV.LivingWithTheAnimals.Episode.Buildings", false, false, false)); 
            /*06*/_episodes.Add(2, new Episode("TV.LivingWithTheAnimals.Episode.Coop", false, false, false));
            /*09*/_episodes.Add(3, new Episode("TV.LivingWithTheAnimals.Episode.Silo", false, false, false));
            /*13*/_episodes.Add(4, new Episode("TV.LivingWithTheAnimals.Episode.Barn", false, false, false));
            /*16*/_episodes.Add(5, new Episode("TV.LivingWithTheAnimals.Episode.FeedingBasket", false, false, true));
            /*20*/_episodes.Add(6, new Episode("TV.LivingWithTheAnimals.Episode.CaveCarrot", false, false, true));
            /*23*/_episodes.Add(7, new Episode("TV.LivingWithTheAnimals.Episode.SpringAnimalContest", false, false, false, true));
            /*27*/_episodes.Add(8, new Episode("TV.LivingWithTheAnimals.Episode.Wheat", false, false, true));
            //SUMMER 1
            /*02*/_episodes.Add(9, new Episode("TV.LivingWithTheAnimals.Episode.CatDogTreat", false, false, true));
            /*06*/_episodes.Add(10, new Episode("TV.LivingWithTheAnimals.Episode.Radish", false, false, true));
            /*09*/_episodes.Add(11, new Episode("TV.LivingWithTheAnimals.Episode.TreatQuality", false, false, true));
            /*13*/_episodes.Add(12, new Episode("TV.LivingWithTheAnimals.Episode.TreatMilk", false, false, true));
            /*16*/_episodes.Add(13, new Episode("TV.LivingWithTheAnimals.Episode.Insemination", false, true, false));
            /*20*/_episodes.Add(14, new Episode("TV.LivingWithTheAnimals.Episode.Grape", false, false, true));
            /*23*/_episodes.Add(15, new Episode("TV.LivingWithTheAnimals.Episode.MeatPrice", true, false, false));
            /*27*/_episodes.Add(16, new Episode("TV.LivingWithTheAnimals.Episode.BokChoy", false, false, true));
            //FALL 1
            /*02*/_episodes.Add(17, new Episode("TV.LivingWithTheAnimals.Episode.TreatIsNotHay", false, false, true));
            /*06*/_episodes.Add(18, new Episode("TV.LivingWithTheAnimals.Episode.TreatPrice", false, false, true));
            /*09*/_episodes.Add(19, new Episode("TV.LivingWithTheAnimals.Episode.PregnancyTime", false, true, false));
            /*13*/_episodes.Add(20, new Episode("TV.LivingWithTheAnimals.Episode.Corn", false, false, true));
            /*16*/_episodes.Add(21, new Episode("TV.LivingWithTheAnimals.Episode.PregnancyRoom", false, true, false));
            /*20*/_episodes.Add(22, new Episode("TV.LivingWithTheAnimals.Episode.MeatDishesBuffs", true, false, false));
            /*23*/_episodes.Add(23, new Episode("TV.LivingWithTheAnimals.Episode.FallAnimalContest", false, false, false, true));
            /*27*/_episodes.Add(24, new Episode("TV.LivingWithTheAnimals.Episode.Heater", false, false, false));
            //WINTER 1
            /*02*/_episodes.Add(25, new Episode("TV.LivingWithTheAnimals.Episode.ChikenCowMeat", true, false, false));
            /*06*/_episodes.Add(26, new Episode("TV.LivingWithTheAnimals.Episode.AgeAnimalContest", false, false, false, true));
            /*09*/_episodes.Add(27, new Episode("TV.LivingWithTheAnimals.Episode.ProfessionAnimalProductQuality", false, false, false));
            /*13*/_episodes.Add(28, new Episode("TV.LivingWithTheAnimals.Episode.TreatAverageAnimalContest", false, false, true, true));
            /*16*/_episodes.Add(29, new Episode("TV.LivingWithTheAnimals.Episode.RabbitPregnancy", false, true, false));
            /*20*/_episodes.Add(30, new Episode("TV.LivingWithTheAnimals.Episode.FriendshipAnimalContest", false, false, false, true));
            /*23*/_episodes.Add(31, new Episode("TV.LivingWithTheAnimals.Episode.Kale", false, false, true));
            /*27*/_episodes.Add(32, new Episode("TV.LivingWithTheAnimals.Episode.Cauliflower", false, false, true));
            //SPRING 2
            /*02*/_episodes.Add(33, new Episode("TV.LivingWithTheAnimals.Episode.CrystalFruit", false, false, true));
            /*06*/_episodes.Add(34, new Episode("TV.LivingWithTheAnimals.Episode.ParticipantAnimalContest", false, false, false, true));
            /*09*/_episodes.Add(35, new Episode("TV.LivingWithTheAnimals.Episode.BeanHotpot", false, false, true));
            /*13*/_episodes.Add(36, new Episode("TV.LivingWithTheAnimals.Episode.TreatVarietyAnimalContest", false, false, true, true));
            /*16*/
            /*20*/_episodes.Add(38, new Episode("TV.LivingWithTheAnimals.Episode.SpringAnimalContest", false, false, false, true, true));
            /*23*/_episodes.Add(39, new Episode("TV.LivingWithTheAnimals.Episode.Apple", false, false, true));
            /*27*/_episodes.Add(40, new Episode("TV.LivingWithTheAnimals.Episode.Melon", false, false, true));
            //SUMMER 2
            /*02*/_episodes.Add(41, new Episode("TV.LivingWithTheAnimals.Episode.HeritageAnimalContest", false, false, false, true));
            /*06*/
            /*09*/_episodes.Add(43, new Episode("TV.LivingWithTheAnimals.Episode.RedCabbage", false, false, true));
            /*13*/
            /*16*/
            /*20*/
            /*23*/
            /*27*/_episodes.Add(48, new Episode("TV.LivingWithTheAnimals.Episode.Amaranth", false, false, true));
            //FALL 2
            /*02*/_episodes.Add(49, new Episode("TV.LivingWithTheAnimals.Episode.Yam", false, false, true));
            /*06*/_episodes.Add(50, new Episode("TV.LivingWithTheAnimals.Episode.TreatAverageAnimalContest", false, false, true, true, true));
            /*09*/_episodes.Add(51, new Episode("TV.LivingWithTheAnimals.Episode.Artichoke", false, false, true));
            /*13*/_episodes.Add(52, new Episode("TV.LivingWithTheAnimals.Episode.Pumpkin", false, false, true));
            /*16*/_episodes.Add(53, new Episode("TV.LivingWithTheAnimals.Episode.PigsWinterPart1", false, true, false));
            /*20*/_episodes.Add(54, new Episode("TV.LivingWithTheAnimals.Episode.AgeAnimalContest", false, false, false, true, true));
            /*23*/_episodes.Add(55, new Episode("TV.LivingWithTheAnimals.Episode.FallAnimalContest", false, false, false, true, true));
            /*27*/_episodes.Add(56, new Episode("TV.LivingWithTheAnimals.Episode.PigWinterPart2", false, true, false));
            // WINTER 2
            /*02*/_episodes.Add(57, new Episode("TV.LivingWithTheAnimals.Episode.PigWinterPart3", false, true, true));
            /*06*/_episodes.Add(58, new Episode("TV.LivingWithTheAnimals.Episode.FriendshipAnimalContest", false, false, false, true, true));
            /*09*/_episodes.Add(59, new Episode("TV.LivingWithTheAnimals.Episode.ParticipantAnimalContest", false, false, false, true, true));
            /*13*/_episodes.Add(60, new Episode("TV.LivingWithTheAnimals.Episode.TreatVarietyAnimalContest", false, false, true, true, true));
            /*16*/_episodes.Add(61, new Episode("TV.LivingWithTheAnimals.Episode.PigWinterPart4", false, true, false));
            /*20*/_episodes.Add(62, new Episode("TV.LivingWithTheAnimals.Episode.PigWinterPart5", false, true, true));
            /*23*/_episodes.Add(63, new Episode("TV.LivingWithTheAnimals.Episode.HeritageAnimalContest", false, false, false, true, true));
            /*27*/_episodes.Add(64, new Episode("TV.LivingWithTheAnimals.Episode.Retirement", false, false, false));
        }

        public void CheckChannelDay()
        {
            CustomTVMod.removeKey(LivingWithTheAnimals);

            if (SDate.Now().DayOfWeek == DayOfWeek.Tuesday || SDate.Now().DayOfWeek == DayOfWeek.Saturday)
            {
                if (GetCurrentEpisode() is Episode episode )
                {
                    string name = DataLoader.i18n.Get("TV.LivingWithTheAnimals.ChannelDisplayName");
                    if (episode.Rerun)
                    {
                        name += " " + DataLoader.i18n.Get("TV.LivingWithTheAnimals.ReRunDisplaySuffix"); ;
                    }
                    
                    CustomTVMod.addChannel(LivingWithTheAnimals, name, ShowAnnouncement);
                }
            }
        }

        private Episode GetCurrentEpisode()
        {
            if (this._episodes.TryGetValue(GetShowNumber(),out Episode episode))
            {
                if ((!episode.AboutMeat || !DataLoader.ModConfig.DisableMeat)
                    && (!episode.AboutPregnancy || !DataLoader.ModConfig.DisablePregnancy)
                    && (!episode.AboutTreats || !DataLoader.ModConfig.DisableTreats)
                    && (!episode.AboutContest || !DataLoader.ModConfig.DisableAnimalContest))
                {
                    return episode;
                }
            }
            return null;
        }

        private static int GetShowNumber()
        {
            return (int)(Game1.stats.DaysPlayed % 224U / 3.5) + 1;
        }

        private void ShowAnnouncement(TV tv, TemporaryAnimatedSprite sprite, StardewValley.Farmer farmer, string answer)
        {
            _showSprite = new TemporaryAnimatedSprite(DataLoader.LooseSpritesName, new Rectangle(0, 0, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            CustomTVMod.showProgram(_showSprite, DataLoader.i18n.Get("TV.LivingWithTheAnimals.Announcement"), ShowPresentation);
        }

        private void ShowPresentation()
        {
            string text = DataLoader.i18n.Get(_episodes[GetShowNumber()].Text);
            if (text.Contains("|"))
            {
                if (DataLoader.ModConfig.DisableMeat)
                {
                    text = text.Split('|')[1];
                }
                else
                {
                    text = text.Split('|')[0];
                }
            }
            CustomTVMod.showProgram(_showSprite, text, CustomTVMod.endProgram);
        }
    }

    public class Episode
    {
        public String Text { get; }
        public bool AboutMeat { get; }
        public bool AboutPregnancy { get; }
        public bool AboutTreats { get; }
        public bool AboutContest { get; }
        public bool Rerun { get; }

        public Episode(string text, bool aboutMeat, bool aboutPregnancy, bool aboutTreats, bool aboutContest = false, bool rerun = false)
        {
            Text = text;
            AboutMeat = aboutMeat;
            AboutPregnancy = aboutPregnancy;
            AboutTreats = aboutTreats;
            AboutContest = aboutContest;
            Rerun = rerun;
        }
    }
}
