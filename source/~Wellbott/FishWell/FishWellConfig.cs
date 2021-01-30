/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

namespace FishWellSpace
{
    public class FishWellConfig
    {
        public bool InstantConstruction { get; set; } = false;
        public bool FishWellsWorkSlower { get; set; } = true;
        public double SlowSpawnSpeed { get; set; } = 0.65;
        public double SlowProduceSpeed { get; set; } = 0.65;
        public int WellPopulationCap { get; set; } = 10;
    }
}