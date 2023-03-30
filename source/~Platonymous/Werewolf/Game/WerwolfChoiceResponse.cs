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
    public class WerwolfChoiceResponse : WerwolfMPMessage
    {
        public string ChoiceID { get; set; }
        public string Answer { get; set; }
        public override string Type { get; set; } = "Response";

        public WerwolfChoiceResponse()
        {

        }
        public WerwolfChoiceResponse(WerwolfClientGame game, string choiceid, string answer) : base(game.Host, game.LocalPlayer.ID, game)
        {
            ChoiceID = choiceid;
            Answer = answer;
        }
    }
}
