/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework;

namespace QuestFramework.Framework.Store
{
    internal interface IStateRestorable : IStatefull
    {
        void RestoreState(StatePayload payload);
        bool VerifyState(StatePayload payload);
    }
}
