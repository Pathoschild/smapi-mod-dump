namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class IncubatorConfig
    {
        public bool CustomIncubatorEnabled { get; set; } = true;
        //public int IncubatorInputMultiplier { get; set; } = 1;
        public int IncubatorMinOutput { get; set; } = 1;
        public int IncubatorMaxOutput { get; set; } = 2;
    }
}
