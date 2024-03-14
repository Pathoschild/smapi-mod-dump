/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace LessFlashy;

public class ModConfig
{
    public string Animations { get; set; } = "NoFlash";
    public string FishingBubbles { get; set; } = "NoFlash";
    public bool Forge { get; set; } = true;
    public bool Lava { get; set; } = true;
    public float Rain { get; set; } = 1.0f;
    public bool Sewing { get; set; }
    public bool SoftLight { get; set; }
    public bool Water { get; set; }
    public bool Bugs { get; set; }
    public bool Magma { get; set; }
    public bool Projectiles { get; set; } = true;
    public bool SlimeBall { get; set; }
    public bool Serpents { get; set; } = true;
}