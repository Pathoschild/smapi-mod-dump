using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Encapsulates player interaction data specific to a <see cref="ItemMail"/> instance.
    /// </summary>
    public class ItemMailInteractionRecord : MailInteractionRecord
    {
        /// <summary>
        /// Create a new instance of the <see cref="ItemMailInteractionRecord"/> class.
        /// </summary>
        /// <param name="selectedItems">The list of selected items.</param>
        /// <param name="unselectedItems">The list of not selected items.</param>
        public ItemMailInteractionRecord(IList<Item> selectedItems, IList<Item> unselectedItems)
        {
            SelectedItems = selectedItems ?? new List<Item>();
            UnselectedItems = unselectedItems ?? new List<Item>();
        }

        /// <summary>
        /// Get a list of the attached mail items which were selected by the player.
        /// </summary>
        /// <remarks>If there are no selected items, this property returns an empty collection.</remarks>
        public IList<Item> SelectedItems { get; }

        /// <summary>
        /// Get a list of the attached mail items which were not selected by the player.
        /// </summary>
        /// <remarks>If there are no unselected items, this property returns an empty collection.</remarks>
        public IList<Item> UnselectedItems { get; }
    }
}
