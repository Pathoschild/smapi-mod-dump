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

namespace Snake
{
    public class Highscore
    {
        public int Value { get; set; } = 0;
        public String Name { get; set; } = "None";

        public Highscore()
        {

        }

        public Highscore(string name, int value)
        {
            Value = value;
            Name = name;
        }
    }
}
