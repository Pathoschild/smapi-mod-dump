/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


namespace HugsAndKisses
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        public bool RoommateKisses { get; set; } = false;
        public int MinHeartsForMarriageKiss { get; set; } = 9;
        public int MinHeartsForDatingKiss { get; set; } = 6;
        public int HeartsForFriendship { get; set; } = 5;
        public bool UnlimitedDailyKisses { get; set; } = true;
        public bool DatingKisses { get; set; } = true;
        public bool FriendHugs { get; set; } = true;
        public float SpouseKissChance0to1 { get; set; } = 0.1f;
        public string CustomKissSound { get; set; } = "kiss.wav";
        public string CustomHugSound { get; set; } = "";
        public string CustomKissFrames { get; set; } = "Sam:36,Penny:35";
        public float MaxDistanceToKiss { get; set; } = 200f;
        public double MinSpouseKissIntervalSeconds { get; set; } = 10;
        public bool AllowPlayerSpousesToKiss { get; set; } = true;
        public bool AllowNPCRelativesToHug { get; set; } = true;
        public bool AllowNPCSpousesToKiss { get; set; } = true;
        public bool AllowRelativesToKiss { get; set; } = false;
        public bool AllowNonDateableNPCsToHugAndKiss { get; set; } = false;
        public bool UseNonDateableNPCsKissFrames { get; set; } = false;
    }
}
