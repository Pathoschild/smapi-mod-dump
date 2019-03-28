namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class CheesePressConfig
    {
        public bool CustomCheesePressEnabled { get; set; } = true;
        public int CheesePressInputMultiplier { get; set; } = 1;
        public int CheesePressMinOutput { get; set; } = 1;
        public int CheesePressMaxOutput { get; set; } = 2;
    }
}
