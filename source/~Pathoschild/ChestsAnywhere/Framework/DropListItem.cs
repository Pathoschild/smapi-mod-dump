/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.ChestsAnywhere.Framework
{
    /// <summary>An item in a drop list.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    internal class DropListItem<TItem>
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item's index in the list.</summary>
        public int Index { get; set; }

        /// <summary>The display name.</summary>
        public string Name { get; set; }

        /// <summary>The item value.</summary>
        public TItem Value { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="index">The item's index in the list.</param>
        /// <param name="name">The display name.</param>
        /// <param name="value">The item value.</param>
        public DropListItem(int index, string name, TItem value)
        {
            this.Index = index;
            this.Name = name;
            this.Value = value;
        }
    }
}
