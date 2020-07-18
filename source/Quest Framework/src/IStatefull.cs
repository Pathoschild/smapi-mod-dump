using StardewModdingAPI;

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
