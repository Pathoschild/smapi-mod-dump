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
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using LoanMod.Common.Constants;
using LoanMod.Common.Enums;

namespace LoanMod
{
    public partial class ModEntry
    {
        private List<Response> _menuItems, _repayMenuItems, _durationMenu, _menuYesNo;

        private void StartBorrow(int stage, string key)
        {
            var context = Game1.currentLocation;
            if (!_loanManager.IsBorrowing)
            {
                switch (stage)
                {
                    case 1:
                        if (_config.CustomMoneyInput)
                            Game1.activeClickableMenu = new NumberSelectionMenu(I18n.Msg_Startborrow1(), (val, cost, farmer) => ProcessBorrowing(val, key), -1, 100, _config.MaxBorrowAmount, 500);
                        else
                            context.createQuestionDialogue(I18n.Msg_Startborrow1(), _menuItems.ToArray(), BorrowMenu);
                        break;
                    case 2:
                        context.createQuestionDialogue(I18n.Msg_Startborrow2(), _durationMenu.ToArray(), BorrowDuration);
                        break;
                }
                return;
            }

            switch (stage)
            {
                case 1:
                    context.createQuestionDialogue(I18n.Msg_Menu1(), _repayMenuItems.ToArray(), RepayMenu);
                    break;
                case 3:
                    context.createQuestionDialogue(I18n.Msg_Menu2(_loanManager.Balance.ToString("N0")), _menuYesNo.ToArray(), RepayFullMenu);
                    break;
            }
        }

        private void InitMenus()
        {
            _menuItems = new List<Response>
            {
                new(MenuConstants.MoneyOptionOne, $"{_config.MoneyAmount1}g"),
                new(MenuConstants.MoneyOptionTwo, $"{_config.MoneyAmount2}g"),
                new(MenuConstants.MoneyOptionThree, $"{_config.MoneyAmount3}g"),
                new(MenuConstants.MoneyOptionFour, $"{_config.MoneyAmount4}g"),
                new(MenuConstants.OptionCancel, I18n.Menu_Cancel())
            };

            _durationMenu = new List<Response>
            {
                new(MenuConstants.DurationOptionOne, $"{_config.DayLength1} {I18n.Menu_Days()} @ {_config.InterestModifier1 * 100}%"),
                new(MenuConstants.DurationOptionTwo, $"{_config.DayLength2} {I18n.Menu_Days()} @ {_config.InterestModifier2 * 100}%"),
                new(MenuConstants.DurationOptionThree, $"{_config.DayLength3} {I18n.Menu_Days()} @ {_config.InterestModifier3 * 100}%"),
                new(MenuConstants.DurationOptionFour, $"{_config.DayLength4} {I18n.Menu_Days()} @ {_config.InterestModifier4 * 100}%"),
                new(MenuConstants.OptionCancel, I18n.Menu_Cancel())
            };

            _repayMenuItems = new List<Response>
            {
                new(MenuConstants.ShowBalance, I18n.Menu_Showbalance()),
                new(MenuConstants.RepayCustom, I18n.Menu_Repaycustom()),
                new(MenuConstants.RepayFull, I18n.Menu_Repayfull()),
                new(MenuConstants.OptionCancel, I18n.Menu_Leave())
            };

            _menuYesNo = new List<Response>
            {
                new(MenuConstants.OptionYes, I18n.Menu_Yes()),
                new(MenuConstants.OptionNo, I18n.Menu_No()),
                new(MenuConstants.OptionCancel, I18n.Menu_Leave())
            };
        }

        private void ProcessBorrowing(int val, string key)
        {
            if (key != ModConstants.BorrowAmountKey) return;
            _amount = val;
            _loanManager.CurrentStage = Stages.Borrowing;
            Monitor.Log($"Selected {_amount}g", LogLevel.Info);
            Game1.activeClickableMenu = null;
            StartBorrow(2, ModConstants.BorrowDurationKey);
        }
        
        private void BorrowMenu(Farmer who, string menu)
        {
            switch (menu)
            {
                case MenuConstants.MoneyOptionOne:
                    SetAmount(_config.MoneyAmount1);
                    break;
                case MenuConstants.MoneyOptionTwo:
                    SetAmount(_config.MoneyAmount2);
                    break;
                case MenuConstants.MoneyOptionThree:
                    SetAmount(_config.MoneyAmount3);
                    break;
                case MenuConstants.MoneyOptionFour:
                    SetAmount(_config.MoneyAmount4);
                    break;
                case MenuConstants.OptionCancel:
                    _loanManager.CurrentStage = Stages.None;
                    break;
            }
        }

        private void SetAmount(int amount)
        {
            _amount = amount;
            _loanManager.CurrentStage = Stages.Borrowing;
            Monitor.Log($"Selected {amount}.", LogLevel.Info);
        }
        
        private void BorrowDuration(Farmer who, string dur)
        {
            switch (dur)
            {
                case MenuConstants.DurationOptionOne:
                    SetDurationAndInterest(_config.DayLength1, _config.InterestModifier1);
                    break;
                case MenuConstants.DurationOptionTwo:
                    SetDurationAndInterest(_config.DayLength2, _config.InterestModifier2);
                    break;
                case MenuConstants.DurationOptionThree:
                    SetDurationAndInterest(_config.DayLength3, _config.InterestModifier3);
                    break;
                case MenuConstants.DurationOptionFour:
                    SetDurationAndInterest(_config.DayLength4, _config.InterestModifier4);
                    break;
                case MenuConstants.OptionCancel:
                    _loanManager.CurrentStage = Stages.Borrowing;
                    break;
            }
        }

        private void SetDurationAndInterest(int duration, float interest)
        {
            _duration = duration;
            _interest = interest;
            Monitor.Log($"Selected {duration} days.");
        }

        private void RepayMenu(Farmer who, string option)
        {
            switch (option)
            {
                case MenuConstants.ShowBalance:
                    ExtensionHelper.AddMessage(I18n.Msg_Payment_Remaining(_loanManager.Balance.ToString("N0"), _loanManager.Duration, _loanManager.DailyAmount.ToString("N0")), HUDMessage.newQuest_type);
                    break;
                case MenuConstants.RepayCustom:
                    InitiateRepayment(false, true);
                    break;
                case MenuConstants.RepayFull:
                    _loanManager.CurrentStage = Stages.Repayment;
                    break;
            }
        }
        
        private void RepayFullMenu(Farmer who, string option)
        {
            if (option == MenuConstants.OptionYes)
                InitiateRepayment(true);
            else
                _loanManager.CurrentStage = Stages.None;
        }
    }
}
