using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of additional requirements needed for a config file to be used on a given farm.</summary>
        private class FileConditions
        {
            public object[] FarmTypes { get; set; } = new object[0]; //a list of farm types on which the config may be used; strings and/or integers are allowed
            public string[] FarmerNames { get; set; } = new string[0]; //a list of farmer names; if the current farmer matches, the config file may be used
            public string[] SaveFileNames { get; set; } = new string[0]; //a list of save file (technically folder) names; if they match the current farm, the config file may be used

            //field below added in version 1.5.0
            public Dictionary<string, bool> OtherMods { get; set; } = new Dictionary<string, bool>(); //a list of mod names + booleans representing whether that mod exists; if the list is accurate, the config file may be used

            //"ResetMainDataFolder" setting removed in version 1.9.3 (obsolete, obstructed players with personal settings)

            public FileConditions()
            {

            }
        }
    }
}