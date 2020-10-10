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
    public class MailContent : IMailContent
    {
        /// <summary>The mail's text content.</summary>
        private string text;

        /// <summary>
        /// Create a new instance of the <see cref="MailContent"/> class.
        /// </summary>
        /// <param name="text">The text content of the mail.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public MailContent(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// The text content of the mail.
        /// </summary>
        /// <exception cref="ArgumentNullException">The mail text cannot be <c>null</c>.</exception>
        public string Text
        {
            get => text;
            set => text = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
