namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class TapperConfig
    {
        public bool CustomTapperEnabled { get; set; } = true;
        //public int TapperInputMultiplier { get; set; } = 1;
        public int TapperMinOutput { get; set; } = 1;
        public int TapperMaxOutput { get; set; } = 2;
    }
}
