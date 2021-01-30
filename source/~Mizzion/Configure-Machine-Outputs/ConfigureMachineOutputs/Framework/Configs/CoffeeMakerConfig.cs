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
    internal class CoffeeMakerConfig
    {
        public bool CustomCoffeeMakerEnabled { get; set; } = true;
        //public int CoffeeMakerInputMultiplier { get; set; } = 1;
        public int CoffeeMakerMinOutput { get; set; } = 1;
        public int CoffeeMakerMaxOutput { get; set; } = 2;
    }
}
