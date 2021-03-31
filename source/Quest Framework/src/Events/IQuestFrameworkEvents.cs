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

namespace QuestFramework.Events
{
    /// <summary>
    /// Events of Quest Framework
    /// </summary>
    public interface IQuestFrameworkEvents
    {
        /// <summary>
        /// Quest Framework lifecycle state changed event.
        /// </summary>
        event EventHandler<ChangeStateEventArgs> ChangeState;

        /// <summary>
        /// Quest Framework getting ready.
        /// Place for register quests, boards, hooks and etc.
        /// </summary>
        event EventHandler<GettingReadyEventArgs> GettingReady;

        /// <summary>
        /// Quest Framework is ready.
        /// Here you can't to do anything with quest registry, hooks and etc.
        /// </summary>
        event EventHandler<ReadyEventArgs> Ready;

        /// <summary>
        /// A quest was completed
        /// </summary>
        event EventHandler<QuestEventArgs> QuestCompleted;

        /// <summary>
        /// A quest was accepted and added to quest log
        /// </summary>
        event EventHandler<QuestEventArgs> QuestAccepted;

        /// <summary>
        /// A quest was removed from log
        /// </summary>
        event EventHandler<QuestEventArgs> QuestRemoved;

        /// <summary>
        /// Quest log menu was open
        /// </summary>
        event EventHandler<EventArgs> QuestLogMenuOpen;

        /// <summary>
        /// Quest log menu was closed
        /// </summary>
        event EventHandler<EventArgs> QuestLogMenuClosed;

        /// <summary>
        /// Managed questlog and/or offers was refreshed
        /// </summary>
        event EventHandler<EventArgs> Refreshed;
    }
}
