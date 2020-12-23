/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //config.json, used by all players/saves for shared functions
        private class ModConfig
        {
            public bool EnableWhereAmICommand //(setting deprecated/renamed) enable the "whereami" command in the SMAPI console
            {
                set
                {
                    EnableConsoleCommands = value;
                }
            } 
            public bool EnableConsoleCommands { get; set; } = true; //enable this mod's SMAPI console commands
            public bool EnableContentPacks { get; set; } = true; //enable any content packs for this mod
            public bool EnableTraceLogMessages { get; set; } = true; //allow the mod to generate trace-level log messages (which tend to spam the "SMAPI for developers" console)
            public int? MonsterLimitPerLocation { get; set; } = null; //an optional number of monsters allowed per game location; the mod won't spawn more monsters while this many exist

            public ModConfig()
            {
                if (Constants.TargetPlatform == GamePlatform.Android) //if a new config.json is created on an Android device
                {
                    MonsterLimitPerLocation = 50; //use the community-recommended default monster limit
                }
            }  
        }
    }
}