/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using LoanMod.Common.Enums;

namespace LoanMod.Common.Interfaces
{
    public interface ILoanManager
    {
        /// <summary>
        /// <param>Indicates whether the player is currently borrowing money.</param>
        /// </summary>
        bool IsBorrowing { get; set; }

        /// <summary>
        /// <param>Shows the amount of money the player is borrowing.</param>
        /// </summary>
        int AmountBorrowed { get; set; }

        /// <summary>
        /// <param>Shows the duration (in days) of the loan.</param>
        /// </summary>
        int Duration { get; set; }

        /// <summary>
        /// <param>Shows the interest rate of the loan.</param>
        /// </summary>
        float Interest { get; set; }

        /// <summary>
        /// <param>Shows the amount of money the player has already repayed.</param>
        /// </summary>
        int AmountRepaid { get; set; }

        /// <summary>
        /// <param>Shows the amount of money the player has paid today.</param>
        /// </summary>
        int AmountRepaidToday { get; set; }

        /// <summary>
        /// <param>Indicates if the player has already made a payment on the current day.</param>
        /// </summary>
        bool HasPaid { get; set; }

        /// <summary>
        /// <param>Shows the daily repayment amount.</param>
        /// </summary>
        int DailyAmount { get; set; }

        /// <summary>
        /// <param>Shows the current balance remaining to be paid off.</param>
        /// </summary>
        int Balance { get; set; }

        /// <summary>
        /// <param>Shows the late payment charge rate.</param>
        /// </summary>
        float LateChargeRate { get; set; }

        /// <summary>
        /// <param>Shows the late payment charge amount.</param>
        /// </summary>
        int LateChargeAmount { get; set; }

        /// <summary>
        /// <param>Shows the days of late payments.</param>
        /// </summary>
        int LateDays { get; set; }

        /// <summary>
        /// <param>Calculates the current balance remaining to be paid off.</param>
        /// </summary>
        double CalculateBalance { get; }

        /// <summary>
        /// <param>Calculates the amount of money the player has to pay at the end of the day.</param>
        /// </summary>
        int CalculateAmountToPayToday { get; }

        /// <summary>
        /// <param>Calculates the daily amount based on the balance left to pay.</param>
        /// </summary>
        double CalculateDailyAmount { get; }

        /// <summary>
        /// <param>Calculates the late payment fee amount based on the balance left to pay.</param>
        /// </summary>
        double CalculateLateFees { get; }

        /// <summary>
        /// <param>Used to determine if the player is currently going through a borrowing/repayment process</param>
        /// </summary>
        Stages CurrentStage { get; set; }

        /// <summary>
        /// <param>Resets the mod.</param>
        /// </summary>
        void InitiateReset();
    }
}