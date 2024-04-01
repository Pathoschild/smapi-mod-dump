/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using MailFrameworkMod;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System;
using System.Text;
using StardewValley.Buildings;

namespace MarketTown;
    class MailData
    {

        public int TotalEarning { get; set; }
        public int SellMoney { get; set; } = 0;
        public string SellList { get; set; } = "";

        public int TodayCustomerInteraction = 0;

        public int TodayMuseumVisitor { get; set; } = 0;
        public int ForageSold { get; set; } = 0;
        public int FlowerSold { get; set; } = 0;
        public int FruitSold { get; set; } = 0;
        public int VegetableSold { get; set; } = 0;
        public int SeedSold { get; set; } = 0;
        public int MonsterLootSold { get; set; } = 0;
        public int SyrupSold { get; set; } = 0;
        public int ArtisanGoodSold { get; set; } = 0;
        public int AnimalProductSold { get; set; } = 0;
        public int ResourceMetalSold { get; set; } = 0;
        public int MineralSold { get; set; } = 0;
        public int CraftingSold { get; set; } = 0;
        public int CookingSold { get; set; } = 0;
        public int FishSold { get; set; } = 0;
        public int GemSold { get; set; } = 0;


        public int TotalForageSold { get; set; } = 0;
        public int TotalFlowerSold { get; set; } = 0;
        public int TotalFruitSold { get; set; } = 0;
        public int TotalVegetableSold { get; set; } = 0;
        public int TotalSeedSold { get; set; } = 0;
        public int TotalMonsterLootSold { get; set; } = 0;
        public int TotalSyrupSold { get; set; } = 0;
        public int TotalArtisanGoodSold { get; set; } = 0;
        public int TotalAnimalProductSold { get; set; } = 0;
        public int TotalResourceMetalSold { get; set; } = 0;
        public int TotalMineralSold { get; set; } = 0;
        public int TotalCraftingSold { get; set; } = 0;
        public int TotalCookingSold { get; set; } = 0;
        public int TotalFishSold { get; set; } = 0;
        public int TotalGemSold { get; set; } = 0;
    }

    internal class MailLoader
    {
        public static ITranslationHelper I18N;

        public static ModConfig ModConfig;

        public MailLoader(IModHelper modHelper)
        {
            var letterTexture = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("Assets/LtBG.png");

            // Init Market Town letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.Welcome",
                    modHelper.Translation.Get("foodstore.letter.mtwelcome"),
                    (Letter l) => !Game1.player.mailReceived.Contains("MT.Welcome") || Game1.dayOfMonth == 1 ,
                    delegate (Letter l)
                    {
                        ((NetHashSet<string>)(object)Game1.player.mailReceived).Add(l.Id);
                    })
                {
                    Title = "Welcome to Market Town",
                    LetterTexture = letterTexture
                }
            );
            MailData model = null;

            if (Game1.IsMasterGame)
            {
                model = modHelper.Data.ReadSaveData<MailData>("MT.MailLog");
            }

            if (model == null) { return; }

            // Museum License Letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.MuseumLicense",
                    modHelper.Translation.Get("foodstore.letter.mtmuseumlicense"),
                    (Letter l) => !Game1.player.mailReceived.Contains("MT.MuseumLicense") && Game1.netWorldState.Value.MuseumPieces.Count() >= 30,
                    delegate (Letter l)
                    {
                        ((NetHashSet<string>)(object)Game1.player.mailReceived).Add(l.Id);
                    })
                {
                    Title = "Museum License available",
                    LetterTexture = letterTexture
                }
            );

            // Restaurant License Letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.RestaurantLicense",
                    modHelper.Translation.Get("foodstore.letter.mtrestaurantlicense"),
                    (Letter l) => !Game1.player.mailReceived.Contains("MT.RestaurantLicense") && model.TotalEarning >= 10000 && model.TotalCookingSold >= 20,
                    delegate (Letter l)
                    {
                        ((NetHashSet<string>)(object)Game1.player.mailReceived).Add(l.Id);
                    })
                {
                    Title = "Restaurant License available",
                    LetterTexture = letterTexture
                }
            );

                // Market Town License Letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.MarketTownLicense",
                    modHelper.Translation.Get("foodstore.letter.mtmarkettownlicense"),
                    (Letter l) => !Game1.player.mailReceived.Contains("MT.MarketTownLicense") && model.TotalEarning >= 30000 && model.TotalForageSold >= 30 && model.TotalVegetableSold >= 30 && model.TotalArtisanGoodSold >= 30 && model.TotalAnimalProductSold >= 30 && model.TotalCookingSold >= 30 && model.TotalFishSold >= 30 && model.TotalMineralSold >= 30,
                    delegate (Letter l)
                    {
                        ((NetHashSet<string>)(object)Game1.player.mailReceived).Add(l.Id);
                    })
                {
                    Title = "Market Town License available",
                    LetterTexture = letterTexture
                }
            );



            // Dynamic Letter --------------------------------------------------------------
            string categoryCountsString = GetCategoryCountsString(model, modHelper);



            // Daily Log letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.SellLogMail",
                    modHelper.Translation.Get("foodstore.mailtotal", 
                    new { totalEarning = model.TotalEarning, sellMoney = model.SellMoney, todayCustomerInteraction = model.TodayCustomerInteraction }) 
                                        + modHelper.Translation.Get("foodstore.todaymuseumvisitor", new {todayMMuseumVisitor = model.TodayMuseumVisitor}) + model.SellList,
                    (Letter l) => model.SellMoney != 0 || model.TodayMuseumVisitor != 0)
                {
                    LetterTexture = letterTexture
                }
            );

            // Weekly Log letter
            MailRepository.SaveLetter(
                new Letter(
                    "MT.WeeklyLogMail",
                    categoryCountsString,
                    (Letter l) => Game1.dayOfMonth == 1 || Game1.dayOfMonth == 8 || Game1.dayOfMonth == 15 || Game1.dayOfMonth == 22)
                {
                    LetterTexture = letterTexture
                }
            );
        }

        public string GetCategoryCountsString(MailData model, IModHelper modHelper)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(modHelper.Translation.Get("foodstore.weeklytotallog", new { totalEarning = model.TotalEarning }));

            stringBuilder.Append(modHelper.Translation.Get("foodstore.forage", new { TotalForageSold = model.TotalForageSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.flower", new { TotalFlowerSold = model.TotalFlowerSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.fruit", new { TotalFruitSold = model.TotalFruitSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.vegetable", new { TotalVegetableSold = model.TotalVegetableSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.seed", new { TotalSeedSold = model.TotalSeedSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.monsterloot", new { TotalMonsterLootSold = model.TotalMonsterLootSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.syrup", new { TotalSyrupSold = model.TotalSyrupSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.artisangood", new { TotalArtisanGoodSold = model.TotalArtisanGoodSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.animalproduct", new { TotalAnimalProductSold = model.TotalAnimalProductSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.resourcemetal", new { TotalResourceMetalSold = model.TotalResourceMetalSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.mineral", new { TotalMineralSold = model.TotalMineralSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.crafting", new { TotalCraftingSold = model.TotalCraftingSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.cooking", new { TotalCookingSold = model.TotalCookingSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.fish", new { TotalFishSold = model.TotalFishSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.gem", new { TotalGemSold = model.TotalGemSold }));


            stringBuilder.Append($"------------------------------------------^");

            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.forage", new { LastweekForageSold = model.ForageSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.flower", new { LastweekFlowerSold = model.FlowerSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.fruit", new { LastweekFruitSold = model.FruitSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.vegetable", new { LastweekVegetableSold = model.VegetableSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.seed", new { LastweekSeedSold = model.SeedSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.monsterloot", new { LastweekMonsterLootSold = model.MonsterLootSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.syrup", new { LastweekSyrupSold = model.SyrupSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.artisangood", new { LastweekArtisanGoodSold = model.ArtisanGoodSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.animalproduct", new { LastweekAnimalProductSold = model.AnimalProductSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.resourcemetal", new { LastweekResourceMetalSold = model.ResourceMetalSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.mineral", new { LastweekMineralSold = model.MineralSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.crafting", new { LastweekCraftingSold = model.CraftingSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.cooking", new { LastweekCookingSold = model.CookingSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.fish", new { LastweekFishSold = model.FishSold }));
            stringBuilder.Append(modHelper.Translation.Get("foodstore.lastweek.gem", new { LastweekGemSold = model.GemSold }));



            return stringBuilder.ToString();
        }
    }