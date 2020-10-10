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
    /// Provides an API to interact with the content of a <see cref="QuestMail"/> instance.
    /// </summary>
    public class QuestMailContent : MailContent, IQuestMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="QuestMailContent"/> class.
        /// </summary>
        /// <param name="text">The text content of the mail.</param>
        /// <param name="questId">The ID of the quest included in the mail. A quest ID less than one (1) indicates no quest.</param>
        /// <param name="isAutomaticallyAccepted">Whether the quest is automatically added to the player's quest log or needs to be manually accepted.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public QuestMailContent(string text, int questId, bool isAutomaticallyAccepted)
            : base(text)
        {
            QuestId = questId;
            IsAutomaticallyAccepted = isAutomaticallyAccepted;
        }

        /// <summary>
        /// The ID of the quest included in the mail. A quest ID less than one (1) indicates no quest.
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// Determines whether the quest is automatically added to the player's quest log on opening the mail, 
        /// or of the player needs to manually accept it.
        /// </summary>
        public bool IsAutomaticallyAccepted { get; set; }
    }
}
