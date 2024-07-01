/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using DeluxeJournal.Framework.Serialization;

namespace DeluxeJournal.Task
{
    [JsonConverter(typeof(ColorSchemaConverter))]
    public class ColorSchema
    {
        /// <summary>Magic Lerping value to lighten the main color to make a vanilla-style padding color.</summary>
        public const float PaddingLightenLerpValue = 0.44f;

        /// <summary>Magic Lerping value to lighten a dark padding color to make a highlighted corner color.</summary>
        public const float CornerLightenLerpValue = 0.34f;

        /// <summary>Magic hue shift value for making a highlight color.</summary>
        public const float HueShift = 3f;

        /// <summary>Magic Luminance threshold value for applying a value (brightness) shift.</summary>
        public const float ValueShiftLumThreshold = 0.694f;

        /// <summary>Magic saturation shift value for making a hover color. (Scaled with Luminance.)</summary>
        public const float HoverMaxSatShift = 0.1739f;

        /// <summary>Magic saturation shift value for making a header color. (Scaled with Luminance.)</summary>
        public const float HeaderMaxSatShift = 0.3188f;

        /// <summary>Error color schema instance.</summary>
        public static readonly ColorSchema ErrorSchema = new(Color.White, Color.LightGray, Color.DarkGray, Color.Black, Color.DarkGray);

        public Color Main { get; set; }

        public Color Hover { get; set; }

        public Color Header { get; set; }

        public Color Accent { get; set; }

        public Color Shadow { get; set; }

        public Color Padding { get; set; }

        public Color Corner { get; set; }

        public ColorSchema(Color main, Color hover, Color header, Color accent, Color shadow, Color? padding = null, Color? corner = null)
        {
            Main = main;
            Hover = hover;
            Header = header;
            Accent = accent;
            Shadow = shadow;
            Padding = padding ?? Color.Lerp(Main, Color.White, PaddingLightenLerpValue);
            Corner = corner ?? (Luminance(Main) > Luminance(Padding) ? Color.Lerp(Padding, Color.White, CornerLightenLerpValue) : Main);
        }

        public ColorSchema(string? main, string? hover, string? header, string? accent, string? shadow, string? padding = null, string? corner = null)
            : this(HexToColor(main) ?? Color.White,
                  HexToColor(hover) ?? Color.White,
                  HexToColor(header) ?? Color.White,
                  HexToColor(accent) ?? Color.White,
                  HexToColor(shadow) ?? Color.White,
                  HexToColor(padding),
                  HexToColor(corner))
        {
        }

        /// <summary>Returns a string representation of this <see cref="ColorSchema"/>.</summary>
        /// <remarks>Does not include alpha channel. Format: <c>{M:[Main] O:[Hover] E:[Header] A:[Accent] S:[Shadow] P:[Padding] C:[Corner]}</c>.</remarks>
        /// <returns>String representation of this <see cref="ColorSchema"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new(64);

            sb.Append("{M:");
            sb.Append(ColorToHex(Main));
            sb.Append(" O:");
            sb.Append(ColorToHex(Hover));
            sb.Append(" E:");
            sb.Append(ColorToHex(Header));
            sb.Append(" A:");
            sb.Append(ColorToHex(Accent));
            sb.Append(" S:");
            sb.Append(ColorToHex(Shadow));
            sb.Append(" P:");
            sb.Append(ColorToHex(Padding));
            sb.Append(" C:");
            sb.Append(ColorToHex(Corner));
            sb.Append('}');

            return sb.ToString();
        }

        /// <summary>Decode a hex string color code into a <see cref="Color"/> instance.</summary>
        /// <param name="encoded">Color hex code. The alpha value can be omitted.</param>
        /// <returns>The <see cref="Color"/> instance or <c>null</c> if <paramref name="encoded"/> could not be parsed.</returns>
        public static Color? HexToColor(string? encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                return null;
            }
            else if (encoded.StartsWith('#'))
            {
                encoded = encoded[1..];
            }

            if (encoded.Length > 8 || !uint.TryParse(encoded, NumberStyles.HexNumber, null, out uint packed))
            {
                return null;
            }

            int numBytes = encoded.Length / 2;

