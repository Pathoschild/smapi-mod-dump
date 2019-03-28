namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class PreservesJarConfig
    {
        public bool CustomPreservesJarEnabled { get; set; } = true;
        public int PreserveInputMultiplier { get; set; } = 1;
        public int PreserveMinOutput { get; set; } = 1;
        public int PreserveMaxOutput { get; set; } = 2;
    }
}
