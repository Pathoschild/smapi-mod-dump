namespace SB_PotC
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        public int WitnessBonus { get; set; } = 2;
        public int StorytellerBonus { get; set; } = 4;
        public int UjamaaBonus { get; set; } = 4;
        public int UmojaBonusFestival { get; set; } = 16;
        public int UmojaBonusMarry { get; set; } = 240;
        public int UmojaBonus { get; set; } = 10;
        public int UjimaBonusStore { get; set; } = 20;
        public int UjimaBonus { get; set; } = 2;
        public int KuumbaBonus { get; set; } = 2;
        public bool HasGottenInitialUjimaBonus { get; set; }
        public bool HasGottenInitialKuumbaBonus { get; set; }
    }
}
