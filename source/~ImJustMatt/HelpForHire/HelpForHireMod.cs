using System;
using System.Collections.Generic;
using System.Linq;
using LeFauxMatt.CustomChores;
using LeFauxMatt.HelpForHire.Menus;
using LeFauxMatt.HelpForHire.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LeFauxMatt.HelpForHire
{
    
    public class HelpForHireMod : Mod
    {
        internal static HelpForHireMod Instance { get; private set; }
        internal static ICustomChoresApi CustomChoresApi { get; private set; }

        /*********
        ** Fields
        *********/
        /// <summary>A list of chores with assets and config related to the shop menu.</summary>
        private readonly IDictionary<string, ChoreHandler> _chores = new Dictionary<string, ChoreHandler>(StringComparer.OrdinalIgnoreCase);

        internal static string PurchasedLabel;
        internal static string NotPurchasedLabel;
        internal static string InsufficientFundsLabel;
        internal static string TotalCosts;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            Instance = this;
            Helper.ReadConfig<ModConfig>();
            PurchasedLabel = Helper.Translation.Get("label.purchased");
            NotPurchasedLabel = Helper.Translation.Get("label.notPurchased");
            InsufficientFundsLabel = Helper.Translation.Get("label.insufficientFunds");
            TotalCosts = Helper.Translation.Get("label.totalCosts");

            // add console commands
            Helper.ConsoleCommands.Add("chores_OpenShop", "Opens the chores shop.\n\nUsage: chores_OpenShop", OpenShop);

            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Displays a shop that sells chore services when the 'chores_openShop' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void OpenShop(string command, string[] args)
        {
            if (!_chores.Any() && !UpdateChores())
                return;

            if (Game1.activeClickableMenu is null)
                Game1.activeClickableMenu = new ChoreMenu(_chores);
        }

        /****
        ** Event handlers
        ****/
        /// <summary>The method invoked on the first update tick, once all mods are initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // init
            CustomChoresApi = Helper.ModRegistry.GetApi<ICustomChoresApi>("furyx639.CustomChores");
        }

        /// <summary>The method invoked when a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var insufficientFunds = false;
            foreach (var choreHandler in _chores)
            {
                var chore = choreHandler.Value;

                if (!chore.IsPurchased || !CustomChoresApi.CheckChore(chore.ChoreName))
                    continue;

                if (Game1.player.Money < chore.EstimatedCost)
                {
                    insufficientFunds = true;
                    break;
                }

                try
                {
                    if (!CustomChoresApi.DoChore(chore.ChoreName))
                        continue;

                    Game1.playSound("purchaseClick");
                    Game1.player.Money -= chore.ActualCost;
                    Monitor.Log($"Successfully performed chore {choreHandler.Key}");
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to perform chore {choreHandler.Key}:\n{ex}", LogLevel.Error);
                    throw;
                }
            }

            if (!insufficientFunds)
                return;

            // Insufficient Funds
            foreach (var choreHandler in _chores)
            {
                choreHandler.Value.IsPurchased = false;
            }

            Game1.addHUDMessage(new HUDMessage(InsufficientFundsLabel, 3));
            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || !(Game1.activeClickableMenu is null))
                return;

            if (!_chores.Any() && !UpdateChores())
                return;

            if (ModConfig.Instance.ShopMenuButton == e.Button)
                Game1.activeClickableMenu = new ChoreMenu(_chores);
        }

        private bool UpdateChores()
        {
            _chores.Clear();

            // get chores
            foreach (var choreKey in CustomChoresApi.GetChores())
            {
                var chore = CustomChoresApi.GetChore(choreKey);
                ModConfig.Instance.Chores.TryGetValue(choreKey, out var price);
                if (price > 0)
                    _chores.Add(choreKey, new ChoreHandler(chore, price));
            }

            return _chores.Any();
        }
    }
}
