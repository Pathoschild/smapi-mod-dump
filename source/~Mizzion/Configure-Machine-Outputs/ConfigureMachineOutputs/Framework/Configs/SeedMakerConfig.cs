/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class SeedMakerConfig
    {
        public bool CustomSeedMakerEnabled { get; set; } = true;
        public bool MoreSeedsForQuality { get; set; } = true;
        public int SeedMakerInputMultiplier { get; set; } = 1;
        public int SeedMakerMinOutput { get; set; } = 2;
        public int SeedMakerMaxOutput { get; set; } = 5;
    }
}
