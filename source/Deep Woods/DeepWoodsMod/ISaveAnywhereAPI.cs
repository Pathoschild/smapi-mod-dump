using System;

namespace Omegasis.SaveAnywhere.API
{
    /// <summary>
    ///     Interface for the Save Anywhere API
    ///     Other mods can use this interface to get the
    ///         API from the SMAPI helper
    /// </summary>
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
