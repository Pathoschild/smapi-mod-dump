/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace EscasModdingPlugins
{
    /// <summary>Adds a SMAPI console command that opens a special orders board for a specified order type.</summary>
    public static class Command_CustomBoard
    {
        /// <summary>True if these commands are currently enabled.</summary>
		public static bool Enabled { get; private set; } = false;
        /// <summary>The helper instance to use for API access.</summary>
        private static IModHelper Helper { get; set; } = null;
        /// <summary>The monitor instance to use for console/log messages.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Initializes this class's SMAPI console commands.</summary>
        /// <param name="helper">The helper instance to use for API access.</param>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        public static void Enable(IModHelper helper, IMonitor monitor)
        {
            if (Enabled)
                return; //do nothing

            //store args
            Helper = helper;
            Monitor = monitor;

            //initialize commands
            helper.ConsoleCommands.Add("EMP", "Runs an EMP command. Type \"EMP\" or \"EMP help\" for details.", CustomBoard);
        }

        /// <summary>Opens a special orders board for a custom order type.</summary>
        /// <param name="command">The console command used when calling this method (e.g. "EMP").</param>
        /// <param name="args">The arguments provided after the console command (e.g. "CustomBoard" "MyCustomOrders").</param>
        private static void CustomBoard(string command, string[] args)
        {
            if (args == null || args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase)) //args are null, blank, or "help"
            {
                string helpText = @"""EMP"" can be used with these commands:

EMP CustomBoard
    Opens a ""special orders board"" menu for a custom order type.

    Usage:   EMP CustomBoard <OrderType>
    Example: EMP CustomBoard Esca.EMP/MyCustomOrders

    - OrderType: The category of special orders to display. Case-sensitive.
        ""Esca.EMP/"" will be added automatically if you don't include it.
        See the ""OrderType"" field in the ""Data/SpecialOrders"" asset.
";

                Monitor.Log(helpText, LogLevel.Info);
                return;
            }

            if (args[0].Equals("CustomBoard", StringComparison.OrdinalIgnoreCase)) //if arg #1 is "CustomBoard"
            {
                if (!Context.IsPlayerFree)
                {
                    Monitor.Log($"Cannot currently open a menu. Please load a game, close other menus, or make sure your character is not busy.", LogLevel.Info);
                    return;
                }

                if (args.Length > 1) //if arg #2 exists
                {
                    string orderType = args[1];

                    if (!orderType.StartsWith(ModEntry.PropertyPrefix, StringComparison.OrdinalIgnoreCase)) //if the order type does NOT start with "Esca.EMP/"
                        orderType = ModEntry.PropertyPrefix + orderType; //add that prefix

                    Monitor.Log($"Opening special orders board menu for order type \"{orderType}\".", LogLevel.Info);
                    Game1.activeClickableMenu = new SpecialOrdersBoard(orderType);
                    return;
                }
                else
                {
                    Monitor.Log($"No order type provided. Opening the default special orders board menu.", LogLevel.Info);
                    Game1.activeClickableMenu = new SpecialOrdersBoard();
                    return;
                }
            }
        }
    }
}
