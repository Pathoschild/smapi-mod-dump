namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class LoomConfig
    {
        public bool CustomLoomEnabled { get; set; } = true;
        public int LoomInputMultiplier { get; set; } = 1;
        public int LoomMinOutput { get; set; } = 1;
        public int LoomMaxOutput { get; set; } = 2;
    }
}
