/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json.Linq;

namespace QuestFramework.Quests.State
{
    public interface IPersistentState
    {
        JObject GetState();
        void Reset();
        void SetState(JObject state);
    }
}
