/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace SafeLightning.CommandParsing.Commands
{
    /// <summary>Sets the weather for today and tomorrow as lightning.</summary>
    internal class SetLightningCommand : BaseCommand
    {
        /*********
        ** Public methods
        *********/

        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">The monitor used for command output.</param>
        public SetLightningCommand(IMonitor monitor) : base(monitor, "set_lightning", "sl", "Sets today and tomorrow's weather to lightning.")
        {
        }

        /// <summary>Invoke the command.</summary>
        /// <param name="args">The command arguments</param>
        public override void Invoke(string[] args)
        {
            Game1.weatherForTomorrow = Game1.weather_lightning;
            Game1.isLightning = true;

            this.monitor.Log("Okay, set weather for today and tomorrow as lightning.", LogLevel.Info);
        }
    }
}
