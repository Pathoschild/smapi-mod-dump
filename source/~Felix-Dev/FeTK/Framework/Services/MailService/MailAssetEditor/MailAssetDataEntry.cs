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

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Encapsulates the mail data needed to add a mail to the game's content mail asset 
    /// using a <see cref="MailAssetEditor"/> instance.
    /// </summary>
    internal class MailAssetDataEntry
    {
        /// <summary>
        /// Create a new instance of the <see cref="MailAssetDataEntry"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="content">The content of the mail.</param>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="id"/> is <c>null</c> or empty -or-
        /// the specified <paramref name="content"/> is <c>null</c> or empty.
        /// </exception>
        public MailAssetDataEntry(string id, string content)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(nameof(id));
            }

            // Mail content cannot be empty as otherwise the game won't create a LetterViewerMenu UI
            // for the mail.
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException(nameof(content));
            }

            Id = id;
            Content = content;
        }

        /// <summary>The ID of the mail.</summary>
        public string Id { get; }

        /// <summary>The content of the mail.</summary>
        public string Content { get; }
    }
}