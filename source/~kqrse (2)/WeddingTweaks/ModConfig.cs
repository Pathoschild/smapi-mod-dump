/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


namespace WeddingTweaks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int DaysUntilMarriage { get; set; } = 3;
        public bool AllSpousesJoinWeddings { get; set; } = true;
        public bool FixWeddingStart { get; set; } = true;
        public bool AllSpousesWearMarriageClothesAtWeddings { get; set; } = true;
        public bool AllowWitnesses { get; set; } = true;
        public int WitnessMinHearts { get; set; } = 6;
        public bool AllowRandomNPCWitnesses { get; set; } = true;
        public int WitnessAcceptPercent { get; set; } = 50;
        public int WitnessAcceptHeartFactorPercent { get; set; } = 50;

    }
}
