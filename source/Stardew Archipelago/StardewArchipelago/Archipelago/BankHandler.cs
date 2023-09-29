/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Numerics;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewArchipelago.Archipelago
{
    public class BankHandler
    {
        // private const string BANKING_KEY = "EnergyLink";
        public const string BANKING_TEAM_KEY = "EnergyLink{0}";
        private const string DEPOSIT_COMMAND = "Deposit";
        private const string WITHDRAW_COMMAND = "Withdraw";
        private const int MAX_DISPLAY_MONEY = 100000000; // 100 Millions
        public const int EXCHANGE_RATE = 10000000; // To be adjusted based on feedback (pun intended)

        public ArchipelagoClient _archipelago;

        public BankHandler(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
        }

        public bool HandleBankCommand(string message)
        {
            if (_archipelago == null)
            {
                return false;
            }

            var bankPrefix = $"{ChatForwarder.COMMAND_PREFIX}bank";
            if (!message.StartsWith(bankPrefix))
            {
                return false;
            }

            if (!_archipelago.SlotData.Banking || !_archipelago.MakeSureConnected()) // Did I enable that feature?
            {
                Game1.chatBox?.addMessage($"Your archipelago slot does not have a Joja Capital account", Color.Gold);
                return true;
            }

            var bankCommand = message.Substring(bankPrefix.Length).Trim();

            if (string.IsNullOrWhiteSpace(bankCommand))
            {
                var currentBankAmount = GetBankMoneyAmount();
                PrintCurrentBalance(currentBankAmount);
                return true;
            }

            var bankCommandParts = bankCommand.Split(" ");
#if DEBUG
            if (bankCommandParts[0] == "reset")
            {
                HandleResetCommand();
            }
#endif

            if (bankCommandParts.Length != 2)
            {
                PrintUsageRules();
                return true;
            }

            if (bankCommandParts[0].Equals(DEPOSIT_COMMAND, StringComparison.OrdinalIgnoreCase))
            {
                HandleDepositCommand(bankCommandParts[1]);
                return true;
            }

            if (bankCommandParts[0].Equals(WITHDRAW_COMMAND, StringComparison.OrdinalIgnoreCase))
            {
                HandleWithdrawCommand(bankCommandParts[1]);
                return true;
            }

            PrintUsageRules();
            return true;
        }

        private void HandleResetCommand()
        {
#if RELEASE
    return;
#endif
            var bankingKey = string.Format(BANKING_TEAM_KEY, _archipelago.GetTeam());
            _archipelago.SetBigIntegerDataStorage(Scope.Global, bankingKey, 0);

            Game1.chatBox?.addMessage($"You have successfully reset your bank account to {0}$", Color.Gold);
        }

        private void HandleDepositCommand(string amount)
        {
            if (!int.TryParse(amount, out var amountToDeposit))
            {
                PrintUsageRules();
                return;
            }

            if (amountToDeposit > Game1.player.Money)
            {
                Game1.chatBox?.addMessage($"You do not have enough money to make this deposit", Color.Gold);
                return;
            }

            var tax = (int)Math.Round(amountToDeposit * _archipelago.SlotData.BankTax);
            var realDepositAmount = amountToDeposit - tax;
            AddToBank(realDepositAmount);
            Game1.player.Money -= amountToDeposit;

            Game1.chatBox?.addMessage($"You have successfully made a deposit of {realDepositAmount}$ into your shared account", Color.Gold);
            Game1.chatBox?.addMessage($"You have been charged a tax of {tax}$", Color.Gold);
            Game1.chatBox?.addMessage($"Thank you for using Joja Capital", Color.Gold);
        }

        private void HandleWithdrawCommand(string amount)
        {
            if (!int.TryParse(amount, out var amountToWithdraw))
            {
                PrintUsageRules();
                return;
            }

            var currentBalance = GetBankMoneyAmount();

            if (amountToWithdraw > currentBalance)
            {
                Game1.chatBox?.addMessage($"There is not enough money in your shared account for this withdrawal.", Color.Gold);
                PrintCurrentBalance(currentBalance);
                return;
            }
            
            RemoveFromBank(amountToWithdraw);
            Game1.player.addUnearnedMoney(amountToWithdraw);

            Game1.chatBox?.addMessage($"You have successfully withdrawn {amountToWithdraw}$ from your shared account", Color.Gold);
            PrintCurrentBalance();
            Game1.chatBox?.addMessage($"Thank you for using Joja Capital", Color.Gold);
        }

        private void PrintCurrentBalance()
        {
            PrintCurrentBalance(GetBankMoneyAmount());
        }

        private void PrintCurrentBalance(BigInteger currentBankAmount)
        {
            Game1.chatBox?.addMessage($"Current Balance: {currentBankAmount}$", Color.Gold);
        }

        private BigInteger GetBankMoneyAmount()
        {
            var realAmountJoules = GetBankJoulesAmount();

            var convertedAmount = JoulesToMoney(realAmountJoules);
            if (convertedAmount > MAX_DISPLAY_MONEY)
            {
                return MAX_DISPLAY_MONEY;
            }

            return convertedAmount;
        }

        private BigInteger GetBankJoulesAmount()
        {
            var bankingKey = string.Format(BANKING_TEAM_KEY, _archipelago.GetTeam());
            var realAmountJoules = _archipelago.ReadBigIntegerFromDataStorage(Scope.Global, bankingKey);
            if (realAmountJoules == null)
            {
                return 0;
            }

            return realAmountJoules.Value;
        }

        private void AddToBank(int amountToAdd)
        {
            var bankingKey = string.Format(BANKING_TEAM_KEY, _archipelago.GetTeam());
            var currentAmountJoules = GetBankJoulesAmount();
            var amountToAddJoules = MoneyToJoules(amountToAdd);
            var bankAmountAfterOperation = currentAmountJoules + amountToAddJoules;
            _archipelago.SetBigIntegerDataStorage(Scope.Global, bankingKey, bankAmountAfterOperation);
        }

        private void RemoveFromBank(BigInteger amountToRemove)
        {
            var bankingKey = string.Format(BANKING_TEAM_KEY, _archipelago.GetTeam());
            var currentAmountJoules = GetBankJoulesAmount();
            var amountToRemoveJoules = MoneyToJoules(amountToRemove);
            var bankAmountAfterOperation = currentAmountJoules - amountToRemoveJoules;
            _archipelago.SetBigIntegerDataStorage(Scope.Global, bankingKey, bankAmountAfterOperation);
        }

        private BigInteger MoneyToJoules(BigInteger money)
        {
            return money * EXCHANGE_RATE;
        }

        private BigInteger JoulesToMoney(BigInteger joules)
        {
            return joules / EXCHANGE_RATE;
        }

        private static void PrintUsageRules()
        {
            Game1.chatBox?.addMessage($"Usage: !!bank [deposit|withdraw] [amount]", Color.Gold);
        }
    }
}
