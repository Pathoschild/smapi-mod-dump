/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Immutable;
using System.Linq;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Actions to be executed on catch.
    /// </summary>
    [JsonDescribe]
    public record CatchActions
    {
        /// <summary>
        /// Raise custom events with this name to notify SMAPI mods that this was caught. Event key
        /// format is "namespace:key" (for example "TehPers.FishingOverhaul:GoldenWalnut").
        /// </summary>
        public ImmutableArray<NamespacedKey> CustomEvents { get; init; } =
            ImmutableArray<NamespacedKey>.Empty;

        /// <summary>
        /// Sets one or more mail flags.
        /// </summary>
        public ImmutableArray<string> SetFlags { get; init; } = ImmutableArray<string>.Empty;

        /// <summary>
        /// Sets one or more quests as active.
        /// </summary>
        public ImmutableArray<int> StartQuests { get; init; } = ImmutableArray<int>.Empty;

        /// <summary>
        /// Adds mail entries to the player's mail tomorrow.
        /// </summary>
        public ImmutableArray<string> AddMail { get; init; } = ImmutableArray<string>.Empty;

        /// <summary>
        /// Starts conversations. The key is the conversation ID and the value is the number of days.
        /// </summary>
        public ImmutableDictionary<string, int> StartConversations { get; init; } =
            ImmutableDictionary<string, int>.Empty;

        /// <summary>
        /// Executes these actions.
        /// </summary>
        /// <param name="fishingApi">The fishing API.</param>
        /// <param name="catchInfo">The catch information.</param>
        public void OnCatch(IFishingApi fishingApi, CatchInfo catchInfo)
        {
            var user = catchInfo.FishingInfo.User;

            // Custom events
            foreach (var customEvent in this.CustomEvents)
            {
                fishingApi.RaiseCustomEvent(new(catchInfo, customEvent));
            }

            // Mail flags
            foreach (var flag in this.SetFlags)
            {
                user.mailReceived.Add(flag);
            }

            // Quests
            foreach (var questId in this.StartQuests)
            {
                user.addQuest(questId);
            }

            // New mail
            foreach (var mail in this.AddMail.Where(id => !user.hasOrWillReceiveMail(id)))
            {
                user.mailForTomorrow.Add(mail);
            }

            // New conversation topics
            foreach (var (key, duration) in this.StartConversations)
            {
                user.addEvent(key, duration);
            }
        }
    }
}