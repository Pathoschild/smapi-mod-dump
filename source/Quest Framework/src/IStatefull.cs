/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using System;

namespace QuestFramework
{
    /// <summary>
    /// Used for statefull objects (like in <cref>CustomQuest<IState></cref>)
    /// </summary>
    public interface IStatefull
    {
        /// <summary>
        /// Reset object state
        /// </summary>
        [Obsolete("Deprecated. Use method CustomQuest.Reset instead")]
        void ResetState();

        /// <summary>
        /// Sync object state with the store or via network
        /// </summary>
        void Sync();
    }

    /// <summary>
    /// Statefull object with state property
    /// </summary>
    /// <typeparam name="TState">Type of state</typeparam>
    public interface IStatefull<TState> : IStatefull
    {
        TState State { get; }
    }
}
