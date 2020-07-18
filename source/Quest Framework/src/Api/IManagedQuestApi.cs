using QuestFramework.Offers;
using QuestFramework.Quests;
using System.Collections.Generic;

namespace QuestFramework.Api
{
    public interface IManagedQuestApi
    {
        /// <summary>
        /// Add custom quest to player's questlog and mark then accepted and new.
        /// </summary>
        /// <param name="questName">Name without @ has resolved in your mod scope</param>
        void AcceptQuest(string questName);

        /// <summary>
        /// Resolve game quest id and returns custom quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CustomQuest GetById(int id);

        /// <summary>
        /// Register custom quest (vanilla or custom type)
        /// WARNING: Can't register new quests when game is loaded. 
        /// Please register all your quests and quest types before game will be loaded. 
        /// (Before Content.IsWorldReady is true)
        /// </summary>
        /// <param name="quest">Quest template</param>
        void RegisterQuest(CustomQuest quest);

        /// <summary>
        /// Mark quest by given name as completed,
        /// if quest with this name exists and is managed.
        /// </summary>
        /// <param name="questName"></param>
        void CompleteQuest(string questName);

        /// <summary>
        /// Schedule a quest for add to specified quest source 
        /// which will be available to accept by player.
        /// (like offer quest on bulletin board, deliver via mail or etc)
        /// </summary>
        /// <param name="schedule"></param>
        void OfferQuest(QuestOffer schedule);

        /// <summary>
        /// Get quest schedules for today by the source name.
        /// </summary>
        /// <param name="source"/>
        /// <exception cref="InvalidOperationException">
        ///     Throws when this method is called outside of loaded game
        /// </exception>
        IEnumerable<QuestOffer> GetTodayQuestOffers(string source);

        /// <summary>
        /// Get quest schedules with attributes (if that schedules has them)
        /// for today by the source name.
        /// </summary>
        /// <typeparam name="TAttributes"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="InvalidOperationException">
        ///     Throws when this method is called outside of loaded game
        /// </exception>
        IEnumerable<QuestOffer<TAttributes>> GetTodayQuestOffers<TAttributes>(string source);
    }
}
