/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace LandGrants.Game
{
    public abstract class WerwolfMPMessage
    {
        public abstract string Type { get; set; }

        public long SendTo { get; set; }

        public long SendFrom { get; set; }

        public string GameID { get; set; }

        public string Callback {get; set;}

        public WerwolfMPMessage()
        {

        }

        public WerwolfMPMessage(long sendTo, long sentFrom, WerwolfGame game, string callbackid = null, Action<string, string> callback = null, Action onDisconnect = null, Action onTimeout = null, int timeout = -1)
        {
            SendFrom = sentFrom;
            SendTo = sendTo;
            GameID = game.GameID;
            if (callbackid != null)
            {
                Callback = callbackid;
                game.AddCallback(new WerwolfCallbackRequest(sendTo, callbackid, callback, onDisconnect, onTimeout, timeout));
            }
        }

        public WerwolfMPMessage(long sendTo, long sentFrom, WerwolfClientGame game, string callbackid = null, Action<string, List<string>> callback = null)
        {
            SendFrom = sentFrom;
            SendTo = sendTo;
            GameID = game.GameID;
        }
    }
}

