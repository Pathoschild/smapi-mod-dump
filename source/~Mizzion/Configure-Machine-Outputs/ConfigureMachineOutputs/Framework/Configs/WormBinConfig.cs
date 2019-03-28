namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class WormBinConfig
    {
        public bool CustomWormBinEnabled { get; set; } = true;
        //public int WormBinInputMultiplier { get; set; } = 1;
        public int WormBinMinOutput { get; set; } = 1;
        public int WormBinMaxOutput { get; set; } = 2;
    }
}
