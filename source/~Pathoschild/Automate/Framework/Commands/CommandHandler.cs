/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.Automate.Framework.Models;
using Pathoschild.Stardew.Common.Commands;
using StardewModdingAPI;

namespace Pathoschild.Stardew.Automate.Framework.Commands
{
    /// <summary>Handles console commands from players.</summary>
    internal class CommandHandler : GenericCommandHandler
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Writes messages to the console.</param>
        /// <param name="config">The mod configuration.</param>
        /// <param name="machineManager">Manages machine groups.</param>
        public CommandHandler(IMonitor monitor, ModConfig config, MachineManager machineManager)
            : base("automate", "Automate", CommandHandler.BuildCommands(monitor, config, machineManager), monitor) { }


        /*********
        ** Private methods
        *********/
        /// <summary>Build the available commands.</summary>
        /// <param name="monitor">Writes messages to the console.</param>
        /// <param name="config">The mod configuration.</param>
        /// <param name="machineManager">Manages machine groups.</param>
        private static ICommand[] BuildCommands(IMonitor monitor, ModConfig config, MachineManager machineManager)
        {
            return new ICommand[]
            {
                new SummaryCommand(monitor, config, machineManager)
            };
        }
    }
}
