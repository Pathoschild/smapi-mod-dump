/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace QualityBait
{
    internal class ModEntry : Mod
    {
        internal static IMonitor IMonitor;
        internal static IModHelper IHelper;
        internal static ITranslationHelper ITranslations;
        internal static Config IConfig;
        internal static IApi IApi;

        internal static Dictionary<string, string> Recipes; 
        private static readonly List<int> Qualities = new() { SObject.lowQuality, SObject.medQuality, SObject.highQuality, SObject.bestQuality };

        public override void Entry(IModHelper helper)
        {
            IMonitor = Monitor;
            IHelper = Helper;
            ITranslations = Helper.Translation;
            IConfig = Helper.ReadConfig<Config>();
            validateConfig();

            Helper.Events.Content.AssetRequested += onAssetRequested;

            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.GameLoop.GameLaunched += onGameLaunched;

            Recipes = new()
            {
                { "Bait (Silver)", $"685 5 380 1/Home/685 5/false/Fishing 4/{ITranslations.Get("Bait.1")}" },
                { "Bait (Gold)", $"685 5 384 1/Home/685 5/false/Fishing 7/{ITranslations.Get("Bait.2")}" },
                { "Bait (Iridium)", $"685 5 386 1/Home/685 5/false/Fishing 9/{ITranslations.Get("Bait.3")}" },
                { "Wild Bait (Silver)", $"774 5 380 1/Home/774 5/false/null/{ITranslations.Get("Wild.1")}" },
                { "Wild Bait (Gold)", $"774 5 384 1/Home/774 5/false/null/{ITranslations.Get("Wild.2")}" },
                { "Wild Bait (Iridium)", $"774 5 386 1/Home/774 5/false/null/{ITranslations.Get("Wild.3")}" },
                { "Magic Bait (Silver)", $"908 5 380 1/Home/908 5/false/null/{ITranslations.Get("Magic.1")}" },
                { "Magic Bait (Gold)", $"908 5 384 1/Home/908 5/false/null/{ITranslations.Get("Magic.2")}" },
                { "Magic Bait (Iridium)", $"908 5 386 1/Home/908 5/false/null/{ITranslations.Get("Magic.3")}" },
            };
        }

        public override object GetApi() => IApi ??= new Api();

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(Monitor, Helper);
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
            CraftingRecipe.InitShared();
        }

        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.FishingLevel >= 4)
            {
                if (!Game1.player.knowsRecipe("Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Bait (Silver)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Silver)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Silver)", 0);
            }
            if (Game1.player.FishingLevel >= 7)
            {
                if (!Game1.player.knowsRecipe("Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Bait (Gold)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Gold)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Gold)", 0);
            }
            if (Game1.player.FishingLevel >= 9)
            {
                if (!Game1.player.knowsRecipe("Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Bait (Iridium)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Iridium)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Iridium)", 0);
            }
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    foreach (var item in Recipes)
                        data[item.Key] = item.Value;
                });
            }
        }

        private void validateConfig()
        {
            if (IConfig.ChancePercentage > 100)
                IConfig.ChancePercentage = 100;
            if (IConfig.ChancePercentage < 0)
                IConfig.ChancePercentage = 0;
            Helper.WriteConfig(IConfig);
        }

        internal static int GetQualityForCatch(int originalQuality, int baitQuality)
        {
            if (originalQuality == SObject.bestQuality || baitQuality == SObject.lowQuality || originalQuality >= baitQuality)
                return originalQuality;
            return Game1.random.NextDouble() <= IConfig.Chance ? baitQuality : Qualities[Qualities.IndexOf(baitQuality) - 1];
        }
    }
}
