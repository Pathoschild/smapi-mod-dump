namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class LightningRodConfig
    {
        public bool CustomLightningRodEnabled { get; set; } = true;
        //public int LightningRodInputMultiplier { get; set; } = 1;
        public int LightningRodMinOutput { get; set; } = 1;
        public int LightningRodMaxOutput { get; set; } = 2;
    }
}
