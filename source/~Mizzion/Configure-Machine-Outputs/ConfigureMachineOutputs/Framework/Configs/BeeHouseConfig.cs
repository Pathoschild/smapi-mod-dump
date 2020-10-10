/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

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
