using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.StardewValley.LetterMenu
{
    /// <summary>
    /// Provides data for the [ItemLetterMenuHelper.MenuClosed] event.
    /// </summary>
    public class ItemLetterMenuClosedEventArgs : EventArgs
    {
        /// <summary>
        /// The selected item or null.
        /// </summary>
        public Item SelectedItem { get; }

        public ItemLetterMenuClosedEventArgs(Item item)
        {
            SelectedItem = item;
        }
    }
}
