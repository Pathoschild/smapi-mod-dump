using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace OmegasisCore
{
    public class ModCore: Mod
    {
        public static IMonitor MONITOR;
        public static ModInfo modInfo;

        public override void Entry(IModHelper helper)
        {
            MONITOR=this.Monitor;
            //  Monitor.Log("IDK", LogLevel.Debug);
            modInfo = new ModInfo("OmegasisCore","1.0.0", "1.15.1");
        }
    }
}
