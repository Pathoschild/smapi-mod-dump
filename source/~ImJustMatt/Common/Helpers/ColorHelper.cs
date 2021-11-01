/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Helpers
{
    using Microsoft.Xna.Framework;

    public static class ColorHelper
    {
        public static Color FromTag(string colorTag)
        {
            return colorTag switch
            {
                "color_red" => Color.Red,
                "color_dark_red" => Color.DarkRed,
                "color_pale_violet_red" => Color.PaleVioletRed,
                "color_blue" => Color.Blue,
                "color_green" => Color.Green,
                "color_dark_green" => Color.DarkGreen,
                "color_jade" => Color.Teal,
                "color_brown" => Color.Brown,
                "color_dark_brown" => Color.Maroon,
                "color_yellow" => Color.Yellow,
                "color_dark_yellow" => Color.Goldenrod,
                "color_aquamarine" => Color.Aquamarine,
                "color_purple" => Color.Purple,
                "color_dark_purple" => Color.Indigo,
                "color_cyan" => Color.Cyan,
                "color_pink" => Color.Pink,
                "color_orange" => Color.Orange,
                _ => Color.Gray,
            };
        }
    }
}