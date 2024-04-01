/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace ConfigureMachineOutputs.Framework.Configs.Machines
{
    internal class SeedMakerConfig
    {
        public bool Enabled { get; set; } = true;

        private static string Id = "(BC)25";

        public bool MoreSeedsForQuality { get; set; } = true;
        public int InputMultiplier { get; set; } = 1;

        internal string QualityId { get; } = Id;
        public int MinOutput { get; set; } = 2;
        public int MaxOutput { get; set; } = 5;
    }
}
