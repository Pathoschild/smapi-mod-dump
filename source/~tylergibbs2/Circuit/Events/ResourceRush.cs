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
    internal class ResourceRush : EventBase
    {
        public ResourceRush(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Resource Rush";
        }

        public override string GetChatWarningMessage()
        {
            return "The ground seems to be rumbling...";
        }

        public override string GetChatStartMessage()
        {
            return "A loud collapse was heard from the mine.";
        }

        public override string GetDescription()
        {
            return "The mines has increased gem node and ore node spawn rates.";
        }
    }
}
