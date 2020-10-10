/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to interact with the content of an <see cref="ItemMail"/> instance.
    /// </summary>
    public class ItemMailContent : MailContent, IItemMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="ItemMailContent"/> class.
        /// </summary>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="attachedItems">The items attached to the mail. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public ItemMailContent(string text, IList<Item> attachedItems) 
            : base(text)
        {
            AttachedItems = attachedItems;
        }

        /// <summary>
        /// The items, if any, attached to the mail. Can be <c>null</c>.
        /// </summary>
        public IList<Item> AttachedItems { get; set; }
    }
}
