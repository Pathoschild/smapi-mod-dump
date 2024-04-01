/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Models.Contained;

public class FarmerFrame
{
    public int Frame { get; set; } = 0;
    public int Duration { get; set; } = 200;
    public bool SecondaryArm { get; set; } = false;
    public bool Flip { get; set; } = false;
    public bool HideArm { get; set; } = false;
}