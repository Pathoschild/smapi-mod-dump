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

namespace LoanMod
{
    public partial class ModEntry
    {
        private List<Response> menuItems, repayMenuItems, durationMenu, menuYesNo;
        private void InitMenus()
        {
            menuItems = new List<Response>
            {
                new Response("money_500", $"{Config.MoneyAmount1}g"),
                new Response("money_1k", $"{Config.MoneyAmount2}g"),
                new Response("money_5k", $"{Config.MoneyAmount3}g"),
                new Response("money_10k", $"{Config.MoneyAmount4}g"),
                new Response("money_Cancel", I18n.Menu_Cancel())
            };

            durationMenu = new List<Response>
            {
                new Response("time_3D", $"{Config.DayLength1} {I18n.Menu_Days()} @ {Config.InterestModifier1 * 100}%"),
                new Response("time_7D", $"{Config.DayLength2} {I18n.Menu_Days()} @ {Config.InterestModifier2 * 100}%"),
                new Response("time_14D", $"{Config.DayLength3} {I18n.Menu_Days()} @ {Config.InterestModifier3 * 100}%"),
                new Response("time_28D", $"{Config.DayLength4} {I18n.Menu_Days()} @ {Config.InterestModifier4 * 100}%"),
                new Response("time_Cancel", I18n.Menu_Cancel())
            };

            repayMenuItems = new List<Response>
            {
                new Response("repay_show_Balance", I18n.Menu_Showbalance()),
                new Response("repay_Custom", I18n.Menu_Repaycustom()),
                new Response("repay_Full", I18n.Menu_Repayfull()),
                new Response("repay_Leave", I18n.Menu_Leave())
            };

            menuYesNo = new List<Response>
            {
                new Response("menu_Yes", I18n.Menu_Yes()),
                new Response("menu_No", I18n.Menu_No()),
                new Response("menu_Leave", I18n.Menu_Leave())
            };
        }

        //private void StartMobileBorrow()
        //{
        //    mobileApi.SetAppRunning(true);
        //    mobileApi.SetRunningApp(Helper.ModRegistry.ModID);
        //    StartBorrow(1, "Key_Amount");
        //}

        private void StartBorrow(int stage, string key)
        {
            var Gamer = Game1.currentLocation;
            //check if player isnt already borrowing
            if (!loanManager.IsBorrowing)
            {
                switch (stage)
                {
                    case 1:
                        if (Config.CustomMoneyInput)
                            Game1.activeClickableMenu = new NumberSelectionMenu(I18n.Msg_Startborrow1(), (val, cost, farmer) => ProcessBorrowing(val, key), -1, 100, Config.MaxBorrowAmount, 500);
                        else
                            Gamer.createQuestionDialogue(I18n.Msg_Startborrow1(), menuItems.ToArray(), BorrowMenu);
                        break;
                    case 2:
                        //Game1.activeClickableMenu = new NumberSelectionMenu(i18n.Get("msg.startborrow-2"), (val, cost, farmer) => ProcessBorrowing(val, cost, farmer, key), -1, 1);
                        Gamer.createQuestionDialogue(I18n.Msg_Startborrow2(), durationMenu.ToArray(), BorrowDuration);
                        break;
                }
            }
            else
            {
                switch (stage)
                {
                    case 1:
                        Gamer.createQuestionDialogue(I18n.Msg_Menu1(), repayMenuItems.ToArray(), RepayMenu);
                        break;
                    case 3:
                        Gamer.createQuestionDialogue(I18n.Msg_Menu2(loanManager.Balance.ToString("N0")), menuYesNo.ToArray(), RepayFullMenu);
                        break;
                }
            }

            //if (mobileApi?.GetRunningApp() == Helper.ModRegistry.ModID)
            //    mobileApi.SetAppRunning(false);
        }

        private void ProcessBorrowing(int val, string key)
        {
            switch (key)
            {
                case "Key_Amount":
                    amount = val;
                    borrowProcess = true;
                    this.Monitor.Log($"Selected {amount}g", LogLevel.Info);
                    Game1.activeClickableMenu = null;
                    StartBorrow(2, "Key_Duration");
                    break;
            }
        }

        private void BorrowMenu(Farmer who, string menu)
        {
            switch (menu)
            {
                case "money_500":
                    amount = Config.MoneyAmount1;
                    borrowProcess = true;
                    this.Monitor.Log("Selected 500g.", LogLevel.Info);
                    break;
                case "money_1k":
                    amount = Config.MoneyAmount2;
                    borrowProcess = true;
                    this.Monitor.Log("Selected 1,000g.", LogLevel.Info);
                    break;
                case "money_5k":
                    amount = Config.MoneyAmount3;
                    borrowProcess = true;
                    this.Monitor.Log("Selected 5,000g.", LogLevel.Info);
                    break;
                case "money_10k":
                    amount = Config.MoneyAmount4;
                    borrowProcess = true;
                    this.Monitor.Log("Selected 10,000g.", LogLevel.Info);
                    break;
                case "money_Cancel":
                    borrowProcess = false;
                    this.Monitor.Log("Option Cancel");
                    break;
            }
        }

        private void BorrowDuration(Farmer who, string dur)
        {
            switch (dur)
            {
                case "time_3D":
                    duration = Config.DayLength1;
                    interest = Config.InterestModifier1;
                    this.Monitor.Log($"Selected {Config.DayLength1} days.");
                    break;
                case "time_7D":
                    duration = Config.DayLength2;
                    interest = Config.InterestModifier2;
                    this.Monitor.Log($"Selected {Config.DayLength2} days.");
                    break;
                case "time_14D":
                    duration = Config.DayLength3;
                    interest = Config.InterestModifier3;
                    this.Monitor.Log($"Selected {Config.DayLength3} days.");
                    break;
                case "time_28D":
                    duration = Config.DayLength4;
                    interest = Config.InterestModifier4;
                    this.Monitor.Log($"Selected {Config.DayLength4} days.");
                    break;
                case "time_Cancel":
                    borrowProcess = false;
                    Monitor.Log("Option Cancel");
                    break;
            }
        }
        private void RepayMenu(Farmer who, string option)
        {
            switch (option)
            {
                case "repay_show_Balance":
                    Monitor.Log("Option show balance", LogLevel.Debug);
                    AddMessage(I18n.Msg_Payment_Remaining(loanManager.Balance.ToString("N0"), loanManager.Duration, loanManager.CalculateAmountToPayToday.ToString("N0")), HUDMessage.newQuest_type);
                    break;
                case "repay_Custom":
                    Monitor.Log("Option repay custom", LogLevel.Debug);
                    InitiateRepayment(false, true);
                    break;
                case "repay_Full":
                    Monitor.Log("Option repay Full", LogLevel.Debug);
                    repayProcess = true;
                    break;
                case "repay_Leave":
                    Monitor.Log("Option Leave", LogLevel.Debug);
                    break;
            }
        }
        private void RepayFullMenu(Farmer who, string option)
        {
            switch (option)
            {
                case "menu_Yes":
                    Monitor.Log("Option Yes", LogLevel.Debug);
                    InitiateRepayment(true);
                    break;
                case "menu_No":
                    Monitor.Log("Option No", LogLevel.Debug);
                    repayProcess = false;
                    break;
                case "menu_Leave":
                    Monitor.Log("Option Leave", LogLevel.Debug);
                    repayProcess = false;
                    break;
            }
        }
    }
}
