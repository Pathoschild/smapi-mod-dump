/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AlwaysShowBarValues.Config
{
    internal class ColorSettings
    {
        private static readonly Dictionary<string, Color> colorNames = new()
        {
            {"Black", Color.Black },
            {"Yellow", new Color(255, 190, 0) },
            {"Green", new Color(0, 190, 0) },
            {"Red", new Color(190, 0, 0) },
            {"Blue", new Color(0, 130, 255) }
        };

        private string hexCode = "#000000";
        public string HexCode
        {
            get { return hexCode; }
            set
            {
                hexCode = value;
                UpdateColor();
            }
        }
        public Color Color { get; private set; } = Color.Black;
        private string ChosenColorPreset { get; set; } = "Black";
        public string ColorPreset
        {
            get
            {
                return ChosenColorPreset;
            }
            set
            {
                ChosenColorPreset = value;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (ColorPreset == "Custom") Color = GetColorFromHexCode();
            else Color = GetColorFromName();
        }

        private Color GetColorFromHexCode()
        {
            if (HexCode.Length < 6 || HexCode.Length > 7) return Color.Black;
            try
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(HexCode.StartsWith("#") ? HexCode : "#" + HexCode);
                if (color != System.Drawing.Color.Empty) return new Color(color.R, color.G, color.B);
            }
            catch (ArgumentException)
            {
                return Color.Black;
            }
            return Color.Black;
        }

        private Color GetColorFromName()
        {
            if (!colorNames.ContainsKey(ColorPreset)) return Color.Black;
            return colorNames[ColorPreset];
        }
        internal ColorSettings()
        {
            UpdateColor();
        }
    }
}
