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
    internal class Nauseous : EventBase
    {
        public Nauseous(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Nauseous";
        }

        public override string GetChatWarningMessage()
        {
            return "Your stomach doesn't feel right...";
        }

        public override string GetChatStartMessage()
        {
            return "The pain is unbearable!";
        }

        public override string GetDescription()
        {
            return "You are unable to consume anything.";
        }
    }
}
