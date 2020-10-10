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
    internal class TapperConfig
    {
        public bool CustomTapperEnabled { get; set; } = true;
        //public int TapperInputMultiplier { get; set; } = 1;
        public int TapperMinOutput { get; set; } = 1;
        public int TapperMaxOutput { get; set; } = 2;
    }
}
