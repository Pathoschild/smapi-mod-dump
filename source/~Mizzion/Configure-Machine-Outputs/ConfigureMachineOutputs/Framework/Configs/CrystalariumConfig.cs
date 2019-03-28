namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class CrystalariumConfig
    {
        public bool CustomCrystalariumEnabled { get; set; } = true;
        public int CrystalInputMultiplier { get; set; } = 1;
        public int CrystalMinOutput { get; set; } = 1;
        public int CystalMaxOutput { get; set; } = 2;
    }
}
