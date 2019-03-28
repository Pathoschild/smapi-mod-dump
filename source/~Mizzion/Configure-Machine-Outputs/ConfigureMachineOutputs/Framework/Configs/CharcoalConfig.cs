namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class CharcoalConfig
    {
        public bool CustomCharcoalEnabled { get; set; } = true;
        public int CharcoalInputMultiplier { get; set; } = 1;
        public int CharcoalMinOutput { get; set; } = 1;
        public int CharcoalMaxOutput { get; set; } = 2;
    }
}
