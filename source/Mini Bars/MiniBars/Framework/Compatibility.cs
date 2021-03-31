/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniBars.Framework
{
    public class Compatibility
    {
        public static bool BlackListedMonster(string _monster_name)
        {
            if (Game1.player.currentLocation.Name == "UnderwaterBeach")
            {
                if (_monster_name == "Serpent") return true;
                else if (_monster_name == "Rock Crab") return true;
                else return false;
            }
            else if (Game1.player.currentLocation.Name == "UnderwaterMountain")
            {
                if (_monster_name == "Pepper Rex") return true;
                else return false;
            }
            return false;
        }
    }
}
