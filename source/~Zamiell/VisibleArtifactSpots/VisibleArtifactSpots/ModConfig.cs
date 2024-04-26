/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace VisibleArtifactSpots
{
    public sealed class ModConfig
    {
        public string HighlightType { get; set; } = "Border";
        public bool HighlightArtifactSpots { get; set; } = true;
        public bool HighlightSeedSpots { get; set; } = true;
        public bool HighlightWeeds { get; set; } = false;
        public bool HighlightTwigs { get; set; } = false;
        public bool HighlightStones { get; set; } = false;
        public bool HighlightCopperNodes { get; set; } = false;
        public bool HighlightIronNodes { get; set; } = false;
        public bool HighlightGoldNodes { get; set; } = false;
        public bool HighlightIridiumNodes { get; set; } = false;
        public bool HighlightRadioactiveNodes { get; set; } = false;
        public bool HighlightCinderNodes { get; set; } = false;
        public bool HighlightChests { get; set; } = false;
        public bool HighlightNonPlanted { get; set; } = false;
        public bool HighlightNonWatered { get; set; } = false;
        public bool HighlightNonFertilized { get; set; } = false;
        public bool HighlightHoeableTile { get; set; } = false;
    }
}
