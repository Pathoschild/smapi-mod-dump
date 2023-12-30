/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jwaad/Stardew_Player_Arrows
**
*************************************************/

public sealed class ModConfig
{
    public bool Enabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    public bool NamesOnArrows { get; set; } = true;
    public bool DrawBorders { get; set; } = true; 
    public int PlayerLocationUpdateFPS { get; set; } = 40;
    public int ArrowOpacity { get; set; } = 90;
    public string ColourPalette { get; set; } = "All";
}