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
using System.Linq;

namespace Werewolf.Game
{
    public class WerwolfCallbackRequest
    {
        public string CallbackID { get; set; }
        public Action<string, string> Callback { get; set; }
        public Action OnDisconnect { get; set; }
        public Action OnTimeout { get; set; }

        public int TimeOut { get; set; }

        public long Player { get; set; }

        public bool Finished { get; set; } = false;

        public WerwolfCallbackRequest()
        {

        }

        public WerwolfCallbackRequest(long player, string callbackID, Action<string, string> callback, Action onDisconnect, Action onTimeout, int timeOut)
        {
            CallbackID = callbackID;
            Callback = callback;
            OnDisconnect = onDisconnect;
            OnTimeout = onTimeout;
            TimeOut = timeOut;
            Player = player;
        }

        public void ReceiveCallback(string id, string answer)
        {
            Finished = true;
            Callback?.Invoke(id, answer);
        }

        public void CheckCallback(WerwolfGame game)
        {
            TimeOut--;
            if(TimeOut == 0)
            {
                Finished = true;
                OnTimeout();
            }
            else if (game.Players.First(p => p.PlayerID == Player).HasDisconnected)
            {
                Finished = true;
                OnDisconnect();
            }
        }
    }
}
