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

namespace LandGrants.Game
{
    public class WerwolfChoice : WerwolfMPMessage
    {
        public override string Type { get; set; } = "Choice";
        public string ChoiceID {get; set;}

        public string Question { get; set; }

        public List<WerwolfChoiceOption> Options { get; set; } = new List<WerwolfChoiceOption>();

        public WerwolfChoice()
        {

        }

        public WerwolfChoice(long sendTo, long sendFrom, WerwolfGame game, string choiceID, string question, List<WerwolfChoiceOption> options, Action<string, string> callback)
            : base(sendTo, sendFrom, game, choiceID, callback)
        { 
            ChoiceID = choiceID;
            Question = question;
            Options = options;
        }
    }
}

