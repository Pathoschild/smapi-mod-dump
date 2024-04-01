/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace ItemExtensions.Models.Contained;

public class LightData
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    public float Transparency { get; set; } = 0.9f;
    public string Hex { get; set; } = null;
    public float Size { get; set; } = 1.2f;

    public Color GetColor()
    {
        if (!string.IsNullOrWhiteSpace(Hex))
        {
            var color = Utility.StringToColor(Hex) ?? Color.White;
            return color * Transparency;
        }
        
        return new Color(R, G, B) * Transparency;
    }

    public string ColorString()
    {
        if (!string.IsNullOrWhiteSpace(Hex))
        {
            return Hex;
        }
        
        return $"{R} {G} {B}";
    }
}