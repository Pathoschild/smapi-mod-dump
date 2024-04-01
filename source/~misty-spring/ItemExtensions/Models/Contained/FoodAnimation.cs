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

namespace ItemExtensions.Models.Contained;

public class FoodAnimation
{
    //advanced configuration
    public string CustomTexture { get; set; } = null;
    public int Frames { get; set; } = 1;
    public int Loops { get; set; } = 0;
    public int Delay { get; set; }
    public float Scale { get; set; } = 1f;
    public LightData Light { get; set; } = null;
    
    //general settings
    public float Duration { get; set; } = 1270f; //254f
    public bool Crunch { get; set; } = true;
    
    //coloring
    public string Color { get; set; } = null;
    public float Transparency { get; set; } = 1f;
    
    //position
    public float Rotation { get; set; } = 0f;
    public Vector2 Offset { get; set; } = new(-21f, -112f);
    public bool Flip { get; set; } = false;

    //sound
    public string StartSound { get; set; }
    public string EndSound { get; set; }
    
    //item movement
    public Vector2 Motion { get; set; } = new(x: 0.8f, y: -11f);
    public Vector2 Speed { get; set; } = new(x: 0.0f, y: 0.5f);
    public int StopX { get; set; }
    public int StopY { get; set; }
}