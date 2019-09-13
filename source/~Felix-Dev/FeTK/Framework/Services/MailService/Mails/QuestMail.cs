using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Represents a Stardew Valley game letter with a quest included.
    /// </summary>
    public class QuestMail : Mail, IQuestMailContent
    {
        /// <summary>
        /// Create a new instance of the <see cref="QuestMail"/> class.
        /// </summary>
        /// <param name="id">The ID of the mail.</param>
        /// <param name="text">The text content of the mail. A quest ID less than one (1) indicates no quest.</param>
        /// <param name="questId">The ID of the quest included in the mail.</param>
        /// <param name="isAutomaticallyAccepted">Whether the quest is automatically added to the player's quest log or needs to be manually accepted.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="id"/> is <c>null</c>, empty or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public QuestMail(string id, string text, int questId, bool isAutomaticallyAccepted = false) 
            : base(id, text)
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
