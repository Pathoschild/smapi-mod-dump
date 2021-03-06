/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

// Class for custom map tooltips
using Newtonsoft.Json;

public class MapTooltip
{
  public int X { get; set; } // Absolute position relative to map
  public int Y { get; set; } // Absolute position relative to map
  public int Width { get; set; } // Width of area on map
  public int Height { get; set; } // Height of area on map
  public string PrimaryText { get; set; } // Primary text
  public string SecondaryText { get; set; } // Secondary text (second line)

  public MapTooltip(int x, int y, int width, int height, string primaryText)
  {
    X = x;
    Y = y;
    Width = width;
    Height = height;
    PrimaryText = primaryText;
    SecondaryText = "";
  }

  [JsonConstructor]
  public MapTooltip(int x, int y, int width, int height, string primaryText, string secondaryText)
  {
    X = x;
    Y = y;
    Width = width;
    Height = height;
    PrimaryText = primaryText;
    SecondaryText = secondaryText;
  }
}