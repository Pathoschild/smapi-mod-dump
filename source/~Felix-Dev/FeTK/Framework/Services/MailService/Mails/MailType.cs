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
    /// Defines constants that specify the type of a <see cref="Mail"/>.
    /// </summary>
    internal enum MailType
    {
        /// <summary>A standard mail with text content only.</summary>
        PlainMail = 0,
        /// <summary>A mail with text and attached items.</summary>
        ItemMail,
        /// <summary>A mail with text and attached money.</summary>
        MoneyMail,
        /// <summary>A mail with text and an attached recipe.</summary>
        RecipeMail,
        /// <summary>A mail with text and a quest included.</summary>
        QuestMail,
    }
}
