namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class MayoConfig
    {
        public bool CustomMayoEnabled { get; set; } = true;
        public int MayoInputMultiplier { get; set; } = 1;
        public int MayoMinOutput { get; set; } = 1;
        public int MayoMaxOutput { get; set; } = 2;
    }
}
