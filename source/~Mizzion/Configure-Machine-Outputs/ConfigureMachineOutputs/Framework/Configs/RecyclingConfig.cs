namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class RecyclingConfig
    {
        public bool CustomRecyclingEnabled { get; set; } = true;
        public int CustomRecyclingInputMultiplier { get; set; } = 1;
        public bool ReplaceStoneWithOreEnabled { get; set; } = false;
        public int StoneMinOutput { get; set; } = 1;
        public int StoneMaxOutput { get; set; } = 4;
        public double StoneToCopperChance { get; set; } = 0.5;
        public double StoneToIronChance { get; set; } = 0.3;
        public double StoneToGoldChance { get; set; } = 0.19;
        public double StoneToIridiumChance { get; set; } = 0.01;

        public bool ReplaceOreOutput { get; set; } = false;
        public int CopperMinOutput { get; set; } = 1;
        public int CopperMaxOutput { get; set; } = 2;
        public int IronMinOutput { get; set; } = 1;
        public int IronMaxOutput { get; set; } = 2;
        public int GoldMinOutput { get; set; } = 1;
        public int GoldMaxOutput { get; set; } = 2;
        public int IridiumMinOutput { get; set; } = 1;
        public int IridiumMaxOutput { get; set; } = 2;
        public int WoodMinOutput { get; set; } = 1;
        public int WoodMaxOutput { get; set; } = 2;
        public int RefinedQuartzMinOutput { get; set; } = 1;
        public int RefinedQuartzMaxOutput { get; set; } = 2;
        public int CoalMinOutput { get; set; } = 1;
        public int CoalMaxOutput { get; set; } = 2;
        public int ClothMinOutput { get; set; } = 1;
        public int ClothMaxOutput { get; set; } = 2;

        public int TorchMinOutput { get; set; } = 1;
        public int TorchMaxOutput { get; set; } = 2;

    }
}
