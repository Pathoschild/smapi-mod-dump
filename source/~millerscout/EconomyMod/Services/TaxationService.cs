using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EconomyMod.Helpers;
using EconomyMod.Model;
using EconomyMod.Multiplayer.Messages;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.LookupAnything.Framework.ItemScanning;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.Layers;

namespace EconomyMod
{
    public class TaxationService
    {
        public SaveState State;
        private WorldItemScanner WorldItemScanner;
        public LotValue LotValue;
        public event EventHandler<EventHandlerMessage> OnPayTaxesCompleted;
        public event EventHandler<EventHandlerMessage> OnPostPoneTaxesCompleted;
        public event EventHandler<IEnumerable<TaxSchedule>> OnTaxScheduleListUpdated;


        public TaxationService()
        {
            Util.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            Util.Helper.Events.GameLoop.DayEnding += this.DayEnding;
            Util.Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;

            LotValue = new LotValue();
            LotValue.AddDefaultLotValue();
            if (Util.Config.IncludeOwnedObjectsOnLotValue)
            {
                WorldItemScanner = new WorldItemScanner(Util.Helper.Reflection);

                LotValue.Add("OwnedItems", () =>
                 {
                     var items = WorldItemScanner.GetAllOwnedItems();

                     var ItemPrices = items.Select(c => new
                     {
                         c.Item.DisplayName,
                         price = c.Item.GetType().GetProperty("Price") != null ? (int)c.Item.GetType().GetProperty("Price").GetValue(c.Item) : c.Item.salePrice()
                     });

                     return ItemPrices.Sum(c => c.price);
                 });
            }
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            Util.Helper.Data.WriteJsonFile(Path.Combine("Save", $"{Game1.player.displayName}_{Game1.uniqueIDForThisGame}_data"), State);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            LoadState();

            if (!Game1.player.useSeparateWallets && !Game1.IsServer)
            {
                return;
            }
            this.CalculateLotValue();

            if (NeedToCalculateUpcomingTax())
            {
                State.CalculatedNextTax();
            }
            OnTaxScheduleListUpdated?.Invoke(this, State.AllUnpaidTaxScheduled.GetAllFromThisSeason(Game1.stats.DaysPlayed));

            if (State.HasPendingTax)
            {

                //Util.Monitor.Log($"{Util.Helper.Translation.Get("PostponedPaymentText")}: {State.PendingTaxAmount}.", LogLevel.Info);
                //Util.Monitor.Log($"{Util.Helper.Translation.Get("CurrentLotValueText")}: {CurrentLotValue}.", LogLevel.Info);

                //int Tax = GetAmountByPaymentType(CurrentLotValue);
                var Taxes = State.AllTaxScheduled;

                //Util.Monitor.Log($"[Hardcoded for now] {Util.Helper.Translation.Get("PaymentModeText")}: {Util.Helper.Translation.Get("DailyText")}, {Util.Helper.Translation.Get("TaxValueText")}: {Tax}", LogLevel.Info);

                Util.Monitor.Log($"{Util.Helper.Translation.Get("SeparateWalletsText")}: {Game1.player.useSeparateWallets}", LogLevel.Info);

                foreach (var tax in Taxes)
                {
                    if (Game1.player.Money - tax.Sum <= 0 || Game1.player.Money == 0)
                    {
                        PostponePayment(tax);
                        return;
                    }
                    if (State.PostPoneDaysLeft == 0)
                    {
                        this.PayTaxes(tax);
                        return;
                    }


                    if (tax.Sum * 100 / Game1.player.Money >= Util.Config.ThresholdInPercentageToAskAboutPayment)
                    {
                        Response[] responses = {
                    new Response ("A", $"{Util.Helper.Translation.Get("PayText")} ( {tax.Sum} )G"),
                    new Response ("B", $"{Util.Helper.Translation.Get("PostponeText")} ( {tax.Sum+tax.Sum/5 } ) G")
                };
                        Game1.currentLocation.createQuestionDialogue($"{Util.Helper.Translation.Get("TaxAboveThresholdText")}", responses, (Farmer _, string answer) =>
                        {
                            switch (answer.Split(' ')[0])
                            {
                                case "A":
                                    this.PayTaxes(tax);
                                    break;

                                case "B":
                                    this.PostponePayment(tax);
                                    break;
                            }
                        });

                    }
                    else
                    {
                        this.PayTaxes(tax);
                    }
                }
            }
        }

        private bool NeedToCalculateUpcomingTax()
        {
            var played = Game1.stats.DaysPlayed;
            return !State.ScheduledTax.Any(c => c.DayCount > played);
        }

        private void LoadState()
        {
            State = Util.Helper.Data.ReadJsonFile<SaveState>(Path.Combine("Save", $"{Game1.player.displayName}_{Game1.uniqueIDForThisGame}_data"));
            if (State == null)
                State = new SaveState()
                {
                    ReferenceDaysPlayed = Game1.stats.DaysPlayed,
                    Day = Game1.dayOfMonth
                };

            if (State.ReferenceDaysPlayed == 0) State.ReferenceDaysPlayed = Game1.stats.DaysPlayed;
            if (!State.Day.HasValue || State.Day != Game1.dayOfMonth)
            {
                State.Day = Game1.dayOfMonth;
            }
            if (State.ScheduledTax == null)
            {
                State.ScheduledTax = new List<TaxSchedule>();
            }
        }

