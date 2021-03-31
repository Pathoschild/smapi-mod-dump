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
using MiniBars.Framework;

namespace MiniBars.Framework
{
    public class Database
    {
        public static string bars_theme;
        public static int distance_x;
        public static int bar_size;

        public static void GetTheme()
        {
            if (ModEntry.config.Bars_Theme == 0)
            {
                bars_theme = "Normal_Themes";
                distance_x = 4;
                bar_size = 20;
            }
            else if (ModEntry.config.Bars_Theme == 2)
            {
                bars_theme = "Simple_Themes";
                distance_x = 15;
                bar_size = 31;
            }
            else
            {
                bars_theme = "Normal_Themes";
                distance_x = 4;
                bar_size = 20;
            }
        }
    }
}
