namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class BeeHouseConfig
    {
        public bool CustomBeeHouseEnabled { get; set; } = true;
        //public int BeeHouseInputMultiplier { get; set; } = 1;
        public int BeeHouseMinOutput { get; set; } = 1;
        public int BeeHouseMaxOutput { get; set; } = 2;
    }
}
