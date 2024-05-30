/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AlwaysShowBarValues.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using xTile;

namespace AlwaysShowBarValues.UIElements
{
    public class PlayerStat
    {
        /// <summary>The stat's name</summary>
        public string StatName { get; set; }
        /// <summary>The texture the stat's icon is in</summary>
        public Texture2D IconSourceTexture = Game1.mouseCursors;
        /// <summary>The icon's position in Cursors</summary>
        public Rectangle IconSourceRectangle;
        /// <summary>The icon might need to be resized</summary>
        public float IconScale;
        /// <summary>The stat's current value</summary>
        private float currentValue;
        public float CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                StatusString = (int)value + "/" + (int)maxValue;
            }
        }
        /// <summary>The stat's maximum value</summary>
        private float maxValue;
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                StatusString = (int)currentValue+ "/" + (int)value;
            }
        }
        /// <summary>The string that'll show up in the box</summary>
        public string StatusString = "";
        /// <summary>The offset from its usual position</summary>
        public Vector2 Offset;

        /// <summary>Current colors according to user-chosen color mode and hex codes</summary>
        internal Dictionary<string, ColorSettings> Colors = new()
        {
            { "max", new ColorSettings() },
            { "middle", new ColorSettings() },
            { "min", new ColorSettings() }
        };

        internal Vector2 MaxStringSize {
            get { return this.GetMaxStringSize(); }
            set { }
        }

        /// <summary>Preset color mode chosen by the user</summary>
        private string ChosenColorMode { get; set; } = "Green/Yellow/Red";
        public string ColorMode
        {
            get { return ChosenColorMode; }
            set
            {
                ChosenColorMode = value;
                string[] colorNames = ChosenColorMode.Split('/');
                Colors["max"].ColorPreset = colorNames[0];
                Colors["middle"].ColorPreset = colorNames.Length > 1 ? colorNames[1] : colorNames[0];
                Colors["min"].ColorPreset = colorNames.Length > 1 ? colorNames[2] : colorNames[0];
            }
        }

        public PlayerStat(string StatName, Rectangle texturePosition, Vector2 offset)
        {
            this.StatName = StatName;
            IconSourceRectangle = texturePosition;
            IconScale = 16f / texturePosition.Width;
            this.Offset = offset;
        }

        public void OnHUDUpdate(float currentValue, float maxValue)
        {
            StatusString = new(currentValue + "/" + maxValue);
        }

        private static Color CalculateCurrentColor(float ratio, Color lowestColor, Color highestColor)
        {
            int red = (int)(lowestColor.R + (highestColor.R - lowestColor.R) * ratio);
            int green = (int)(lowestColor.G + (highestColor.G - lowestColor.G) * ratio);
            int blue = (int)(lowestColor.B + (highestColor.B - lowestColor.B) * ratio);
            return new Color(red, green, blue);
        }

        public Color GetTextColor()
        {
            float barFullness = CurrentValue / MaxValue;
            if (barFullness > 0.5)
            {
                float ratio = 2 * barFullness - 1;
                return CalculateCurrentColor(ratio, Colors["middle"].Color, Colors["max"].Color);
            }
            else
            {
                float ratio = 2 * barFullness;
                return CalculateCurrentColor(ratio, Colors["min"].Color, Colors["middle"].Color);
            }
        }

        private Vector2 GetMaxStringSize()
        {
            // I swear to God this is faster than a prettier code
            string biggestValue = this.maxValue > 999 ? "-9999/9999" : this.maxValue > 99 ? "-999/999" : "99/99";
            return Game1.smallFont.MeasureString(biggestValue);
        }

    }
}
