using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            AddSMAPIEvents(helper); //pass any necessary event methods to SMAPI

            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by other areas of this mod's code

            Utility.LoadModConfig(helper); //attempt to load the config.json ModConfig file

            Utility.Helper = helper; //pass the helper for use by other areas of this mod's code

            if (Utility.MConfig?.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
            }
        }
    }
}