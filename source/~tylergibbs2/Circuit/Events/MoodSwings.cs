/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

namespace Circuit.Events
{
    internal class MoodSwings : EventBase
    {
        public MoodSwings(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Mood Swings";
        }

        public override string GetChatWarningMessage()
        {
            return "Everyone seems a little on edge lately...";
        }

        public override string GetChatStartMessage()
        {
            return "People are having mood swings!";
        }

        public override string GetDescription()
        {
            return "All changes to friendship points are doubled.";
        }
    }
}
