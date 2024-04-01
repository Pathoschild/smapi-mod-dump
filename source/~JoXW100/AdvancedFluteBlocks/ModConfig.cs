/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace AdvancedFluteBlocks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton ToneModKey { get; set; } = SButton.LeftAlt;
        public SButton PitchModKey { get; set; } = SButton.LeftShift;
        public string CurrentTone { get; set; } = "flute";
        public int PitchStep { get; set; } = 100;
        public int CurrentPitch { get; set; } = 0;
        public string ToneList { get; set; } = "flute,toyPiano,crystal,clam_tone,toolCharge";
    }
}
