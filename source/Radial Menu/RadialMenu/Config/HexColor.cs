/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace RadialMenu.Config;

/// <summary>
/// Describes a color used in configuration; compatible with the
/// <see cref="Microsoft.Xna.Framework.Color"/> but serializes as a hex string.
/// </summary>
[JsonConverter(typeof(HexColorConverter))]
public class HexColor
{
    private readonly Color color;

    internal HexColor(Color color)
    {
        this.color = color;
    }

    public static implicit operator Color(HexColor color)
    {
        return color.color;
    }

    public static HexColor Parse(string hexString)
    {
        return TryParse(hexString, out var result)
            ? result
            : throw new FormatException($"Invalid hex color string: {hexString}");
    }

    public static bool TryParse(string hexString, [MaybeNullWhen(false)] out HexColor result)
    {
        result = null;
        hexString = hexString.Trim().TrimStart('#');
        if (hexString.Length == 3)
        {
            // Not an especially elegant way to repeat the characters in a string, but probably the
            // most efficient for running in a possibly-tight deserialization method.
            hexString = new StringBuilder()
                .Append(hexString[0], 2)
                .Append(hexString[1], 2)
                .Append(hexString[2], 2)
                .ToString();
        }
        if (hexString.Length != 6 && hexString.Length != 8)
        {
            return false;
        }
        if (!uint.TryParse(hexString, NumberStyles.HexNumber, null, out var argb))
        {
            return false;
        }
        var (a, r, g, b) = (
            hexString.Length > 6 ? (argb & 0xff000000) >> 24 : 0xff,
            (argb & 0xff0000) >> 16,
            (argb & 0xff00) >> 8,
            argb & 0xff);
        result = new HexColor(new Color((byte)r, (byte)g, (byte)b, (byte)a));
        return true;
    }

    public override string ToString()
    {
        var hexString = new StringBuilder("#");
        if (color.A < 255)
        {
            hexString.Append(color.A.ToString("x2"));
        }
        hexString
            .Append(color.R.ToString("x2"))
            .Append(color.G.ToString("x2"))
            .Append(color.B.ToString("x2"));
        return hexString.ToString();
    }
}

class HexColorConverter : JsonConverter<HexColor>
{
    public override HexColor? ReadJson(
        JsonReader reader,
        Type objectType,
        HexColor? _existingValue,
        bool _hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }
        var hexString = (reader.Value as string);
        return !string.IsNullOrEmpty(hexString) ? HexColor.Parse(hexString) : null;
    }

    public override void WriteJson(JsonWriter writer, HexColor? value, JsonSerializer serializer)
    {
        if (value is not HexColor hexColor)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(hexColor.ToString());
    }
}
