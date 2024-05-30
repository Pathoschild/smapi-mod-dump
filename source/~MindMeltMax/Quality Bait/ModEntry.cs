/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using Object = StardewValley.Object;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace QualityBait
{
    public class ModEntry : Mod
    {
        internal static IMonitor IMonitor;
        internal static IModHelper IHelper;
        internal static ITranslationHelper ITranslations;
        internal static Config IConfig;
        internal static IApi IApi;

        internal static Dictionary<string, string> Recipes; 
        private static readonly List<int> Qualities = [Object.lowQuality, Object.medQuality, Object.highQuality, Object.bestQuality];

        public override void Entry(IModHelper helper)
        {
            IMonitor = Monitor;
            IHelper = Helper;
            ITranslations = Helper.Translation;
            IConfig = Helper.ReadConfig<Config>();
            validateConfig();

            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.Content.LocaleChanged += onLocaleChanged;

            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.GameLoop.GameLaunched += onGameLaunched;

            reloadRecipes();
        }

        private void onLocaleChanged(object sender, LocaleChangedEventArgs e)
        {
            reloadRecipes();
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
            CraftingRecipe.InitShared();
        }

        public override object GetApi() => IApi ??= new Api();

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(ModManifest.UniqueID);
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
            CraftingRecipe.InitShared();

            registerForGMCM();
        }

        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.FishingLevel >= 4)
            {
                if (!Game1.player.knowsRecipe("Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Bait (Silver)", 0);
                if (!Game1.player.knowsRecipe("Deluxe Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Deluxe Bait (Silver)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Silver)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Silver)", 0);
                if (Game1.player.knowsRecipe("Challenge Bait") && !Game1.player.knowsRecipe("Challenge Bait (Silver)"))
                    Game1.player.craftingRecipes.Add("Challenge Bait (Silver)", 0);
            }
            if (Game1.player.FishingLevel >= 7)
            {
                if (!Game1.player.knowsRecipe("Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Bait (Gold)", 0);
                if (!Game1.player.knowsRecipe("Deluxe Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Deluxe Bait (Gold)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Gold)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Gold)", 0);
                if (Game1.player.knowsRecipe("Challenge Bait") && !Game1.player.knowsRecipe("Challenge Bait (Gold)"))
                    Game1.player.craftingRecipes.Add("Challenge Bait (Gold)", 0);
            }
            if (Game1.player.FishingLevel >= 9)
            {
                if (!Game1.player.knowsRecipe("Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Bait (Iridium)", 0);
                if (!Game1.player.knowsRecipe("Deluxe Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Deluxe Bait (Iridium)", 0);
                if (Game1.player.knowsRecipe("Wild Bait") && !Game1.player.knowsRecipe("Wild Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Wild Bait (Iridium)", 0);
                if (Game1.player.knowsRecipe("Magic Bait") && !Game1.player.knowsRecipe("Magic Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Magic Bait (Iridium)", 0);
                if (Game1.player.knowsRecipe("Challenge Bait") && !Game1.player.knowsRecipe("Challenge Bait (Iridium)"))
                    Game1.player.craftingRecipes.Add("Challenge Bait (Iridium)", 0);
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
            IConfig.ChancePercentage = IConfig.ChancePercentage > 100 ? 100 : (IConfig.ChancePercentage < 0 ? 0 : IConfig.ChancePercentage);
            Helper.WriteConfig(IConfig);
        }

        private void registerForGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => IConfig = new(), () => validateConfig());

            gmcm.AddNumberOption(ModManifest, () => IConfig.ChancePercentage, (x) => IConfig.ChancePercentage = x, () => ITranslations.Get("Config.ChancePercentage.Title"), () => ITranslations.Get("Config.ChancePercentage.Description"), 0, 100);

            gmcm.AddBoolOption(ModManifest, () => IConfig.BaitMakerQuality, (x) => IConfig.BaitMakerQuality = x, () => ITranslations.Get("Config.BaitMakerQuality.Title"), () => ITranslations.Get("Config.BaitMakerQuality.Description"));

            gmcm.AddBoolOption(ModManifest, () => IConfig.ForceLowerQuality, (x) => IConfig.ForceLowerQuality = x, () => ITranslations.Get("Config.ForceLowerQuality.Title"), () => ITranslations.Get("Config.ForceLowerQuality.Description"));
        }

        private void reloadRecipes()
        {
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
                { "Deluxe Bait (Silver)", $"DeluxeBait 5 380 1/Home/DeluxeBait 5/false/Fishing 4/{ITranslations.Get("Deluxe.1")}" },
                { "Deluxe Bait (Gold)", $"DeluxeBait 5 384 1/Home/DeluxeBait 5/false/Fishing 7/{ITranslations.Get("Deluxe.2")}" },
                { "Deluxe Bait (Iridium)", $"DeluxeBait 5 386 1/Home/DeluxeBait 5/false/Fishing 9/{ITranslations.Get("Deluxe.3")}" },
                { "Challenge Bait (Silver)", $"ChallengeBait 5 380 1/Home/ChallengeBait 5/false/null/{ITranslations.Get("Challenge.1")}" },
                { "Challenge Bait (Gold)", $"ChallengeBait 5 384 1/Home/ChallengeBait 5/false/null/{ITranslations.Get("Challenge.2")}" },
                { "Challenge Bait (Iridium)", $"ChallengeBait 5 386 1/Home/ChallengeBait 5/false/null/{ITranslations.Get("Challenge.3")}" },
            };
        }

        internal static int GetQualityForCatch(string itemId, int originalQuality, int baitQuality)
        {
            if (ItemRegistry.IsQualifiedItemId(itemId))
            {
                itemId = itemId.Split(')')[1];
                IMonitor.Log($"Received QualifiedItemId {itemId} for {nameof(GetQualityForCatch)}, converting to non-qualified variant");
            }
            if (originalQuality == Object.bestQuality || baitQuality == Object.lowQuality || originalQuality >= baitQuality || IsTrashObject(new Object(itemId, 1)))
                return originalQuality;
            return Game1.random.NextDouble() <= IConfig.Chance ? baitQuality : Qualities[Qualities.IndexOf(baitQuality) - 1];
        }

        internal static int GetQualityForBait(int originalQuality, int inputQuality)
        {
            if (originalQuality == Object.bestQuality || inputQuality == Object.lowQuality || originalQuality >= inputQuality)
                return originalQuality;
            return Game1.random.NextDouble() <= IConfig.Chance ? inputQuality : Qualities[Qualities.IndexOf(inputQuality) - 1];
        }

        internal static bool IsTrashObject(Object obj) => obj.Category == Object.junkCategory || obj.Category != Object.FishCategory;
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
