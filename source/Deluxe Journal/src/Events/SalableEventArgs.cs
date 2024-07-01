/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;

namespace DeluxeJournal.Events
{
    public class SalableEventArgs(Farmer player, ISalable salable, int amount) : EventArgs
    {
        /// <summary>The player handling the <see cref="ISalable"/>.</summary>
        public Farmer Player { get; } = player;

        /// <summary>The target <see cref="ISalable"/>.</summary>
        public ISalable Salable { get; } = salable;

        /// <summary>Amount associated with the <see cref="ISalable"/>.</summary>
        public int Amount { get; } = amount;
    }
}
