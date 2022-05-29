/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using StardewValley;
using System;

namespace LoanMod
{
    public partial class ModEntry
    {
        internal class LoanManager
        {
            /// <summary>
            /// <param name="IsBorrowing">Indicates whether the player is currently borrowing money.</param>
            /// </summary>
            public bool IsBorrowing { get; set; } = false;
            /// <summary>
            /// <param name="AmountBorrowed">Shows the amount of money the player is borrowing.</param>
            /// </summary>
            public int AmountBorrowed { get; set; } = 0;
            /// <summary>
            /// <param name="Duration">Shows the duration (in days) of the loan.</param>
            /// </summary>
            public int Duration { get; set; } = 0;
            /// <summary>
            /// <param name="Interest">Shows the interest rate of the loan.</param>
            /// </summary>
            public float Interest { get; set; } = 0;
            /// <summary>
            /// <param name="AmountRepaid">Shows the amount of money the player has already repayed.</param>
            /// </summary>
            public int AmountRepaid { get; set; } = 0;
            /// <summary>
            /// <param name="AmountRepaidToday">Shows the amount of money the player has paid today.</param>
            /// </summary>
            public int AmountRepaidToday { get; set; } = 0;
            /// <summary>
            /// <param name="hasPaid">Indicates if the player has already made a payment on the current day.</param>
            /// </summary>
            public bool HasPaid { get; set; } = false;
            /// <summary>
            /// <param name="DailyAmount">Shows the daily repayment amount.</param>
            /// </summary>
            public int DailyAmount { get; set; } = 0;
            /// <summary>
            /// <param name="Balance">Shows the current balance remaining to be paid off.</param>
            /// </summary>
            public int Balance { get; set; } = 0;
            /// <summary>
            /// <param name="LateChargeRate">Shows the late payment charge rate.</param>
            /// </summary>
            public float LateChargeRate { get; set; } = 0;
            /// <summary>
            /// <param name="LateChargeAmount">Shows the late payment charge amount.</param>
            /// </summary>
            public int LateChargeAmount { get; set; } = 0;
            /// <summary>
            /// <param name="LateDays">Shows the days of late payments.</param>
            /// </summary>
            public int LateDays { get; set; } = 0;

            /// <summary>
            /// <param name="CalculateBalance">Calculates the current balance remaining to be paid off.</param>
            /// </summary>
            internal double CalculateBalance
            {
                get
                {
                    double bal = (AmountBorrowed - AmountRepaid);
                    double balinterest = bal * Interest;

                    return Math.Round(bal + balinterest, MidpointRounding.AwayFromZero);
                }
            }

            /// <summary>
            /// <param name="CalculateAmountToPayToday">Calculates the amount of money the player has to pay at the end of the day.</param>
            /// </summary>
            internal int CalculateAmountToPayToday => Math.Max(DailyAmount - AmountRepaidToday, 0);

            /// <summary>
            /// <param name="CalculateInitDailyAmount">Calculates the daily amount based on the balance left to pay.</param>
            /// </summary>
            internal double CalculateInitDailyAmount
            {
                get
                {
                    double daily = CalculateBalance / Duration;
                    return Math.Round(daily, MidpointRounding.AwayFromZero);
                }
            }

            /// <summary>
            /// <param name="CalculateLateFees">Calculates the late payment fee amount based on the balance left to pay.</param>
            /// </summary>
            internal double CalculateLateFees
            {
                get
                {
                    double daily = (Balance * LateChargeRate);
                    return Math.Round(daily, MidpointRounding.AwayFromZero);
                }
            }

            /// <summary>
            /// Estimates interest based on the amount and days.
            /// </summary>
            /// <param name="borrowAmount">Amount the player is borrowing.</param>
            /// <param name="days">The duration of the loan.</param>
            /// <param name="cfg">The config file used.</param>
            /// <returns>Interest rate as float.</returns>
            internal static double EstimateInterest(int borrowAmount, int days, ModConfig cfg)
            {
                float interestFromAmount = 0;
                float interestFromDays = 0;

                if (borrowAmount <= cfg.MoneyAmount1)
                {
                    interestFromAmount = cfg.InterestModifier1 / 2;
                }
                else if (borrowAmount <= cfg.MoneyAmount2)
                {
                    interestFromAmount = cfg.InterestModifier2 / 2;
                }
                else if (borrowAmount <= cfg.MoneyAmount3)
                {
                    interestFromAmount = cfg.InterestModifier3 / 2;
                }
                else if (borrowAmount >= cfg.MoneyAmount3)
                {
                    interestFromAmount = cfg.InterestModifier4 / 2;
                }

                if (days <= cfg.DayLength1)
                {
                    interestFromDays = cfg.InterestModifier1 / 2;
                }
                else if (days <= cfg.DayLength2)
                {
                    interestFromDays = cfg.InterestModifier2 / 2;
                }
                else if (days <= cfg.DayLength3)
                {
                    interestFromDays = cfg.InterestModifier3 / 2;
                }
                else if (days >= cfg.DayLength3)
                {
                    interestFromDays = cfg.InterestModifier4 / 2;
                }

                return interestFromAmount + interestFromDays;
            }

            /// <summary>
            /// <param name="InitiateReset">Resets the mod.</param>
            /// </summary>
            internal void InitiateReset()
            {
                IsBorrowing = false;
                AmountBorrowed = 0;
                Duration = 0;
                Interest = 0;
                AmountRepaid = 0;
                HasPaid = false;
                Balance = 0;
                DailyAmount = 0;
            }
        }
    }
}