        /// <summary>Get a rectangular grid of tiles.</summary>
        /// <param name="x">The X coordinate of the top-left tile.</param>
        /// <param name="y">The Y coordinate of the top-left tile.</param>
        /// <param name="width">The grid width.</param>
        /// <param name="height">The grid height.</param>
        /// Code Taken from Pathoschild's DebugMod.
        public IEnumerable<Vector2> GetTiles(int x, int y, int width, int height)
        {
            for (int curX = x, maxX = x + width - 1; curX <= maxX; curX++)
            {
                for (int curY = y, maxY = y + height - 1; curY <= maxY; curY++)
                    yield return new Vector2(curX, curY);
            }
        }

        private void CalculateLotValue()
        {
            var farm = Game1.getFarm();

            CalculateUsableSoil();


            State.Detailed = new TaxDetailed
            {
                UsableSoil = State.UsableSoil,
                LotValue = LotValue.Sum
            };
            State.Detailed.CurrentDepreciation = CalculateDepreciation();


            int CalculateDepreciation()
            {
                int depreciationObjectsCount = 0;
                foreach (int item in Util.Config.ListOfDepreciationObjects)
                {
                    int count = farm.numberOfObjectsOfType(item, false);
                    depreciationObjectsCount += count;
                    if (count > 0)
                        State.Detailed.AddDepreciation(item, count);
                }
                return depreciationObjectsCount;
            }

            void CalculateUsableSoil()
            {
                if (!State.CalculatedUsableSoil || State.UsableSoil == 0)
                {
                    if (State.UsableSoil > 0) State.UsableSoil = 0;
                    Layer layer = farm.Map.GetLayer("Back");
                    foreach (var tile in this.GetTiles(0, 0, layer.LayerWidth, layer.LayerHeight))
                    {
                        if (farm.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null)
                        {
                            State.UsableSoil++;
                        }
                    }
                    Util.Monitor.Log($"Detected {State.UsableSoil} usable soil.", LogLevel.Info);
                    State.CalculatedUsableSoil = true;

                    /// Failover when we couldn't calculate.
                    if (State.UsableSoil == 0) State.UsableSoil = 3687;

                }
            }

        }
        internal void PayTaxes(TaxSchedule Tax = null)
        {

            if (Tax == null) Tax = State.AllTaxScheduled.FirstOrDefault();
            if (Tax.Sum > Game1.player.Money)
            {
                Game1.addHUDMessage(new HUDMessage(Util.Helper.Translation.Get("PayTax_NotEnoughMoneyText"), 3));
                return;
            }
            Game1.player.Money = Math.Max(0, Game1.player.Money - Tax.Sum);
            Tax.Paid = true;
            State.PostPoneDaysLeft = State.PostPoneDaysLeftDefault;
            Game1.addHUDMessage(new HUDMessage(Util.Helper.Translation.Get("TaxPaidText").ToString().Replace("#Tax#", $"{Tax.Sum}"), 2));


            OnPayTaxesCompleted?.Invoke(this, new EventHandlerMessage(Tax.Sum, Game1.player.IsMale));
            OnTaxScheduleListUpdated?.Invoke(this, State.AllUnpaidTaxScheduled.GetAllFromThisSeason(Game1.stats.DaysPlayed));
        }

        private void PostponePayment(TaxSchedule Tax)
        {
            Game1.addHUDMessage(new HUDMessage(Util.Helper.Translation.Get("PostponedPaymentText"), 2));

            if (State.PostPoneDaysLeft > 0)
                State.PostPoneDaysLeft -= 1;

            Game1.chatBox.addInfoMessage(Util.Helper.Translation.Get("PostponeChatText").ToString().Replace("#playerName#", Game1.player.displayName).Replace("#Tax#", $"{State.PendingTaxAmount}"));
            OnPostPoneTaxesCompleted?.Invoke(this, new EventHandlerMessage(State.PendingTaxAmount, Game1.player.IsMale));

        }
    }

    public class TaxDetailed
    {
        public int UsableSoil { get; set; }
        public int CurrentDepreciation { get; set; }
        public int LotValue { get; set; }
        public int DepreciationPercentage => (100 - (UsableSoil - CurrentDepreciation) * 100 / UsableSoil);

        public int TaxTotal => DepreciationPercentage > 0 ? LotValue / DepreciationPercentage : LotValue;
        public Dictionary<int, int> depreciationList = new Dictionary<int, int>();
        public int CalculateSum(int dayCount = 0)
        {

            int CalculatedSum = TaxTotal;
            switch (Util.Config.TaxPaymentType)
            {
                case TaxPaymentType.Daily:
                    CalculatedSum = TaxTotal / 28 / 4;
                    break;
                case TaxPaymentType.Weekly:
                    CalculatedSum = TaxTotal / 7 / 4;
                    break;
            }
            if (dayCount == 0)
            {
                for (int i = 0; i < Game1.stats.DaysPlayed - dayCount; i++)
                {
                    CalculatedSum += CalculatedSum / 5;
                }
            }
            if (Game1.player.useSeparateWallets)
            {
                int validFarmers = Game1.getAllFarmers().Select(c => c.name).Where(c => !string.IsNullOrEmpty(c)).Count();

                CalculatedSum /= validFarmers;
                //Util.Monitor.Log($"{Util.Helper.Translation.Get("ValidFarmersText")}: {validFarmers}", LogLevel.Info);
                //Util.Monitor.Log($"{Util.Helper.Translation.Get("TaxEachFarmerText")}: {CalculatedSum}", LogLevel.Info);
            }
            return CalculatedSum;
        }

        internal void AddDepreciation(int item, int count)
        {
            depreciationList.Add(item, count);
        }

    }
}
