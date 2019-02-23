namespace ConfigureMachineSpeed
{
    internal class ModConfig
    {
        public uint UpdateInterval { get; set; } = 10;
        public MachineConfig[] Machines { get; set; } = new MachineConfig[0];
    }

    internal class MachineConfig
    {
        public string Name = "";
        public int Minutes;
    }
}
