/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SeedMachines.Framework;
using StardewValley.Network;
using StardewValley.Menus;
using StardewValley.Locations;
using System.Diagnostics;

namespace SeedMachines
{
    class ModEntry : Mod
    {
        internal static IModHelper modHelper;
        internal static IMonitor monitor;
        internal static DataLoader dataLoader;
        internal static ModSettings settings;

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            monitor = this.Monitor;

            readSettings();
            writeSettings();

            Commands.addCommands();

            modHelper.Events.GameLoop.GameLaunched += EventHandlers.OnGameLaunched;
            modHelper.Events.GameLoop.DayStarted += EventHandlers.OnDayStarted;
            modHelper.Events.GameLoop.DayEnding += EventHandlers.OnDayEnding;
            modHelper.Events.Player.Warped += EventHandlers.OnWarped;
            modHelper.Events.World.ObjectListChanged += EventHandlers.OnObjectListChanged;
            modHelper.Events.Input.ButtonPressed += EventHandlers.OnButtonPressed;
        }

        public static void readSettings()
        {
            settings = modHelper.Data.ReadJsonFile<ModSettings>("settings.json") ?? new ModSettings();
        }

        public static void writeSettings()
        {
            modHelper.Data.WriteJsonFile("settings.json", settings);
        }

        public static void debug(String debugLine)
        {
            monitor.Log(debugLine, LogLevel.Debug);
        }
    }
}
