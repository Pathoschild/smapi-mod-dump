/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Dynamic;
using StardewValley;
using StardewValley.GameData.Machines;

namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class CmoConfig
    {
        public bool ModEnabled { get; set; } = true;

        //public MachineConfig Machines { get; set; } = new MachineConfig();
        public MachineConfig MachineData { get; set; } = new MachineConfig();
    }
}
