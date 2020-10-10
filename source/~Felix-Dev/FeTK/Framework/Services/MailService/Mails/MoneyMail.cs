/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FelixDev.StardewMods.Common.StardewValley;
using StardewModdingAPI.Utilities;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Represents a Stardew Valley game letter optionally attached with a monetary value the mail recipient will receive.
    /// </summary>
    public class MoneyMail : Mail, IMoneyMailContent
    {
        /// <summary>The monetary value attached to the mail.</summary>
        private int attachedMoney;

        /// <summary>The currency of the monetary value attached to the mail.</summary>
        private Currency currency;

        /// <summary>
        /// Create a new instance of the <see cref="MoneyMail"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="attachedMoney">The monetary value attached to the mail.</param>
        /// <param name="currency">The currency of the <paramref name="attachedMoney"/>.</param>
        /// <exception cref="ArgumentException">The speicified <paramref name="id"/> is <c>null</c>, empty or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified <paramref name="attachedMoney"/> is less than zero -or-
        /// the specified <paramref name="currency"/> is invalid.
        /// </exception>
        public MoneyMail(string id, string text, int attachedMoney, Currency currency = Currency.Money)
            : base(id, text)
        {
            if (attachedMoney < 0)
            {
                throw new ArgumentOutOfRangeException("The specified monetary value has to greater than or equal to zero!", nameof(attachedMoney));
            }

            if (!Enum.IsDefined(typeof(Currency), currency))
            {
                throw new ArgumentOutOfRangeException(nameof(currency));
            }

            this.attachedMoney = attachedMoney;
            this.currency = currency;
        }

        /// <summary>
        /// The monetary value attached to the mail.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The attached monetary value cannot be less than zero.</exception>
        public int AttachedMoney
        {
            get => attachedMoney;
            set => attachedMoney = (value >= 0)
                ? value
                : throw new ArgumentOutOfRangeException(nameof(value), "The attached monetary value has to greater than or equal to zero!");
        }

        /// <summary>
        /// The currency of the monetary value attached to the mail.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The specified currency is invalid.</exception>
        public Currency Currency
        {
            get => currency;
            set
            {
                if (!Enum.IsDefined(typeof(Currency), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                currency = value;
            }
        }
    }
}
