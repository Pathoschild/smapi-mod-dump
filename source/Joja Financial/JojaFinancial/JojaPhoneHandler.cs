/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace StardewValleyMods.JojaFinancial
{
    public class JojaPhoneHandler
        : IPhoneHandler, ISimpleLog
    {
        protected ModEntry Mod { get; private set; } = null!;

        private const string JojaFinancialOutgoingPhone = "JojaFinancial.CallCenter";

        // TODO: Add much more spam to the opening.

        public void Entry(ModEntry mod)
        {
            this.Mod = mod;

            Phone.PhoneHandlers.Add(this);
        }

        public string? CheckForIncomingCall(Random random)
        {
            return null;
        }

        public IEnumerable<KeyValuePair<string, string>> GetOutgoingNumbers()
        {
            yield return new KeyValuePair<string, string>(JojaFinancialOutgoingPhone, L("Joja Finance"));
        }

        public bool TryHandleIncomingCall(string callId, out Action? showDialogue)
        {
            showDialogue = null;
            return false;
        }

        [ExcludeFromCodeCoverage]
        public bool TryHandleOutgoingCall(string callId)
        {
            if (callId != JojaFinancialOutgoingPhone) return false;

            Game1.currentLocation.playShopPhoneNumberSounds("JojaFinancial"); // <- string is only used to generate a consistent, random DTMF tone sequence.
            Game1.player.freezePause = 4950;
            DelayedAction.functionAfterDelay(delegate
            {
                Game1.playSound("bigSelect");
                if (this.ShouldGivePlayerTheRunAround)
                {
                    this.PhoneDialog(L("Welcome to JojaFinancial's Super-Helpful(tm) automated phone system!#$b#This call may be monitored for training and quality purposes."), () => this.RigmaroleMenu());
                }
                else
                {
                    this.PhoneDialog(L("Welcome to JojaFinancial's Super-Helpful(tm) automated phone system!"), () => this.MainMenu(L("How can we help you today?")));
                }
            }, 4950);

            return true;
        }

        // Consider making this configurable
        public bool ShouldGivePlayerTheRunAround => this.Mod.Loan.IsLoanUnderway;

        public record PhoneMenuItem(string Response, Action Action);

        private static string[] Names = [
            L("Elvis Aaron Presley"),
            L("King Leopold II"),
            L("George Foreman"),
            L("Judith Sheindlin"),
            L("Nipsy Russell"),
            L("Janet Reno"),
            L("The Right Honourable Viscount Nelson"),
            L("Wilma Flintstone"),
            L("James T. Kirk"),
            L("Tina Turner"),
            L("Beverly Crusher"),
            L("Patsy Stone"),
            L("Bruce Lee"),
            L("Detective Lennie Briscoe")
        ];

        public void RigmaroleMenu()
        {
            var actions = Names
                .OrderBy(x => Game1.random.Next())
                .Take(3)
                .Append(LF($"Farmer {this.Mod.Game1.PlayerName}"))
                .Select(x => new PhoneMenuItem(x, () => this.HaveIGotADealForYou(x)))
                .ToArray();
            this.PhoneDialog(L("In order to server you better, please give us your name:"), actions);
        }

        private static readonly string[] RandomSaleItems = [
            "208", // glazed yams
            "16",  // Wild horseradish
            "24",  // parsnip
            "78",  // Cave Carrot
            "88",  // Coconut
            "92",  // Sap
            "136", // Largemouth Bass
            "142", // Carp
            "153", // Green Algae
            "167", // Joja Cola
            "231", // Eggplant Parmesan
            "271", // Unmilled rice
            "306", // Mayonnaise
            "456", // Algae Soup
            "731", // Maple Bar
            "874", // Bug Steak
        ];

        public void HaveIGotADealForYou(string chosenName)
        {
            StardewValley.Object? thingToSell = null;
            do
            {
                string item = Game1.random.Choose(RandomSaleItems);
                thingToSell = this.Mod.Game1.CreateObject(item, Game1.random.Next(18)+2);
                if (thingToSell is null)
                {
                    this.LogError($"Bad random item {item} - not able to create it.");
                }
                else if (thingToSell.Price <= 0)
                {
                    this.LogError($"Bad random item {item}'s price is {thingToSell.Price}.");
                }
            } while (thingToSell is null || thingToSell.Price <= 0);
            int salesPrice = (int)(thingToSell.Stack * thingToSell.Price * 2.11); // randomize the number a bit
            string message =
                LF($@"Hey, just a quick second here to let you know that our buyers scour the earth for great deals that we can share with you. {chosenName}, our AI-backed sales team has picked a special deal, just for you. We are prepared to offer you {thingToSell.Stack} {thingToSell.Name} at the extra special, discounted price of {salesPrice}g.");
            this.PhoneDialog(message, [
                new PhoneMenuItem(L("I'll take it!"), () => this.HandleOneBornEveryMinute(chosenName, thingToSell, salesPrice)),
                new PhoneMenuItem(L("No thanks."), () => this.HandleHardSell(chosenName, thingToSell, salesPrice)),
            ]);
        }

        public void HandleOneBornEveryMinute(string chosenName, StardewValley.Object item, int salesPrice)
        {
            if (this.Mod.Game1.PlayerMoney >= salesPrice)
            {
                this.Mod.Game1.PlayerMoney -= salesPrice;
                this.Mod.GeneratedMail.SendMail("jojaSale", LF($"Your {item.Name} from JojaFinancial"),
                    L("Here's your special purchase from JojaFinancial's Super-Helpful Automated Phone System!  We're so glad our AI predicted your needs so well!"),
                    (item.QualifiedItemId, item.Stack));
                this.MainMenu(LF($"Processing...#$b#{chosenName}, your {item.Name} is on its way!  JojaCorp appreciates your business!#$b#Is there anything else I can help you with?"));
            }
            else
            {
                this.MainMenu(LF($"Processing...#$b#{chosenName}, I'm sorry to tell you that your bank declined the transaction citing insufficient funds!  Please come back when your credit situation improves!#$b#Is there anything else I can help you with?"));
            }
        }

        public void HandleHardSell(string chosenName, StardewValley.Object item, int salesPrice)
        {
            // Maybe randomize the messages?
            //   "Are you sure?  Next-day shipping is included at no extra charge!"
            this.PhoneDialog(
                L("Are you sure?  You know that buying stuff you don't need at inflated prices is a great way to boost your credit score!"),
                new PhoneMenuItem(L("Well, okay, if it'll boost my credit score..."), () => this.HandleOneBornEveryMinute(chosenName, item, salesPrice)),
                new PhoneMenuItem(L("Yes, quit asking!"), () => this.MainMenu(L("Okay, but if you change your mind, call us back!  Now, how else can we help you today?"))));
        }

        public void MainMenu(string message)
        {
            if (this.Mod.Loan.IsLoanUnderway)
            {
                this.PhoneDialog(message, [
                    new PhoneMenuItem(L("Get your balance and minimum payment amount"), this.HandleGetBalance),
                    new PhoneMenuItem(L("Make a payment"), this.HandleMakePayment),
                    this.Mod.Loan.IsOnAutoPay
                        ? new PhoneMenuItem(L("Turn off autopay"), this.HandleAutoPay)
                        : new PhoneMenuItem(L("Set up autopay"), this.HandleAutoPay),
                ]);
            }
            else if (this.Mod.Loan.IsPaidOff)
            {
                this.PhoneDialog(L("I'm sorry, but we have no more loan opportunities to offer you at this time.  But keep calling back for our special offers!"), () => { });
            }
            else
            {
                this.PhoneDialog(message, [
                    new PhoneMenuItem(L("Get the 2-year loan terms"), () => this.HandleGetLoanTerms(new LoanScheduleTwoYear())),
                    new PhoneMenuItem(L("Get the 3-year loan terms"), () => this.HandleGetLoanTerms(new LoanScheduleThreeYear())),
                    new PhoneMenuItem(L("Start a 2-year loan"), () => this.HandleStartTheLoan(new LoanScheduleTwoYear())),
                    new PhoneMenuItem(L("Start a 3-year loan"), () => this.HandleStartTheLoan(new LoanScheduleThreeYear())),
                ]);
            }
        }

        protected virtual void PhoneDialog(string message, Action doAfter)
        {
            var dialog = new Dialogue(null, null, message);
            dialog.overridePortrait = this.Mod.Helper.GameContent.Load<Texture2D>("Portraits\\AnsweringMachine");
            Game1.DrawDialogue(dialog);
            Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate { doAfter(); });
        }

        protected virtual void PhoneDialog(string message, params PhoneMenuItem[] menuItems)
        {
            // The PhoneDialog without the menu uses the 'Dialogue' class, which supports multipart messages, while
            //  this one does not.  So we'll do some shenanigans to give it consistent behavior.  Maybe.
            int multipartIndex = message.LastIndexOf(I("#$b#"));
            if (multipartIndex > 0)
            {
                string introPart = message.Substring(0, multipartIndex);
                string menuPart = message.Substring(multipartIndex + I("#$b#").Length);
                this.PhoneDialog(introPart, () => this.PhoneDialog(menuPart, menuItems));
            }
            else
            {
                // The "JojaFinanceResponse" string doesn't seem to be important at all.
                var responsesPlusHangUp = menuItems
                    .Select(i => new Response("JojaFinanceResponse", i.Response))
                    .Append(new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp")))
                    .ToArray();
                var actionsPlusHangUp = menuItems.Select(i => i.Action).Append(() => {}).ToArray();
                Game1.activeClickableMenu = new DialogueAndAction(message, responsesPlusHangUp, actionsPlusHangUp, this.Mod.Helper.Input);
            }
        }

        private void HandleGetLoanTerms(ILoanSchedule schedule)
        {
            this.Mod.Loan.SendMailLoanTerms(schedule);
            this.PhoneDialog(L("Great!  I just mailed to you the loan terms, you should have them tomorrow morning!  Call us back before the end of the month to lock in these low rates!"),
                () => this.MainMenu(L("Is there anything else we can do for you?")));
        }

        private void HandleStartTheLoan(ILoanSchedule schedule)
        {
            this.Mod.Loan.InitiateLoan(schedule);
            this.PhoneDialog(LF($"Great!  I just mailed to you the catalogs and started your loan!  Remember to make your payments by the 21st of every month or you can set up auto-pay !"),
                () => this.MainMenu(L("Is there anything else we can do for you?")));
        }

        public void HandleGetBalance()
        {
            string message;
            if (this.Mod.Loan.MinimumPayment == 0)
            {
                message = LF($"Your current balance is {this.Mod.Loan.RemainingBalance}g.  No payment is due this season.");
            }
            else if (this.Mod.Loan.PaidThisSeason < this.Mod.Loan.MinimumPayment)
            {
                if (this.Mod.Game1.Date.DayOfMonth <= 21)
                {
                    message = LF($"Your current balance is {this.Mod.Loan.RemainingBalance}g.  Your minimum payment is {this.Mod.Loan.MinimumPayment}g and is due on the 21st.");
                }
                else
                {
                    message = LF($"Your current balance is {this.Mod.Loan.RemainingBalance}g.  Your minimum payment is {this.Mod.Loan.MinimumPayment}g and was due on the 21st.");
                }
            }
            else
            {
                message = LF($"Your current balance is {this.Mod.Loan.RemainingBalance}g.  Your minimum payment has been made this season, Thank you!");
            }

            this.PhoneDialog(message, () => this.MainMenu(L("Is there anything else we can do for you?")));
        }

        public void HandleMakePayment()
        {
            List<PhoneMenuItem> menuItems = new();
            if (this.Mod.Loan.MinimumPayment == 0 || this.Mod.Loan.MinimumPayment >= this.Mod.Loan.PaidThisSeason)
            {
                menuItems.Add(new PhoneMenuItem(LF($"The minimum ({this.Mod.Loan.MinimumPayment}g)"), () => this.HandleMakePayment(this.Mod.Loan.MinimumPayment)));
            }
            menuItems.Add(new PhoneMenuItem(LF($"The full remaining balance ({this.Mod.Loan.RemainingBalance}g)"), () => this.HandleMakePayment(this.Mod.Loan.RemainingBalance)));

            this.PhoneDialog(L("How much would you like to pay?"), menuItems.ToArray());
        }

        public void HandleMakePayment(int amount)
        {
            if (amount == 0)
            {
                this.MainMenu(L("There's no need to pay at this point - you owe nothing right now."));
            }
            else
            {
                this.PhoneDialog(L("Processing..."), () =>
                {
                    string message = this.Mod.Loan.TryMakePayment(amount)
                        ? (this.Mod.Loan.IsPaidOff
                            ? L("Thank you!  Your loan is fully repaid!  You and JojaCorp thrive together!  Is there anything else we can do for you today?")
                            : L("Thank you for making your payment!  Is there anything else we can do for you today?"))
                        : L("I'm sorry, but your bank declined the request citing insufficient funds.  Is there anything else we can do for you today?");
                    this.MainMenu(message);
                });
            }
        }

        public void HandleAutoPay()
        {
            this.Mod.Loan.IsOnAutoPay = !this.Mod.Loan.IsOnAutoPay;
            string message = this.Mod.Loan.IsOnAutoPay
                ? LF($"Thank you for taking advantage of AutoPay - remember to have sufficient funds in your account by day {Loan.AutoPayDayOfSeason} of each season to cover the minimum payment.")
                : LF($"Auto-Pay has been turned off for your account.  Remember to call the Super-Helpful(tm) automated phone system by day {Loan.PaymentDueDayOfSeason} of each season to make your seasonal payment.");
            message += L(" Is there anything else I can help you with?");
            this.MainMenu(message);
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            ((ISimpleLog)this.Mod).WriteToLog(message, level, isOnceOnly);
        }
    }
}
