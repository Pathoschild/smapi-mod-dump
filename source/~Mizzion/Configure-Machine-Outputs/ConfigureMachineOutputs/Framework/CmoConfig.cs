using ConfigureMachineOutputs.Framework.Configs;

namespace ConfigureMachineOutputs.Framework
{
    internal class CmoConfig
    {
        public bool ModEnabled { get; set; } = true;

        public MachineConfig Machines { get; set; } = new MachineConfig();
    }
}
