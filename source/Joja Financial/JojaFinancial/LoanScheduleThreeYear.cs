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
using System.Threading.Tasks;
using StardewValley;

namespace StardewValleyMods.JojaFinancial
{
    public class LoanScheduleThreeYear : ILoanSchedule
    {
        private readonly static double[] percentageByMonth = [0, 0, 0.03, 0.04, 0.05, 0.06, 0.1, 0.15, 0.25, 0.3, 0.5, 1 ];

        public double GetInterestRate(int seasonsSinceLoanOrigination)
        {
            return seasonsSinceLoanOrigination > 0 ? .02 : 0;
        }

        public int GetLoanOriginationFeeAmount(int loanAmount) => 5000;

        public int GetMinimumPayment(int seasonsSinceLoanOrigination, int remainingBalance)
        {
            return (int)(remainingBalance * (seasonsSinceLoanOrigination < percentageByMonth.Length ? percentageByMonth[seasonsSinceLoanOrigination] : 1));
        }
    }
}
