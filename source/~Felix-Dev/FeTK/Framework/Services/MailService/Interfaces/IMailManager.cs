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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to add a mail to the game.
    /// </summary>
    interface IMailManager
    {
        /// <summary>
        /// Add a mail to the game.
        /// </summary>
        /// <param name="modId">The ID of the mod which wants to add the mail.</param>
        /// <param name="mailId">The ID of the mail.</param>
        /// <param name="arrivalDay">The day of arrival of the mail.</param>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="modId"/> is <c>null</c>, does not contain at least one 
        /// non-whitespace character or contains an invalid character sequence -or-
        /// the specified <paramref name="mailId"/> is <c>null</c>, does not contain at least one 
        /// non-whitespace character or contains an invalid character sequence -or-
        /// a mail with the specified <paramref name="mailId"/> provided by the mod with the specified <paramref name="modId"/> 
        /// for the specified <paramref name="arrivalDay"/> already exists.
        /// </exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="arrivalDay"/> is <c>null</c>.</exception>
        void Add(string modId, string mailId, SDate arrivalDay);

        /// <summary>
        /// Register a mail sender with the mail manager.
        /// </summary>
        /// <param name="modId">The ID of the mod using the specified mail sender.</param>
        /// <param name="mailSender">The <see cref="IMailSender"/> instance to register.</param>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="modId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character -or-
        /// a mail sender with the specified <paramref name="modId"/> has already been registered.
        /// </exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mailSender"/> is <c>null</c>.</exception>
        void RegisterMailSender(string modId, IMailSender mailSender);

        /// <summary>
        /// Determine whether the player's mailbox contains the specified mail.
        /// </summary>
        /// <param name="modId">The ID of the mod which created this mail.</param>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns>
        /// <c>true</c> if a mail with the specified <paramref name="mailId"/> created by the mod with the 
        /// specified <paramref name="modId"/> is in the player's mailbox; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="modId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character -or-
        /// the specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        bool HasMailInMailbox(string modId, string mailId);

        /// <summary>
        /// Get whether the specified mail was already sent to the player.
        /// </summary>
        /// <param name="modId">The ID of the mod which created this mail.</param>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns>
        /// <c>true</c> if a mail with the specified <paramref name="mailId"/> created by the mod with the 
        /// specified <paramref name="modId"/> was already sent to the player; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="modId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character -or-
        /// the specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        bool HasReceivedMail(string modId, string mailId);
    }
}
