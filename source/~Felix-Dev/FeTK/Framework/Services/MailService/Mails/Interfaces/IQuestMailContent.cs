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
    public interface IQuestMailContent : IMailContent
    {
        /// <summary>
        /// The ID of the quest included in the mail. A quest ID less than one (1) indicates no quest.
        /// </summary>
        int QuestId { get; set; }

        /// <summary>
        /// Determines whether the quest is automatically added to the player's quest log on opening the mail, 
        /// or of the player needs to manually accept it.
        /// </summary>
        bool IsAutomaticallyAccepted { get; set; }
    }
}
