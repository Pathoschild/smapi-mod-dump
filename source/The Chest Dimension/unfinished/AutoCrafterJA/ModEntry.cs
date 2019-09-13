using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Logger;

namespace AutoCrafterJA
{
    class ModEntry : Mod
    {
       
        JAApi JA;
        public override void Entry(IModHelper helper)
        {
            monitor = Monitor;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            JA = Helper.ModRegistry.GetApi<JAApi>("spacechase0.JsonAssets");
            if (JA == null)
            {
                LogError("Could not retrieve the Json Assets API. Please make sure that Json Assets is installed and active.");
                Dispose();
            } else
            {
                JA.LoadAssets(Path.Combine(Helper.DirectoryPath,@"assets\[JA] Crafter"));
            }
        }
    }
}
