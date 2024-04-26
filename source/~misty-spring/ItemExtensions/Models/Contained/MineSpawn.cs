/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Enums;

namespace ItemExtensions.Models.Contained;

public class MineSpawn
{
    public MineSpawn()
    {
    }

    public MineSpawn(IEnumerable<string> floors, double spawnFrequency, double additionalChancePerLevel, bool main)
    {
        Type = main ? MineType.General : MineType.All;
        RealFloors = floors as List<string>;
        SpawnFrequency = spawnFrequency;
        AdditionalChancePerLevel = additionalChancePerLevel;
    }

    public string Floors { get; set; } = null;
    public MineType Type { get; set; } = MineType.All;
    public string Condition { get; set; } = null;
    internal List<string> RealFloors { get; set; } = new();
    public double SpawnFrequency { get; set; } = 0.1;
    public double AdditionalChancePerLevel { get; set; }
    internal int LastFrenzy { get; set; } = -1;

    public void Parse(IEnumerable<string> floors)
    {
        RealFloors = floors as List<string>;
    }
}
