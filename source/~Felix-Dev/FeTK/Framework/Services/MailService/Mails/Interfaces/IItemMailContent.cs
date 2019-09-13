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
    public interface IItemMailContent : IMailContent
    {
        /// <summary>
        /// The items, if any, attached to the mail. Can be <c>null</c>.
        /// </summary>
        IList<Item> AttachedItems { get; set; }
    }
}
