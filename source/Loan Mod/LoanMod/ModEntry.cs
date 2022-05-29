/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace LoanMod
{
    public partial class ModEntry : Mod
    {
        internal ModConfig Config;
        private bool canSave;
        private bool borrowProcess, repayProcess;
        private int amount, duration;
        private float interest;
        private LoanManager loanManager;

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) => AddModFunctions();

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += GameLoaded;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.Display.MenuChanged += MenuChanged;

            Config = helper.ReadConfig<ModConfig>();
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.CanPlayerMove)
                return;

            if (Helper.Input.IsDown(Config.LoanButton))
            {
                StartBorrow(1, "Key_Amount");
                Monitor.Log($"{Game1.player.Name} pressed {e.Button} to open Loan Menu.", LogLevel.Debug);
            }
        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (borrowProcess && Game1.player.canMove)
            {
                if (amount >= 0 && duration == 0)
                    StartBorrow(2, "Key_Duration");
                else if (amount >= 0 && duration > 0)
                    InitiateBorrow(amount, duration, interest);
            }

            if (repayProcess && Game1.player.canMove)
                StartBorrow(3, "Key_Repay");
        }

        private void InitiateBorrow(int option, int duration, float interest)
        {
            loanManager.AmountBorrowed = option;
            loanManager.Duration = duration;
            loanManager.Interest = interest;
            loanManager.Balance = (int)loanManager.CalculateBalance;
            loanManager.DailyAmount = (int)loanManager.CalculateInitDailyAmount;

            Monitor.Log($"Amount: {option}, Duration: {duration}, Interest: {interest}.", LogLevel.Info);

            Game1.player.Money += option;

            loanManager.IsBorrowing = true;
            borrowProcess = false;

            Monitor.Log($"Is Borrowing: {loanManager.IsBorrowing}.", LogLevel.Info);

            amount = 0;
            this.duration = 0;
            this.interest = 0;

            AddMessage(I18n.Msg_Payment_Credited(loanManager.AmountBorrowed.ToString("N0")), HUDMessage.achievement_type);

            if (mobileApi?.GetRunningApp() == Helper.ModRegistry.ModID)
                mobileApi.SetAppRunning(false);
        }

        private void InitiateRepayment(bool full, bool custom = false)
        {
            if (loanManager.IsBorrowing && loanManager.Balance > 0 )
            {
                //check if player wants to repay in full.
                if (custom)
                {
                    Game1.activeClickableMenu = new NumberSelectionMenu(I18n.Msg_Startrepay(), (val, cost, farmer) =>
                    {
                        //if user chooses to repay full amount from custom menu
                        if (val == loanManager.Balance)
                        {
                            StartBorrow(3, "Key_Repay");
                            return;
                        }

                        if (Game1.player.Money >= val)
                        {
                            Game1.player.Money -= val;
                            loanManager.AmountRepaid += val;
                            loanManager.Balance -= val;
                            //recalculate daily amount in case balance is lower than daily repayment
                            loanManager.AmountRepaidToday += val;
                            AddMessage(I18n.Msg_Payment_Complete(val.ToString("N0")), HUDMessage.achievement_type);
                        }
                        Game1.activeClickableMenu = null;
                    }, -1, 1, loanManager.Balance, Math.Min(loanManager.DailyAmount, loanManager.Balance));
                }
                else if (full)
                {
                    if (Game1.player.Money >= loanManager.Balance)
                    {   //Repays the remaining balance
                        Game1.player.Money -= loanManager.Balance;
                        loanManager.InitiateReset();
                        AddMessage(I18n.Msg_Payment_Full(), HUDMessage.achievement_type);
                    }
                    else
                    {
                        AddMessage(I18n.Msg_Payment_Failed(loanManager.Balance.ToString("N0")), HUDMessage.error_type);
                    }
                    repayProcess = false;
                    return;
                } //Check if you are still in loan contract
                else if (loanManager.Balance > 0)
                {
                    int moneyToRepay = loanManager.CalculateAmountToPayToday;
                    //If player has enough Money for the daily deduction amount
                    if (Game1.player.Money >= moneyToRepay)
                    {
                        //Checks if the balance is greater than or equal to the daily repayment amount
                        if (loanManager.Balance > moneyToRepay)
                        {
                            Game1.player.Money -= moneyToRepay;
                            loanManager.AmountRepaid += moneyToRepay;
                            loanManager.Balance -= moneyToRepay;
                        }
                        else
                        {
                            //Repays the remaining balance
                            Game1.player.Money -= loanManager.Balance;
                            loanManager.IsBorrowing = false;
                            AddMessage(I18n.Msg_Payment_Full(), HUDMessage.achievement_type);
                        }
                        loanManager.HasPaid = true;

                        if (loanManager.Duration > 0) loanManager.Duration--;
                        loanManager.LateDays = 0;
                    }
                    else
                    {
                        if (Config.LatePaymentChargeRate != 0)
                        {
                            loanManager.LateChargeRate = Config.LatePaymentChargeRate;
                            loanManager.LateChargeAmount = (int)loanManager.CalculateLateFees;
                            AddMessage(I18n.Msg_Payment_Failed(loanManager.DailyAmount.ToString("N0")), HUDMessage.error_type);
                            if (loanManager.LateDays == 0)
                            {
                                AddMessage(I18n.Msg_Payment_Missed1(loanManager.LateChargeAmount.ToString("N0")), HUDMessage.error_type);
                                loanManager.LateDays++;
                            }
                            else
                            {
                                AddMessage(I18n.Msg_Payment_Missed2(loanManager.LateChargeAmount.ToString("N0")), HUDMessage.error_type);
                                loanManager.Balance += loanManager.LateChargeAmount;
                            }
                        }
                        loanManager.HasPaid = false;
                    }
                }
            }
        }

        private void GameLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Current Locale: " + Helper.Translation.LocaleEnum, LogLevel.Info);
            InitMenus();

            //checks if player is currently taking any loans, if so it will load all the loan data.
            if (Game1.player.IsMainPlayer)
                loanManager = this.Helper.Data.ReadSaveData<LoanManager>("Doomnik.MoneyManage"); 

            if (loanManager == null || Config.Reset)
            {
                loanManager = new LoanManager();
                Config.Reset = false;
                Helper.WriteConfig<ModConfig>(Config);
                AddMessage(I18n.Msg_Create(), HUDMessage.achievement_type);
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            //checks if player has made payment.
            if (loanManager.IsBorrowing)
            {
                if (loanManager.HasPaid)
                {
                    if (loanManager.DailyAmount > 0) 
                        AddMessage(I18n.Msg_Payment_Complete(loanManager.CalculateAmountToPayToday.ToString("N0")), HUDMessage.achievement_type);

                    loanManager.AmountRepaidToday = 0;
                    loanManager.HasPaid = false;
                }
                if (loanManager.Balance < loanManager.DailyAmount) loanManager.DailyAmount = loanManager.Balance;
            }
            else
            {
                loanManager.InitiateReset();
            }

            if (loanManager.Balance < 0)
            {
                Monitor.Log($"Amount Borrowed vs Repaid: {loanManager.AmountBorrowed} / {loanManager.AmountRepaid}, Duration: {loanManager.Duration}. Interest: {loanManager.Interest}", LogLevel.Info);
                loanManager.InitiateReset();
                AddMessage(I18n.Msg_Payment_Error(), HUDMessage.error_type);
            }
        }

        /// <summary>
        /// This method prevents mods like SaveAnytime from interfering with repayments.
        /// </summary>
        private void DayEnding(object sender, DayEndingEventArgs e) => canSave = true;

        private void Saving(object sender, SavingEventArgs e)
        {
            if (canSave)
            {
                InitiateRepayment(false);
                if (Context.IsMainPlayer)
                {
                    Helper.Data.WriteSaveData("Doomnik.MoneyManage", loanManager);
                }

                canSave = false;
            }
        }
    }
}