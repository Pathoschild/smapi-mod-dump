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
    public interface IItemMailContent : IMailContent
    {
        /// <summary>
        /// The items, if any, attached to the mail. Can be <c>null</c>.
        /// </summary>
        IList<Item> AttachedItems { get; set; }
    }
}
