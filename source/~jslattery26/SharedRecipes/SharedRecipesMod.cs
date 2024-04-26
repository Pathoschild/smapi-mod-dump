/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jslattery26/stardew_mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Threading.Tasks;

#nullable enable
namespace SharedRecipes
{
    public class SharedRecipesMod : Mod
    {
        public static IMonitor? StaticMonitor { get; private set; }
        internal static SharedRecipesMod? ModInstance { get; private set; }
        public static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor?.Log(s, l);
        public const string ModUniqueId = "jslattery26.SharedRecipes";
        internal const string UserConfigFilename = "config.json";
        private ModConfig? Config;

        public override void Entry(IModHelper helper)
        {
            StaticMonitor = Monitor;
            ModInstance = this;
            Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }


        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.Register(ModManifest, Helper.ModRegistry, Monitor,
                getConfig: () => Config,
                reset: () => Config = new(),
                save: () =>
                {
                    if (Config != null)
                    {
                        Helper.WriteConfig(Config);
                    }
                }
            );
        }
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            SyncFarmerRecipes(delayForStart: true);
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Config != null && Config.Keys.SyncRecipes.JustPressed())
                SyncFarmerRecipes();

        }

        private static void LogToScreen(Farmer farmer, Farmer otherFarmer, int count)
        {
            if (farmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                Game1.addHUDMessage(new HUDMessage($"Shared {count} recipes with {otherFarmer.Name}", HUDMessage.newQuest_type) { timeLeft = 1000 });
            }
            if (otherFarmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                Game1.addHUDMessage(new HUDMessage($"Received {count} recipes from {farmer.Name}", HUDMessage.newQuest_type) { timeLeft = 1000 });
            }
        }
        private async void SyncFarmerRecipes(bool delayForStart = false)
        {
            if (Config == null) return;

            List<Farmer> farmers = Game1.getAllFarmers().ToList();

            if (farmers.Count < 2) return;

            Log($"Checking {farmers.Count} farmers for shared recipes..");

            if (delayForStart) await Task.Delay(3000);
            int zeroCount = 0;
            foreach (Farmer farmer in farmers)
            {

                Log($"{farmer.Name} has {farmer.craftingRecipes.Count()} crafting and {farmer.cookingRecipes.Count()} cooking recipes.");
                foreach (Farmer otherFarmer in farmers)
                {
                    int count = 0;
                    if (farmer.UniqueMultiplayerID == otherFarmer.UniqueMultiplayerID)
                        continue;
                    var craftingRecipesToShare = farmer.craftingRecipes.Keys.Except(otherFarmer.craftingRecipes.Keys).ToHashSet();
                    var cookingRecipesToShare = farmer.cookingRecipes.Keys.Except(otherFarmer.cookingRecipes.Keys).ToHashSet();

                    if (craftingRecipesToShare.Count == 0 && cookingRecipesToShare.Count == 0)
                    {
                        zeroCount++;
                        continue;
                    }

                    if (Config.ShareCrafting)
                    {
                        count += craftingRecipesToShare.Count;
                        foreach (var recipe in craftingRecipesToShare)
                            otherFarmer.craftingRecipes[recipe] = farmer.craftingRecipes[recipe];
                        Log($"{farmer.Name} shared {craftingRecipesToShare.Count} crafting recipes with {otherFarmer.Name}.");
                    }

                    if (Config.ShareCooking)
                    {
                        count += cookingRecipesToShare.Count;
                        foreach (var recipe in cookingRecipesToShare)
                            otherFarmer.cookingRecipes[recipe] = farmer.cookingRecipes[recipe];
                        Log($"{farmer.Name} shared {cookingRecipesToShare.Count} cooking recipes with {otherFarmer.Name}.");
                    }
                    LogToScreen(farmer, otherFarmer, count);
                }
            }
            if (!delayForStart && zeroCount == farmers.Count) Game1.addHUDMessage(new HUDMessage($"All Synced. Ur Good", HUDMessage.achievement_type) { timeLeft = 1000 });

        }
    }
}
