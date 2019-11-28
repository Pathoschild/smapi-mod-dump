using ConfigureMachineOutputs.Framework.Configs;

namespace ConfigureMachineOutputs.Framework
{
    internal class CmoConfig
    {
        public bool ModEnabled { get; set; } = true;

        public bool SDV_14 { get; set; } = false;
        //public double SMAPIVersion { get; set; } = 3.0;

        public MachineConfig Machines { get; set; } = new MachineConfig();
    }
}
