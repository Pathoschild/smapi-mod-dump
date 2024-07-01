/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

namespace StardewValleyMods.JojaFinancial
{
    public interface ILoanSchedule
    {
        int GetMinimumPayment(int seasonsSinceLoanOrigination, int remainingBalance);
        public double GetInterestRate(int seasonsSinceLoanOrigination);
        public int GetLoanOriginationFeeAmount(int loanAmount);
    }
}

