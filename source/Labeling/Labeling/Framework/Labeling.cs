/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/Labeling
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Labeling.Framework;

public class Labeling
{
    public Labeling(string name, Vector2 firstObjectTile, Vector2 secondObjectTile, string currentGameLocation,
        bool display, Color color)
    {
        Name = name;
        FirstObjectTile = firstObjectTile;
        SecondObjectTile = secondObjectTile;
        CurrentGameLocation = currentGameLocation;
        Display = display;
        Color = color;
    }

    public string Name { get; set; }
    public Vector2 FirstObjectTile { get; set; }
    public Vector2 SecondObjectTile { get; set; }

    public string CurrentGameLocation { get; set; }
    public bool Display { get; set; }

    public Color Color { get; set; }
}