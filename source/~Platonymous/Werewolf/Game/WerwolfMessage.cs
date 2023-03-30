/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Werewolf.Game
{
    public class WerwolfMessage : WerwolfMPMessage
    {
        public override string Type { get; set; } = "Message";
        public WerwolfMessageType MessageType { get; set; }
        public string Message { get; set; }

        public string Title { get; set; }

        public WerwolfMessage()
        {

        }
        public WerwolfMessage(long sendTo, long sendFrom, WerwolfGame game, WerwolfMessageType type, string message, string title, string callback = null) : base(sendTo, sendFrom, game, callback)
        {
            MessageType = type;
            Message = message;
            Title = title;
        }
    }
}
