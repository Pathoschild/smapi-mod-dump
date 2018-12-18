using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Resources
{


    ///<summary>
    ///
    /// In order to add a color, look up a color online, take the RGB values and subtract the online RBG values from 255 to get the correct light color. A values stays the same.
    ///</summary>
    class LightColors
    {

        //Colors Taken From W3 Schools

            //Get Color Picker Function From Character Creation Menu For Custom Lights

        //http://www.w3schools.com/colors/color_tryit.asp?color=Brownhttp://www.w3schools.com/colors/color_tryit.asp?color=Brown

        /// <summary>
        /// A whiteish sort of color
        /// </summary>
        public static Color AliceBlue = new Color(15, 7, 0,255);

        public static Color AntiqueWhite = new Color(5, 20, 40, 255);

        public static Color Aqua = new Color(255, 0, 0, 255);

        public static Color Aquamarine = new Color(128, 0, 43, 255);

        /// <summary>
        /// A whiteish sort of color witha  hint of blue
        /// </summary>
        public static Color Azure = new Color(15, 0, 0, 255);

        public static Color Beige = new Color(10, 10, 35, 255);

        public static Color Bisque = new Color(0, 27, 59, 255);

        public static Color Black = new Color(255, 255, 255, 255);

        public static Color BlanchedAlmond = new Color(0, 20, 40, 255);

        public static Color Blue = new Color(255, 255, 0, 255);

        public static Color BlueViolet = new Color(117, 212, 29, 255);

        public static Color Brown = new Color(90, 213, 213, 255);

        public static Color BurlyWood = new Color(33, 71, 120, 255);

        public static Color CadetBlue = new Color(160, 97, 95, 255);

        public static Color Chartreuse = new Color(128, 0, 255, 255);

        public static Color Chocolate = new Color(45, 150, 225, 255);

        public static Color Coral = new Color(0, 128, 175, 255);

        public static Color CornflowerBlue = new Color(155, 106, 18, 255);

        /// <summary>
        /// A yellowish color.
        /// </summary>
        public static Color Cornsilk = new Color(0, 7, 35, 255);

        public static Color Crimson = new Color(35, 235, 195, 255);

        public static Color Cyan = new Color(255, 0, 0, 255);

        public static Color DarkBlue = new Color(255, 255, 116, 255);

        public static Color DarkCyan = new Color(255, 114, 114, 255);

        public static Color DarkGoldenRod = new Color(71, 121, 244, 255);

        public static Color DarkGray = new Color(86, 86, 86, 255);

        public static Color DarkGreen = new Color(255, 155, 255, 255);

        public static Color DarkKhaki = new Color(66, 72, 148, 255);

        public static Color DarkMagenta = new Color(116, 255, 116, 255);

        public static Color DarkOliveGreen = new Color(170, 148, 208, 255);

        public static Color DarkOrange = new Color(0, 115, 255, 255);

        public static Color DarkOrchid = new Color(102, 105, 51, 255);

        public static Color DarkRed = new Color(116, 255, 255, 255);

        public static Color DarkSalmon = new Color(22, 105, 133, 255);

        public static Color DarkSeaGreen = new Color(112, 67, 112, 255);

        public static Color DarkSlateBlue = new Color(183, 194, 116, 255);

        public static Color DarkSlateGray = new Color(208, 176, 176, 255);

        public static Color DarkTurquoise = new Color(255, 49, 46, 255);

        public static Color DarkViolet = new Color(107, 255, 44, 255);

        public static Color DeepPink = new Color(0, 235, 108, 255);

        public static Color DeepSkyBlue = new Color(255, 64, 0, 255);

        public static Color DimGray = new Color(150, 150, 150, 255);

        public static Color DodgerBlue = new Color(225, 111, 0, 255);

        public static Color FireBrick = new Color(77, 221, 221, 255);

        public static Color FloralWhite = new Color(0, 5, 15, 255);

        public static Color ForestGreen = new Color(221, 116, 221, 255);

        public static Color Fuchsia = new Color(0, 255, 0, 255);

        /// <summary>
        /// More of a white-ish color
        /// </summary>
        public static Color Gainsboro = new Color(35, 35, 35, 255);

        public static Color GhostWhite = new Color(7, 7, 0);

        public static Color Gold = new Color(0, 40, 255, 255);

        public static Color GoldenRod = new Color(37, 90, 223, 255);

        public static Color Gray = new Color(127, 127, 127, 255);

        public static Color Green = new Color(255, 127, 255, 255);

        public static Color GreenYellow = new Color(82, 0, 208, 255);

        public static Color HoneyDew = new Color(15, 0, 15, 255);

        public static Color HotPink = new Color(0, 140, 75, 255);

        public static Color IndianRed = new Color(40, 163, 163, 255);

        public static Color Indigo = new Color(180, 255, 125, 255);

        public static Color Ivory = new Color(0, 0, 15, 255);

        public static Color Khaki = new Color(15, 25, 115, 255);

        public static Color Lavender = new Color(25, 25, 5, 255);

        public static Color LavenderBlush = new Color(0, 15, 10, 255);

        public static Color LawnGreen = new Color(131, 3, 255, 255);

        public static Color LemonChiffron = new Color(0, 5, 50, 255);

        public static Color LightBlue = new Color(82, 39, 25, 255);

        public static Color LightCoral = new Color(15, 127, 127, 255);

        public static Color LightCyan = new Color(31, 0, 0, 255);

        public static Color LightGoldenRodYellow = new Color(5, 5, 45, 255);

        public static Color LightGray = new Color(44, 44, 44, 255);

        public static Color LightGreen = new Color(111, 17, 111, 255);

        public static Color LightPink = new Color(0, 73, 62, 255);

        public static Color LightSalmon = new Color(0, 95, 133, 255);

        public static Color LightSeaGreen = new Color(223, 77, 85, 255);

        public static Color LightSkyBlue = new Color(120, 49, 5, 255);

        public static Color LightSlateGray = new Color(146, 119, 102, 255);

        public static Color LightSteelBlue = new Color(79, 59, 33, 255);

        public static Color LightYellow = new Color(0, 0, 31, 255);

        public static Color Lime = new Color(255, 0, 255,255);

        public static Color LimeGreen = new Color(105, 50, 105, 255);

        public static Color Linen = new Color(5, 15, 25, 255);

        public static Color Magenta = new Color(0, 255, 0 ,255);

        public static Color Maroon = new Color(127, 255, 255, 0);

        public static Color MediumAquaMarine = new Color(153, 50, 85, 255);

        public static Color MediumBlue = new Color(255, 255, 50, 255);

        public static Color MediumOrchid = new Color(69, 170, 44, 255);

        public static Color MediumPurple = new Color(108, 143, 36, 255);

        public static Color MediumSeaGreen = new Color(195, 78, 142, 255);

        public static Color MediumSlateBlue = new Color(132, 151, 17, 255);

        public static Color MediumSpringGreen = new Color(255, 5, 101, 255);

        public static Color MediumTurquoise = new Color(183, 46, 51, 255);

        public static Color MediumVioletRed = new Color(46, 234, 122, 255);

        public static Color MidnightBlue = new Color(230, 230, 143, 255);

        public static Color MintCream = new Color(10, 0, 5, 255);

        public static Color MistyRose = new Color(0, 27, 30, 255);

        public static Color Moccasin = new Color(0, 33, 82, 255);

        public static Color NavajoWhite = new Color(0, 33, 82, 255);

        public static Color Navy = new Color(255, 255, 127, 255);

        public static Color OldLace = new Color(2, 10, 25, 255);

        public static Color Olive = new Color(127, 127, 255, 255);

        public static Color OliveDrab = new Color(148, 113, 220, 255);

        public static Color Orange = new Color(0, 90, 255, 255);

        public static Color OrangeRed = new Color(0, 186, 255, 255);

        public static Color Orchid = new Color(37, 143, 41, 255);

        public static Color PaleGoldenRod = new Color(17, 23, 85, 255);

        public static Color PaleGreen = new Color(103, 4, 103, 255);

        public static Color PaleTurquoise = new Color(80, 17, 17, 255);

        public static Color PaleVioletRed = new Color(36, 143, 108, 255);

        public static Color PapayaWhip = new Color(0, 16, 42, 255);

        public static Color PeachPuff = new Color(0, 37, 70, 255);

        public static Color Peru = new Color(50, 122, 192, 255);

        public static Color Pink = new Color(0, 63, 52, 255);

        public static Color Plum = new Color(34, 95, 34, 255);

        public static Color PowderBlue = new Color(79, 31, 25, 255);

        public static Color Purple = new Color(127, 255, 127, 255);

        public static Color RebeccaPurple = new Color(153, 104, 102, 255);

        public static Color Red = new Color(0, 255, 255, 255);

        public static Color RosyBrown = new Color(67, 112, 112, 255);

        public static Color RoyalBlue = new Color(190, 150, 30, 255);

        public static Color SaddleBrown = new Color(115, 186, 231, 255);

        public static Color Salmon = new Color(5, 127, 141, 255);

        public static Color SandyBrown = new Color(11, 91, 159, 255);

        public static Color SeaGreen = new Color(209, 116, 168, 255);

        public static Color SeaShell = new Color(0, 10, 17, 255);

        public static Color Sienna = new Color(95, 173, 210, 255);

        public static Color Silver = new Color(63, 63, 63, 255);

        public static Color SkyBlue = new Color(120, 49, 20, 255);

        public static Color SlateBlue = new Color(149, 165, 50, 255);

        public static Color SlateGray = new Color(143, 127, 111, 255);

        public static Color Snow = new Color(0, 5, 5, 255);

        public static Color SpringGreen = new Color(255, 0, 128, 255);

        public static Color SteelBlue = new Color(185, 125, 75, 255);

        public static Color Tan = new Color(45, 75, 115, 255);

        public static Color Teal = new Color(255, 127, 127, 255);

        public static Color Thistle = new Color(39, 64, 39, 255);

        public static Color Tomato = new Color(0, 156, 184, 255);

        public static Color Turquoise = new Color(191, 31, 47, 255);

        public static Color Violet = new Color(17, 125, 17, 255);

        public static Color Wheat = new Color(10, 33, 76, 255);

        public static Color White = new Color(0, 0, 0, 255);

        public static Color WhiteSmoke = new Color(10, 10, 10, 255);

        public static Color Yellow = new Color(0, 0, 255);

        public static Color YellowGreen = new Color(101, 50, 205, 255);

        //Custom Colors go beneath here


        public static Color randomColor()
        {
            Random r = new Random(Game1.player.money + Game1.tileSize + Game1.dayOfMonth);
            int R = r.Next(0, 255);
            int G = r.Next(0, 255);
            int B = r.Next(0, 255);
            int A = 255;
            return new Color(R, G, B, A);
        }

    }
}
