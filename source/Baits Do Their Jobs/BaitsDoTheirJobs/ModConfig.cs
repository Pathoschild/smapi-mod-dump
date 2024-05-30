/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/BaitsDoTheirJobs
**
*************************************************/

namespace BaitsDoTheirJobs;

public sealed class ModConfig
{
    public bool MagnetBaitEnabled { get; set; } = true;
    public bool ForceTreasure { get; set; } = false;
    public bool WildBaitEnabled { get; set; } = true;
    public bool ForceDoubleFish { get; set; } = false;
    public bool TargetedBaitEnabled { get; set; } = true;
    public bool ForceTargetedFish { get; set; } = false;
}