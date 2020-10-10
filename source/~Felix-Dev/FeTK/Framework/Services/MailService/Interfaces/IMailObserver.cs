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
    /// Provides an API to notify a <see cref="IMailObserver"/> instance of mail events.
    /// </summary>
    internal interface IMailObserver
    {
        /// <summary>
        /// Notify an observer that a mail is being opened.
        /// </summary>
        /// <param name="e">Information about the mail being opened.</param>
        void OnMailOpening(MailOpeningEventArgs e);

        /// <summary>
        /// Notify an observer that a mail has been closed.
        /// </summary>
        /// <param name="e">Information about the closed mail.</param>
        void OnMailClosed(MailClosedCoreEventArgs e);
    }
}
