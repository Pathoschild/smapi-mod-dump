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
using System.Text;
using System.Threading.Tasks;

namespace ChessBoard
{
    public class Question
    {
        public string Text { get; set; }
        public List<Choice> Choices { get; set; } = new List<Choice>();

        public Question(string text, params Choice[] choices)
        {
            Text = text;

            foreach (Choice choice in choices)
                Choices.Add(choice);
        }
    }
}
