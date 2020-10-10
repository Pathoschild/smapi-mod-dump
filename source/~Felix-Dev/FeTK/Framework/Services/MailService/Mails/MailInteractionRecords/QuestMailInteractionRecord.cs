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
    /// Encapsulates player interaction data specific to a <see cref="QuestMail"/> instance.
    /// </summary>
    public class QuestMailInteractionRecord : MailInteractionRecord
    {
        /// <summary>
        /// Create a new instance of the <see cref="QuestMailInteractionRecord"/> class.
        /// </summary>
        /// <param name="questId">The ID of the quest included in the mail.</param>
        /// <param name="isQuestAccepted">Whether the quest was accepted or not.</param>
        public QuestMailInteractionRecord(int questId, bool isQuestAccepted)
        {
            QuestId = questId;
            IsQuestAccepted = isQuestAccepted;
        }

        /// <summary>
        /// The ID of the quest included in the mail.
        /// </summary>
        /// <remarks>A quest ID less than one (1) indicates no quest.</remarks>
        public int QuestId { get; }

        /// <summary>
        /// Whether the quest was accepted or not.
        /// </summary>
        public bool IsQuestAccepted { get; }
    }
}
