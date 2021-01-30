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
    internal class DeconstructorConfig
    {
        public bool CustomDeconstructorEnabled { get; set; } = true;
        //public int DeconstructorInputMultiplier { get; set; } = 1;
        public int DeconstructorMinOutput { get; set; } = 1;
        public int DeconstructorMaxOutput { get; set; } = 2;
    }
}
