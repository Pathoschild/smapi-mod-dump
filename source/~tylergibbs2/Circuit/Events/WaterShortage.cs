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
    internal class WaterShortage : EventBase
    {
        public WaterShortage(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Water Shortage";
        }

        public override string GetChatWarningMessage()
        {
            return "The heat seems to be picking up...";
        }

        public override string GetChatStartMessage()
        {
            return "The spring has dried up!";
        }

        public override string GetDescription()
        {
            return "Sprinklers are unable to function.";
        }
    }
}
