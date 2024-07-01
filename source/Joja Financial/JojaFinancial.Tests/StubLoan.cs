/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System.Text.RegularExpressions;
using JojaFinancial.Tests;
using StardewValley;

namespace StardewValleyMods.JojaFinancial.Tests
{
    internal class StubLoan
        : Loan
    {
        public StubModHelper StubHelper => (StubModHelper)this.Mod.Helper;
        public StubGeneratedMail StubMailer => (StubGeneratedMail)this.Mod.GeneratedMail;

        private static readonly Regex PaymentEntryRegex = new Regex(@"(?<season>(spring|summer|fall|winter)) of year (?<year>\d+)\r?\n\s+Payment: \-(?<payment>\d+)g", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public List<(Season season, int year, int payment)> EnsureTermsHaveBeenDelivered()
        {
            var mailItem = this.StubMailer.SentMail.Find(m => m.IdPrefix.StartsWith("terms"));
            Assert.IsNotNull(mailItem, "Terms mail was not delivered");

            List<(Season season, int year, int payment)> result = new();
            foreach (Match match in PaymentEntryRegex.Matches(mailItem.Message))
            {
                Season season = Enum.Parse<Season>(match.Groups["season"].Value);
                int year = int.Parse(match.Groups["year"].Value);
                int payment = int.Parse(match.Groups["payment"].Value);
                result.Add(new(season, year, payment));
            }

            int expectedPayments = this.Mod.Game1.Date.DayOfMonth >= Loan.PaymentDueDayOfSeason ? 7 : 8;
            if (mailItem.IdPrefix == "terms.LoanScheduleThreeYear")
            {
                expectedPayments += 4;
            }
            Assert.AreEqual(expectedPayments, result.Count, $"Expected {expectedPayments} payments, but found {result.Count}");

            this.StubMailer.SentMail.Remove(mailItem);
            mailItem = this.StubMailer.SentMail.Find(m => m.IdPrefix == "terms");
            Assert.IsNull(mailItem, "More than one loan terms mail was sent");
            return result;
        }

        public void EnsureCatalogsHaveBeenDelivered()
        {
            var mailItem = this.StubMailer.EnsureSingleMatchingItemWasDelivered(m => m.IdPrefix == "welcome", "Loan Welcome");
        }

        public void AssertGotAutoPaySuccessMail()
        {
            var mailItem = this.StubMailer.EnsureSingleMatchingItemWasDelivered(m => m.Synopsis == "Autopay Succeeded", "AutoPay success");
            if (this.RemainingBalance > 0)
            {
                this.StubMailer.AssertNoMoreMail();
            }
            // Leave it to the test to validate the end-of-loan mail.
        }

        public void AssertGotAutoPayFailedMail()
        {
            var mailItem = this.StubMailer.EnsureSingleMatchingItemWasDelivered(m => m.Synopsis == "Auto-pay Failed!" && m.Message.Contains(this.MinimumPayment.ToString()), "AutoPay success");
            this.StubMailer.AssertNoMoreMail();
        }

        private static readonly Regex StatementPaymentRegex = new Regex($@"minimum payment.*{Loan.PaymentDueDayOfSeason}.*season is: (?<payment>\d+)g", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex NoPaymentDueRegex = new Regex($@"No payment is necessary", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public int EnsureSeasonalStatementDelivered()
        {
            var mailItem = this.StubMailer.EnsureSingleMatchingItemWasDelivered(m => m.IdPrefix == "statement", "Seasonal Statement");
            this.StubMailer.AssertNoMoreMail();

            bool hasAutopayThing = mailItem.Message.Contains("automatically", StringComparison.OrdinalIgnoreCase);

            var match = StatementPaymentRegex.Match(mailItem.Message);
            if (match.Success)
            {
                Assert.AreEqual(hasAutopayThing, this.IsOnAutoPay, $"If autopay is on, it should mention that, and otherwise not.  Message:\r\n{mailItem.Message}");
                return int.Parse(match.Groups["payment"].Value);
            }
            else if (NoPaymentDueRegex.IsMatch(mailItem.Message))
            {
                return 0;
            }
            else
            {
                Assert.Fail($"Could not find the payment amount in the statement:\r\n{mailItem.Message}");
                return 0;
            }
        }

        public void AssertGotPaidInFullMail()
        {
            _ = this.StubMailer.EnsureSingleMatchingItemWasDelivered(m => m.Message.Contains("paid in full"), "Closing Message");
        }

        public void AssertGotMissedPaymentMail()
        {
            _ = this.StubMailer.EnsureSingleMatchingItemWasDelivered(
                m => m.Synopsis.Contains("Missed payment", StringComparison.OrdinalIgnoreCase)
                     && m.Message.Contains(Loan.LateFee.ToString())
                , "Missed Payment Message");

        }
    }
}
