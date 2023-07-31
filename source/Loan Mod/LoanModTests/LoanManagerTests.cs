/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using LoanMod.Common;
using LoanMod.Common.Interfaces;
using Xunit;

namespace LoanModTests
{
    public class LoanManagerTests
    {
        private readonly ILoanManager _loanManager = new LoanManager
        {
            AmountBorrowed = 500,
            AmountRepaid = 200,
            AmountRepaidToday = 200,
            Interest = 0.1f,
            Duration = 2,
            LateChargeRate = 0.1f,
        };

        [Fact]
        public void CalculateBalance_ReturnsZero_IfEmpty()
        {
            _loanManager.InitiateReset();
            Assert.Equal(0, _loanManager.CalculateBalance);
        }

        [Fact]
        public void CalculateBalance_ReturnsCorrectBalance()
        {
            Assert.Equal(330, _loanManager.CalculateBalance);
        }

        [Theory]
        [InlineData(500, 200, 0.1f, 330)]
        [InlineData(500, 500, 0.1f, 0)]
        [InlineData(500, 0, 0.1f, 550)]
        public void CalculateBalance_ReturnsCorrectBalance_WithInterest(int borrowed, int repaid, float interest, int expected)
        {
            _loanManager.AmountBorrowed = borrowed;
            _loanManager.AmountRepaid = repaid;
            _loanManager.Interest = interest;
            Assert.Equal(expected, _loanManager.CalculateBalance);
        }

        [Fact]
        public void CalculateBalance_ReturnsZero_IfNegative()
        {
            _loanManager.AmountBorrowed = 500;
            _loanManager.AmountRepaid = 800;
            Assert.Equal(0, _loanManager.CalculateBalance);
        }

        [Theory]
        [InlineData(2, 165)]
        [InlineData(3, 110)]
        [InlineData(4, 83)]
        public void CalculateDailyAmount_Correct(int duration, int expected)
        {
            _loanManager.Duration = duration;
            Assert.Equal(expected, _loanManager.CalculateDailyAmount);
        }

        [Theory]
        [InlineData(300, 102)]
        [InlineData(4353, 1480)]
        [InlineData(3455, 1175)]
        [InlineData(234234, 79640)]
        public void CalculateLateFees_Correct(int balance, int expected)
        {
            _loanManager.LateChargeRate = 0.34f;
            _loanManager.Balance = balance;
            Assert.Equal(expected, _loanManager.CalculateLateFees);
        }

        [Theory]
        [InlineData(500, 200, 150, 15)]
        [InlineData(500, 500, 500, 0)]
        [InlineData(500, 0, 0, 275)]
        public void CalculateAmountToPayToday_Correct(int borrowed, int repaid, int paidToday, int expected)
        {
            _loanManager.AmountBorrowed = borrowed;
            _loanManager.AmountRepaid = repaid;
            _loanManager.AmountRepaidToday = paidToday;
            _loanManager.DailyAmount = (int)_loanManager.CalculateDailyAmount;
            Assert.Equal(expected, _loanManager.CalculateAmountToPayToday);
        }
    }
}