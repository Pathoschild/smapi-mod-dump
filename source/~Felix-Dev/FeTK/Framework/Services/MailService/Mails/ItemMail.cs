/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Represents a Stardew Valley game letter optionally attached with items.
    /// </summary>
    public class ItemMail : Mail, IItemMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="ItemMail"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="item">The attached item of the mail. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="id"/> is <c>null</c>, empty or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public ItemMail(string id, string text, Item item)
            : this(id, text, (item != null) ? new List<Item>() { item } : null)
        {}

        /// <summary>
        /// Create a new instance of the <see cref="ItemMail"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="attachedItems">The items attached to the mail. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentException">The speicified <paramref name="id"/> is <c>null</c>, empty or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public ItemMail(string id, string text, IList<Item> attachedItems) 
            : base(id, text)
        {
            this.AttachedItems = attachedItems;
        }

        /// <summary>
        /// The items, if any, attached to the mail. Can be <c>null</c>.
        /// </summary>
        public IList<Item> AttachedItems { get; set; }
    }
}
