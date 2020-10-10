/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

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
