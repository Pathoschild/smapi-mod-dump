using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //config.json, used by all players/saves for shared functions
        private class ModConfig
        {
            public bool EnableWhereAmICommand { get; set; }
            public bool EnableContentPacks { get; set; } = true;
            public bool EnableContentPackFileChanges { get; set; } = true;
            public bool EnableTraceLogMessages { get; set; } = true;

            public ModConfig()
            {
                EnableWhereAmICommand = true; //enable the "whereami" command in the SMAPI console
                EnableContentPacks = true; //enable any content packs for this mod
                EnableContentPackFileChanges = true; //allow content packs to manipulate files, e.g. reset the main data folder
                EnableTraceLogMessages = true; //allow the mod to generate trace-level log messages (which tend to spam the "SMAPI for developers" console)
            }  
        }
    }
}