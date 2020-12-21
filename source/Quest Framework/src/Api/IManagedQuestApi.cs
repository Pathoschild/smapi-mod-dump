/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Offers;
using QuestFramework.Quests;
using System;
using System.Collections.Generic;

namespace QuestFramework.Api
{
    public interface IManagedQuestApi
    {
        /// <summary>
        /// Add custom quest to player's questlog and mark then accepted and new.
        /// </summary>
        /// <param name="questName">Name without @ has resolved in your mod scope</param>
        void AcceptQuest(string questName, bool silent = false);

        /// <summary>
        /// DEPRECATED! Resolve game quest id and returns custom quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete("This API is deprecated! Use IManagedQuestApi.GetQuestById instead.", true)]
        CustomQuest GetById(int id);

        /// <summary>
        /// Resolve game quest id and returns custom quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CustomQuest GetQuestById(int id);

        /// <summary>
        /// Resolve game quest by name and return it's instance
        /// You can request quest by fullname (with @) or localname (localname returns quest in this mod-managed scope).
        /// </summary>
        /// <param name="questName"></param>
        /// <returns></returns>
        CustomQuest GetQuestByName(string questName);

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

        /// <summary>
        /// Exposes global condition for usage in offers or hooks.
        /// </summary>
        /// <param name="conditionName">Name of condition</param>
        /// <param name="conditionHandler">Handler for this condition</param>
        void ExposeGlobalCondition(string conditionName, Func<string, CustomQuest, bool> conditionHandler);

        /// <summary>
        /// Exposes a quest type for content packs and other mods based on QF.
        /// </summary>
        /// <typeparam name="TQuest">Quest class type</typeparam>
        /// <param name="type">Name of quest type in registry</param>
        /// <param name="factory">Factory of quest of declared type</param>
        void ExposeQuestType<TQuest>(string type, Func<TQuest> factory) where TQuest : CustomQuest;

        /// <summary>
        /// Exposes a quest type for content packs and other mods based on QF.
        /// </summary>
        /// <typeparam name="TQuest">Quest class type</typeparam>
        /// <param name="type">Name of quest type in registry</param>
        void ExposeQuestType<TQuest>(string type) where TQuest : CustomQuest, new();

        bool HasQuestType(string type);
    }
}
