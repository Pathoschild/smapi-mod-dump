/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.TwitchApp;
using StardewValley;


namespace BNC.Actions
{
    class MessageAction : BaseAction
    {
        private readonly string _message;

        public MessageAction(string message)
        {
            _message = message;
        }

        public override ActionResponse Handle()
        {
            Game1.addHUDMessage(new HUDMessage(_message, null));
            return ActionResponse.Done;
        }
    }
}
