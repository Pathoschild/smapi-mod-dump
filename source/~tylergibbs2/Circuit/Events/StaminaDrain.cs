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
    internal class StaminaDrain : EventBase
    {
        public StaminaDrain(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Stamina Drain";
        }

        public override string GetChatWarningMessage()
        {
            return "Everything you do starts to feel harder...";
        }

        public override string GetChatStartMessage()
        {
            return "Even standing is a struggle!";
        }

        public override string GetDescription()
        {
            return "All stamina usage is doubled.";
        }
    }
}
