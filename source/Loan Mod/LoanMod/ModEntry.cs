/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using LoanMod.Common;
using LoanMod.Common.Constants;
using LoanMod.Common.Enums;
using LoanMod.Common.Interfaces;
using StardewValley.Menus;

namespace LoanMod
{
    public partial class ModEntry : Mod
    {
        private ModConfig _config;
        private ILoanManager _loanManager;
        private bool _canSave;
        private int _amount, _duration;
        private float _interest;

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

            _config = helper.ReadConfig<ModConfig>();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.CanPlayerMove)
                return;

            if (!Helper.Input.IsDown(_config.LoanButton)) return;
            StartBorrow(1, ModConstants.BorrowAmountKey);
            Monitor.Log($"{Game1.player.Name} pressed {e.Button} to open Loan Menu.", LogLevel.Debug);
        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (_loanManager == null || !Game1.player.canMove) return;
            switch (_loanManager.CurrentStage)
            {
                case Stages.Borrowing:
                    switch (_amount)
                    {
                        case >= 0 when _duration == 0:
                            StartBorrow(2, ModConstants.BorrowDurationKey);
                            break;
                        case >= 0 when _duration > 0:
                            InitiateBorrow(_amount, _duration, _interest);
                            break;
                    }
                    break;
                case Stages.Repayment:
                    StartBorrow(3, ModConstants.RepayKey);
                    break;
            }
        }

        private void InitiateBorrow(int option, int duration, float interest)
        {
            _loanManager.AmountBorrowed = option;
            _loanManager.Duration = duration;
            _loanManager.Interest = interest;
            _loanManager.Balance = (int)_loanManager.CalculateBalance;
            _loanManager.DailyAmount = (int)_loanManager.CalculateDailyAmount;

            Monitor.Log($"Amount: {option}, Duration: {duration}, Interest: {interest}.", LogLevel.Info);

            Game1.player.addUnearnedMoney(option);

            _loanManager.IsBorrowing = true;
            _loanManager.CurrentStage = Stages.None;

            Monitor.Log($"Is Borrowing: {_loanManager.IsBorrowing}.", LogLevel.Info);

            _amount = 0;
            _duration = 0;
            _interest = 0;

            ExtensionHelper.AddMessage(I18n.Msg_Payment_Credited(_loanManager.AmountBorrowed.ToString("N0")), HUDMessage.achievement_type);

            if (_mobileApi?.GetRunningApp() == Helper.ModRegistry.ModID)
                _mobileApi.SetAppRunning(false);
        }

        private void InitiateRepayment(bool full, bool custom = false)
        {
            if (!_loanManager.IsBorrowing || _loanManager.Balance <= 0)
                return;

            if (custom)
            {
                InitiateCustomRepayment();
            }
            else if (full)
            {
                InitiateFullRepayment();
            }
            else
            {
                InitiateRegularRepayment();
            }
        }

        private void GameLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Current Locale: " + Helper.Translation.LocaleEnum, LogLevel.Info);
            InitMenus();

            //checks if player is currently taking any loans, if so it will load all the loan data.
            if (Game1.player.IsMainPlayer)
                _loanManager = Helper.Data.ReadSaveData<LoanManager>("Doomnik.MoneyManage");

            if (_loanManager != null && !_config.Reset) return;

            _loanManager = new LoanManager();
            _config.Reset = false;
            Helper.WriteConfig(_config);
            ExtensionHelper.AddMessage(I18n.Msg_Create(), HUDMessage.achievement_type);
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (_loanManager.IsBorrowing)
            {
                if (_loanManager.Balance < _loanManager.DailyAmount) _loanManager.DailyAmount = _loanManager.Balance;
                if (_loanManager.HasPaid)
                {
                    _loanManager.AmountRepaidToday = 0;
                    _loanManager.HasPaid = false;
                }
            }
            else
            {
                _loanManager.InitiateReset();
            }

            if (_loanManager.Balance >= 0) return;

            Monitor.Log($"Amount Borrowed vs Repaid: {_loanManager.AmountBorrowed} / {_loanManager.AmountRepaid}, Duration: {_loanManager.Duration}. Interest: {_loanManager.Interest}", LogLevel.Error);
            _loanManager.InitiateReset();
            ExtensionHelper.AddMessage(I18n.Msg_Payment_Error(), HUDMessage.error_type);
        }

        /// <summary>
        /// This method prevents mods like SaveAnytime from interfering with repayments.
        /// </summary>
        private void DayEnding(object sender, DayEndingEventArgs e) => _canSave = true;

        private void Saving(object sender, SavingEventArgs e)
        {
            if (!_canSave) return;

            InitiateRepayment(false);

            if (Context.IsMainPlayer) Helper.Data.WriteSaveData("Doomnik.MoneyManage", _loanManager);
            _canSave = false;
        }
        
        private void InitiateCustomRepayment()
        {
            Game1.activeClickableMenu = new NumberSelectionMenu(I18n.Msg_Startrepay(), (val, _, _) =>
            {
                if (val == _loanManager.Balance)
                {
                    StartBorrow(3, ModConstants.RepayKey);
                    return;
                }

                if (Game1.player.Money >= val) RepayAmount(val);
                Game1.activeClickableMenu = null;
            }, -1, 1, _loanManager.Balance, Math.Min(_loanManager.DailyAmount, _loanManager.Balance));
        }

        private void InitiateFullRepayment()
        {
            if (Game1.player.Money >= _loanManager.Balance)
            {
                RepayAmount(_loanManager.Balance);
                _loanManager.InitiateReset();
                ExtensionHelper.AddMessage(I18n.Msg_Payment_Full(), HUDMessage.achievement_type);
            }
            else
            {
                ExtensionHelper.AddMessage(I18n.Msg_Payment_Failed(_loanManager.Balance.ToString("N0")), HUDMessage.error_type);
            }

            _loanManager.CurrentStage = Stages.Repayment;
        }

        private void InitiateRegularRepayment()
        {
            if (_loanManager.Balance <= 0) return;

            var moneyToRepay = _loanManager.CalculateAmountToPayToday;

            if (Game1.player.Money >= moneyToRepay)
            {
                if (_loanManager.Balance > moneyToRepay)
                {
                    RepayAmount(moneyToRepay);
                }
                else
                {
                    RepayAmount(_loanManager.Balance);
                    _loanManager.IsBorrowing = false;
                    ExtensionHelper.AddMessage(I18n.Msg_Payment_Full(), HUDMessage.achievement_type);
                }

                if (_loanManager.Duration > 0) _loanManager.Duration--;

                _loanManager.HasPaid = true;
                _loanManager.LateDays = 0;
            }
            else
            {
                HandleLatePayment();
            }
        }

        private void RepayAmount(int amount)
        {
            if (amount == 0) return;
            Game1.player.Money -= amount;
            _loanManager.Balance -= amount;
            _loanManager.AmountRepaid += amount;
            _loanManager.AmountRepaidToday += amount;

            ExtensionHelper.AddMessage(I18n.Msg_Payment_Complete(amount.ToString("N0")), HUDMessage.achievement_type);
        }

        private void HandleLatePayment()
        {
            if (_config.LatePaymentChargeRate != 0)
            {
                _loanManager.LateChargeRate = _config.LatePaymentChargeRate;
                _loanManager.LateChargeAmount = (int)_loanManager.CalculateLateFees;
                ExtensionHelper.AddMessage(I18n.Msg_Payment_Failed(_loanManager.DailyAmount.ToString("N0")), HUDMessage.error_type);

                if (_loanManager.LateDays == 0)
                {
                    ExtensionHelper.AddMessage(I18n.Msg_Payment_Missed1(_loanManager.LateChargeAmount.ToString("N0")), HUDMessage.error_type);
                    _loanManager.LateDays++;
                }
                else
                {
                    ExtensionHelper.AddMessage(I18n.Msg_Payment_Missed2(_loanManager.LateChargeAmount.ToString("N0")), HUDMessage.error_type);
                    _loanManager.Balance += _loanManager.LateChargeAmount;
                }
            }

            _loanManager.HasPaid = false;
        }
    }
}