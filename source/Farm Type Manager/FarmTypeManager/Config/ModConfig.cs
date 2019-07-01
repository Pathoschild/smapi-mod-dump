using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //config.json, used by all players/saves for shared functions
        private class ModConfig
        {
            public bool EnableWhereAmICommand { get; set; }
            public bool EnableContentPacks { get; set; } = true; //added in version 1.4; default used here to automatically fill in values with SMAPI's json interface
            public bool EnableContentPackFileChanges { get; set; } = true; //added in version 1.4.2

            public ModConfig()
            {
                EnableWhereAmICommand = true; //enable the "whereami" command in the SMAPI console
                EnableContentPacks = true; //enable any content packs for this mod
                EnableContentPackFileChanges = true; //allow content packs to manipulate files, e.g. reset the main data folder
            }  
        }
    }
}