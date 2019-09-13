using FelixDev.StardewMods.Common.StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Encapsulates player interaction data specific to a <see cref="MoneyMail"/> instance.
    /// </summary>
    public class MoneyMailInteractionRecord : MailInteractionRecord
    {
        /// <summary>
        /// Create a new instance of the <see cref="MoneyMail"/> class.
        /// </summary>
        /// <param name="moneyReceived">The monetary value received by the player.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified <paramref name="moneyReceived"/> is less than zero -or-
        /// the specified <paramref name="currency"/> is invalid.
        /// </exception>
        public MoneyMailInteractionRecord(int moneyReceived, Currency currency)
        {
            if (moneyReceived < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moneyReceived), "The received monetary value cannot be less than zero!");
            }

            if (!Enum.IsDefined(typeof(Currency), currency))
            {
                throw new ArgumentOutOfRangeException(nameof(currency));
            }

            MoneyReceived = moneyReceived;
        }

        /// <summary>
        /// The monetary value received by the player.
        /// </summary>
        public int MoneyReceived { get; }

        /// <summary>
        /// The currency of the monetary value received by the player.
        /// </summary>
        public Currency Currency { get; }
    }
}
