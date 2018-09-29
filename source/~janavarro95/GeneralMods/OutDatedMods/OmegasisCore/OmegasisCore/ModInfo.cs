using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore
{
   public class ModInfo
    {
        public static string ModName;
        public static string ModVersion;
        public static string SMAPIVersion;

        public string MajorVersion;
        public string MinorVersion;
        public string BuildVersion;

        public ModInfo(string modName, string modVersion,string apiVersion)
        {
            ModName = modName;
            ModVersion = modVersion;
            SMAPIVersion = apiVersion;

            checkAPIVersion();
        }

        /// <summary>
        /// Used to determine what verion of the mod I am currently 
        /// </summary>
         private void checkAPIVersion()
        {
            ModCore.MONITOR.Log("Running Mod Version: "+ModVersion, StardewModdingAPI.LogLevel.Info);

            if (SMAPIVersion != StardewModdingAPI.Constants.ApiVersion.ToString())
            {
                ModCore.MONITOR.Log("Mod SMAPI Version: " + SMAPIVersion, StardewModdingAPI.LogLevel.Error);
                ModCore.MONITOR.Log("This mod is built for an older version of SMAPI and is currently outdated!\nThis mod is not expected to work at all so you should contact me to let me know so I can update it.\nLeave a message in one of these places:\n*Note that there is an order of preference. Higher on the list means I'll be more likely to see the message.\n", StardewModdingAPI.LogLevel.Warn);
                ModCore.MONITOR.Log("Github:https://github.com/janavarro95/Stardew_Valley_Mods");
                ModCore.MONITOR.Log("Nexus Forums:https://forums.nexusmods.com/index.php?/user/32171640-omegasis/");
                ModCore.MONITOR.Log("Discord PM: @Omegasis");
                ModCore.MONITOR.Log("Stardew Valley Forums:http://community.playstarbound.com/conversations/add \nWhere participant is zipy199522. (It's and old username)");
                ModCore.MONITOR.Log("Email: janavarro95@gmail.com");
            }
            else Logger.GoodMessage("Running Console Version: " + SMAPIVersion);
        }

    }
}
