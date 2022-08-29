/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace BankOfFerngill.Framework.Data
{
    public class BankData
    {
        public bool BankActive { get; set; } = false;
        public int MoneyInBank { get; set; } = 0;

        public int LoanedMoney { get; set; } = 0;

        public int MoneyPaidBack { get; set; } = 0;

        public int BankInterest { get; set; } = 1;

        public int LoanInterest { get; set; } = 3;

        public int NumberOfLoansPaidBack { get; set; } = 0;

        public int TotalNumberOfLoans { get; set; } = 0;

        public bool WasHardModeDebtAdded { get; set; } = false;
    }
}