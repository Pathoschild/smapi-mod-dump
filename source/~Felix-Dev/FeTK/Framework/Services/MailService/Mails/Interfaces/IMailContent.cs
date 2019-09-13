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
