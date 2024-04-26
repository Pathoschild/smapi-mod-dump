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

namespace LandGrants.Game
{
    public class WerwolfActionRequest : WerwolfMPMessage
    {
        public string ActionID { get; set; }

        public long TargetPlayer { get; set; }

        public override string Type { get; set; } = "Action";

        public WerwolfActionRequest()
        {

        }

        public WerwolfActionRequest(WerwolfClientGame game, string actionID, long targetPlayer) : base(game.Host, game.LocalPlayer.ID, game)
        {
            ActionID = actionID;
            TargetPlayer = targetPlayer;
        }
    }
}
