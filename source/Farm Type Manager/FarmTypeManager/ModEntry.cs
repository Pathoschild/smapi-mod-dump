/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by other areas of this mod's code
            Utility.Helper = helper; //pass the helper for use by other areas of this mod's code

            Utility.LoadModConfig(); //attempt to load the config.json ModConfig file

            if (Utility.MConfig?.EnableConsoleCommands == true) //if enabled, pass the mod's console command methods to the helper
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                helper.ConsoleCommands.Add("list_monsters", "Outputs a list of available monster types, including custom types loaded by other mods.", ListMonsters);
            }

            AddSMAPIEvents(helper); //pass any necessary event methods to SMAPI
            ApplyHarmonyPatches(); //pass any necessary patches to Harmony
        }
    }
}