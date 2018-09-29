using System;

namespace CustomGuildChallenges.API
{
    public interface ISaveAnywhereAPI
    {
        /*********
        ** Events
        *********/
        /// <summary>
        ///     Event that fires before game save
        /// </summary>
        event EventHandler BeforeSave;
        /// <summary>
        ///     Event that fires after game save
        /// </summary>
        event EventHandler AfterSave;
        /// <summary>
        ///     Event that fires after game load
        /// </summary>
        event EventHandler AfterLoad;
    }
}
