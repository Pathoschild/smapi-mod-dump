/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace HelpWanted
{
    public class HelpWantedAPI : IHelpWantedAPI
    {
        public void AddQuestToday(IQuestData data)
        {
            ModEntry.SMonitor.Log($"Adding mod quest data {data.quest.GetType()}");
            ModEntry.modQuestList.Add(data);
        }
    }

    public interface IHelpWantedAPI
    {
        public void AddQuestToday(IQuestData data);
    }
}