            return new((byte)(packed >> (numBytes - 1) * 8),
                (byte)(packed >> (numBytes - 2) * 8),
                (byte)(packed >> (numBytes - 3) * 8),
                (byte)(numBytes == 4 ? packed : 0xff));
        }

        /// <summary>Encode a <see cref="Color"/> as a hex string.</summary>
        /// <param name="color">The color to be encoded.</param>
        /// <param name="includeAlpha">Include the alpha value as the trailing hex digits.</param>
        /// <returns>The hex color code of the input color or white if the input is <c>null</c>.</returns>
        public static string ColorToHex(Color? color, bool includeAlpha = false)
        {
            string format = includeAlpha ? "X8" : "X6";

            if (color?.PackedValue is not uint packed)
            {
                return 0xFFFFFFFF.ToString(format);
            }

            uint reversed = ((packed & 0xFF0000) >> 16) | (packed & 0xFF00) | ((packed & 0xFF) << 16);

            if (includeAlpha)
            {
                reversed = (reversed << 8) | ((packed & 0xFF000000) >> 24);
            }

            return reversed.ToString(format);
        }

        /// <summary>Produce a <see cref="Color"/> instance from a set of HSV color values.</summary>
        /// <param name="h">Hue component in degrees <c>[0,360)</c>.</param>
        /// <param name="s">Saturation component in the range <c>[0,1]</c>.</param>
        /// <param name="v">Value (brightness) component in the range <c>[0,1]</c>.</param>
        /// <returns>The <see cref="Color"/> instance with corresponding HSV values encoded as sRGB.</returns>
        public static Color HSVToColor(float h, float s, float v)
        {
            float r, g, b;

            if (s <= 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int sector = (int)h / 60;
                float fraction = h / 60f - sector;

                // Calculate color components t <= u1|u2 <= v
                float t = v * (1f - s);
                float u1 = v * (1f - (s * fraction));
                float u2 = v * (1f - (s * (1f - fraction)));

                switch (sector)
                {
                    case 1:
                        r = u1;
                        g = v;
                        b = t;
                        break;
                    case 2:
                        r = t;
                        g = v;
                        b = u2;
                        break;
                    case 3:
                        r = t;
                        g = u1;
                        b = v;
                        break;
                    case 4:
                        r = u2;
                        g = t;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = t;
                        b = u1;
                        break;
                    default:
                        r = v;
                        g = u2;
                        b = t;
                        break;
                }
            }

            return new Color(r, g, b);
        }

        /// <summary>Convert an sRGB <see cref="Color"/> to a set of HSV color values.</summary>
        /// <remarks>The HSV values are in the standard ranges of <c>{H, S, V} = {[0,360), [0,1], [0,1]}</c>.</remarks>
        /// <param name="color">The color to be converted.</param>
        /// <param name="h">Hue component in degrees <c>[0,360)</c>.</param>
        /// <param name="s">Saturation component in the range <c>[0,1]</c>.</param>
        /// <param name="v">Value (brightness) component in the range <c>[0,1]</c>.</param>
        public static void ColorToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            float chroma = max - min;

            s = max <= 0 ? 0 : chroma / max;
            v = max;

            if (chroma <= 0)
            {
                h = 0;
                return;
            }
            else if (max == r)
            {
                h = (g - b) / chroma;

                if (b > g)
                {
                    h = ((h % 6) + 6) % 6;
                }
            }
            else if (max == g)
            {
                h = (b - r) / chroma + 2;
            }
            else
            {
                h = (r - g) / chroma + 4;
            }

            h *= 60f;
        }

        /// <summary>Shift the values of a <see cref="Color"/> using HSV color value offsets.</summary>
        /// <param name="color">The target color.</param>
        /// <param name="dh">Delta hue offset.</param>
        /// <param name="ds">Delta saturation offset.</param>
        /// <param name="dv">Delta value (brightness) offset.</param>
        /// <param name="hueTarget">Hue value that the <paramref name="dh"/> value should shift towards, or a negative value to unconditionally add <paramref name="dh"/>.</param>
        /// <returns>The <see cref="Color"/> whose values have been shifted by the HSV color offsets.</returns>
        public static Color HSVShiftColor(Color color, float dh, float ds, float dv, float hueTarget = -1f)
        {
            ColorToHSV(color, out float h, out float s, out float v);

            if (hueTarget >= 0 && (h > hueTarget ? h - hueTarget < 180f : hueTarget - h > 180f))
            {
                dh = -dh;
            }

            return HSVToColor((((h + dh) % 360f) + 360f) % 360f, Math.Clamp(s + ds, 0f, 1f), Math.Clamp(v + dv, 0f, 1f));
        }

        /// <summary>Get the Luminance value for a <see cref="Color"/> using the ITU BT.709 standard coefficients.</summary>
        /// <param name="color">The target color.</param>
        /// <returns>The Luminance value within the range <c>[0,1]</c>.</returns>
        public static float Luminance(Color color)
        {
            return Linearize(color.R) * 0.2126f + Linearize(color.G) * 0.7152f + Linearize(color.B) * 0.0722f;

            // Linearize gamma encoded sRGB value.
            static float Linearize(byte value)
            {
                float norm = value / 255f;

                if (norm < 0.04045f)
                {
                    return norm / 12.92f;
                }
                else
                {
                    return (float)Math.Pow((norm + 0.055d) / 1.055d, 2.4d);
                }
            }
        }

        /// <summary>Extract a color schema profile from a sprite texture.</summary>
        /// <remarks>The sprite must have the same general structure as the vanilla box and must be composed of 9 equal sized sections.</remarks>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="source">The bounds within the texture containing the texture box.</param>
        /// <param name="border">The border color of the texture box.</param>
        /// <returns>A <see cref="ColorSchema"/> instance with the same color palette as the source texture box.</returns>
        public static ColorSchema ExtractFromTextureBox(Texture2D texture, Rectangle source, out Color border)
        {
            int width = source.Width;
            int section = width / 3;
            int sectionMidpoint = (int)Math.Ceiling(section / 2d);
            Color[] pixels = new Color[width * source.Height];

            texture.GetData(0, source, pixels, 0, pixels.Length);

            Color main = pixels[width * section + section];
            Color accent = pixels[width * section + section / 2];
            border = pixels[1];

            ColorToHSV(accent, out float targetHue, out float _, out float _);
            ColorToHSV(main, out float _, out float _, out float mainValue);
            float luminance = Luminance(main);
            float satBoost = 1f - mainValue;
            float valueShift = Math.Min(0, ValueShiftLumThreshold - luminance);

            return new ColorSchema(
                main,
                HSVShiftColor(main, HueShift, HoverMaxSatShift * luminance + satBoost, valueShift, targetHue),
                HSVShiftColor(main, HueShift, HeaderMaxSatShift * luminance + satBoost, valueShift, targetHue),
                accent,
                pixels[width * sectionMidpoint + sectionMidpoint],
                pixels[width + 2],
                pixels[width + 1]);
        }
    }
}
