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

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to interact with the content of a <see cref="Mail"/> instance.
    /// </summary>
    public interface IMailContent
    {
        /// <summary>
        /// The text content of the mail.
        /// </summary>
        /// <exception cref="ArgumentNullException">The mail text is <c>null</c>.</exception>
        string Text { get; set; }
    }
}
