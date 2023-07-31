/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using System;
using LoanMod.Common.Enums;
using LoanMod.Common.Interfaces;

namespace LoanMod.Common
{
    public class LoanManager : ILoanManager
    {
        public bool IsBorrowing { get; set; }
        public int AmountBorrowed { get; set; }
        public int Duration { get; set; }
        public float Interest { get; set; }
        public int AmountRepaid { get; set; }
        public int AmountRepaidToday { get; set; }
        public bool HasPaid { get; set; }
        public int DailyAmount { get; set; }
        public int Balance { get; set; }
        public float LateChargeRate { get; set; }
        public int LateChargeAmount { get; set; }
        public int LateDays { get; set; }
        public Stages CurrentStage { get; set; }

        public double CalculateBalance
        {
            get
            {
                double bal = AmountBorrowed - AmountRepaid;
                var balInterest = bal * Interest;

                return Math.Max(0, Math.Round(bal + balInterest, MidpointRounding.AwayFromZero));
            }
        }
        
        public int CalculateAmountToPayToday => Math.Max(DailyAmount - AmountRepaidToday, 0);
        
        public double CalculateDailyAmount
        {
            get
            {
                var daily = CalculateBalance / Duration;
                return Math.Round(daily, MidpointRounding.AwayFromZero);
            }
        }

        public double CalculateLateFees
        {
            get
            {
                double daily = (Balance * LateChargeRate);
                return Math.Round(daily, MidpointRounding.AwayFromZero);
            }
        }

        public void InitiateReset()
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
