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

namespace LoanMod.Common
{
    public class ModConfig
    {
        private const int MAX_LOAN_INT = 99999999;
        private int _maxBorrowAmount = 1000000;
        public SButton LoanButton { get; set; } = SButton.L;
        public bool CustomMoneyInput { get; set; } = true;
        public int MaxBorrowAmount
        {
            get => _maxBorrowAmount;
            set => _maxBorrowAmount = value > MAX_LOAN_INT ? MAX_LOAN_INT : value;
        }
        public float LatePaymentChargeRate { get; set; } = 0.1F;
        public float InterestModifier1 { get; set; } = 0.5F;
        public float InterestModifier2 { get; set; } = 0.25F;
        public float InterestModifier3 { get; set; } = 0.1F;
        public float InterestModifier4 { get; set; } = 0.05F;
        public int MoneyAmount1 { get; set; } = 500;
        public int MoneyAmount2 { get; set; } = 1000;
        public int MoneyAmount3 { get; set; } = 5000;
        public int MoneyAmount4 { get; set; } = 10000;
        public int DayLength1 { get; set; } = 3;
        public int DayLength2 { get; set; } = 7;
        public int DayLength3 { get; set; } = 14;
        public int DayLength4 { get; set; } = 28;
        public bool Reset { get; set; }
        public bool AddMobileApp { get; set; } = true;
    }
}