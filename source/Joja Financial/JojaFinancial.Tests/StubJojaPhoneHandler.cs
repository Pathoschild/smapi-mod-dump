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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValleyMods.JojaFinancial;

namespace JojaFinancial.Tests
{
    public class StubJojaPhoneHandler
        : JojaPhoneHandler
    {
        public Queue<string> PromptToTake { get; } = new Queue<string>();
        private List<string> Messages = new List<string>();

        public void GivenPlayerMakesMainMenuChoices(params string[] prompts)
        {
            this.Messages.Clear();
            Assert.IsTrue(!this.PromptToTake.Any(), "There are some untaken phone choices left.  Perhaps a bad test or something went wrong earlier?");
            foreach (string prompt in prompts)
            {
                this.PromptToTake.Enqueue(prompt);
            }
            this.PromptToTake.Enqueue("");
            this.MainMenu("test");

            if (this.PromptToTake.Count == 1 && this.PromptToTake.Peek() == "")
            {
                // On the last day of the season, the system hangs up on the player...  We'll allow that in general I guess.
                this.PromptToTake.Dequeue();
            }
            Assert.IsTrue(!this.PromptToTake.Any(), "There are some untaken phone choices left after phone call completed.");
        }

        public void GivenPlayerGetsRigmarole(params string[] prompts)
        {
            this.Messages.Clear();
            Assert.IsTrue(!this.PromptToTake.Any(), "There are some untaken phone choices left.  Perhaps a bad test or something went wrong earlier?");
            foreach (string prompt in prompts)
            {
                this.PromptToTake.Enqueue(prompt);
            }
            this.PromptToTake.Enqueue("");
            this.RigmaroleMenu();

            Assert.IsTrue(!this.PromptToTake.Any(), "There are some untaken phone choices left after phone call completed.");
        }

        protected override void PhoneDialog(string message, params PhoneMenuItem[] menuItems)
        {
            if (!this.PromptToTake.Any())
            {
                Assert.Fail("Test failed to set PromptToTake");
            }

            this.Messages.Add(message);
            string prompt = this.PromptToTake.Dequeue();
            if (prompt == "")
            {
                // Hang-up
                return;
            }

            var options = menuItems.Where(mi => mi.Response.Contains(prompt, StringComparison.OrdinalIgnoreCase)).ToList();
            Assert.IsTrue(options.Count > 0, $"None of the actual prompts matched '{prompt}'.  Options include: {string.Join(", ", menuItems.Select(i => i.Response))}");
            Assert.IsTrue(options.Count == 1, $"'{this.PromptToTake}' needs to be made more specific, options include: {string.Join(", ", menuItems.Select(i => i.Response))}");

            options[0].Action();
        }

        protected override void PhoneDialog(string message, Action doAfter)
        {
            this.Messages.Add(message);
            doAfter();
        }

        public void AssertPaymentAndBalance(int payment, int balance)
        {
            this.GivenPlayerMakesMainMenuChoices("balance and minimum payment");
            if (payment == 0)
            {
                var match = new Regex(@"balance is (?<balance>\d+)g.*No payment is due", RegexOptions.IgnoreCase).Match(this.Messages[1]);
                Assert.IsTrue(match.Success, $"The balance and minimum payment result is unreadable: {this.Messages[1]}");
                Assert.AreEqual(balance, int.Parse(match.Groups["balance"].Value));
            }
            else
            {
                var match = new Regex(@"balance is (?<balance>\d+)g.*minimum payment is (?<payment>\d+)g.*(?<isOrWas>is|was) due on the", RegexOptions.IgnoreCase).Match(this.Messages[1]);
                Assert.IsTrue(match.Success, $"The balance and minimum payment result is unreadable: {this.Messages[1]}");
                Assert.AreEqual(payment, int.Parse(match.Groups["payment"].Value));
                Assert.AreEqual(balance, int.Parse(match.Groups["balance"].Value));
            }
        }

        /// <summary>
        ///   After the loan runs its course, the phone system just shuts down.
        /// </summary>
        public void AssertNothingAvailable()
        {
            this.GivenPlayerMakesMainMenuChoices();
            Assert.IsTrue(this.Messages[0].Contains("no more loan opportunities"));
        }

        public void GetPaymentAndBalance(out int payment, out int balance, out bool isOverdue)
        {
            this.GivenPlayerMakesMainMenuChoices("balance and minimum payment");

            var match = new Regex(@"balance is (?<balance>\d+)g.*minimum payment is (?<payment>\d+)g.*(?<isOrWas>is|was) due on the", RegexOptions.IgnoreCase).Match(this.Messages[1]);
            if (match.Success)
            {
                balance = int.Parse(match.Groups["balance"].Value);
                payment = int.Parse(match.Groups["payment"].Value);
                isOverdue = match.Groups["isOrWas"].Value == "was";
                return;
            }

            // ... It'd be nice if we could enforce which of these two we got.
            match = new Regex(@"balance is (?<balance>\d+)g.*No payment is due", RegexOptions.IgnoreCase).Match(this.Messages[1]);
            if (match.Success)
            {
                balance = int.Parse(match.Groups["balance"].Value);
                payment = 0;
                isOverdue = false;
                return;
            }

            match = new Regex(@"balance is (?<balance>\d+)g.*minimum payment has been made this season", RegexOptions.IgnoreCase).Match(this.Messages[1]);
            if (match.Success)
            {
                balance = int.Parse(match.Groups["balance"].Value);
                payment = 0;
                isOverdue = false;
                return;
            }

            Assert.Fail($"The balance and minimum payment result is unreadable: {this.Messages[1]}");
            throw new NotImplementedException(); // not reachable.
        }

        public void EnsurePlayerHasNothingMoreToPayThisSeason()
        {
            this.GivenPlayerMakesMainMenuChoices("balance and minimum payment");
            var match = new Regex(@"balance is (?<balance>\d+)g.*minimum payment has been made this season", RegexOptions.IgnoreCase).Match(this.Messages[1]);
            Assert.IsTrue(match.Success, $"The balance and minimum payment result is unreadable: {this.Messages[1]}");
        }

        public void GivenPlayerSetsUpAutopay()
        {
            this.GivenPlayerMakesMainMenuChoices("Set up autopay");
            string response = this.Messages[1];
            Assert.IsTrue(response.StartsWith("Thank you for taking advantage of AutoPay"), $"Unexpected response: {response}");
            Assert.IsTrue(response.Contains(Loan.AutoPayDayOfSeason.ToString()), $"AutoPay response should have mentioned the automatic payment date: {response}");
        }

        public void GivenPlayerTurnsOffAutopay()
        {
            this.GivenPlayerMakesMainMenuChoices("Turn off autopay");
            string response = this.Messages[1];
            Assert.IsTrue(response.StartsWith("Auto-Pay has been turned off"), $"Unexpected response: {response}");
            Assert.IsTrue(response.Contains(Loan.PaymentDueDayOfSeason.ToString()), $"AutoPay response should have mentioned the payment date: {response}");
        }

        internal void EnsureRandoSaleObjectDeliveredAndPaidFor(int priorPlayerMoney)
        {
            var stubMailer = (StubGeneratedMail)this.Mod.GeneratedMail;
            var mailItem = stubMailer.EnsureSingleMatchingItemWasDelivered(m => m.IdPrefix == "jojaSale", "Joja Rando-Sale");
            Assert.AreEqual($"Your {StubGame1.RandoSaleObjectName} from JojaFinancial", mailItem.Synopsis);
            Assert.AreEqual(1, mailItem.attachedItems.Length);
            Assert.AreEqual("(O)" + StubGame1.RandoSaleObjectId, mailItem.attachedItems[0].qiid);
            var m = new Regex(@" (?<price>\d+)g").Match(this.Messages[1]);
            Assert.IsTrue(m.Success);
            int listPrice = int.Parse(m.Groups["price"].Value);
            Assert.IsTrue(listPrice >= 2 * mailItem.attachedItems[0].count * StubGame1.RandoSalePrice);
            Assert.IsTrue(listPrice <= 3 * mailItem.attachedItems[0].count * StubGame1.RandoSalePrice);
            Assert.AreEqual(priorPlayerMoney - listPrice, this.Mod.Game1.PlayerMoney);
        }
    }
}
