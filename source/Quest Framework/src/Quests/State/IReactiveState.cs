/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace QuestFramework.Quests.State
{
    internal interface IReactiveState
    {
        [JsonIgnore]
        bool WasChanged { get; }

        event Action<JObject> OnChange;
        void Initialize(CustomQuest customQuest);
        void StateSynced();
    }
}
