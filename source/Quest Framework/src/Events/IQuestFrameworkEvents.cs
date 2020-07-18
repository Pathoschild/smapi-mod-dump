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
        /// Place for register quests, hooks and etc.
        /// </summary>
        event EventHandler<GettingReadyEventArgs> GettingReady;

        /// <summary>
        /// Quest Framework is ready.
        /// Here you can't to do anything with quest registry, hooks and etc.
        /// </summary>
        event EventHandler<ReadyEventArgs> Ready;
    }
}
