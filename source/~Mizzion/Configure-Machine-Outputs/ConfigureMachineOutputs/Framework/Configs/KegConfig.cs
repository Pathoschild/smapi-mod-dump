namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class KegConfig
    {
        public bool CustomKegEnabled { get; set; } = true;
        public int KegInputMultiplier { get; set; } = 1;
        public int KegMinOutput { get; set; } = 1;
        public int KegMaxOutput { get; set; } = 2;
    }
}
