/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

namespace HeartEventHelper
{
    public class ModConfig
    {
        public string beforePositive { get; set; } = "{heart}|";
        public string beforeNeutral { get; set; } = "*|";
        public string beforeNegative { get; set; } = "{3}|";
        public string afterPositive { get; set; } = " ({#_abs})";
        public string afterNeutral { get; set; } = "";
        public string afterNegative { get; set; } = " ({#})";
        
        // public bool neutralDisable { get; set; } = false;
    }
}