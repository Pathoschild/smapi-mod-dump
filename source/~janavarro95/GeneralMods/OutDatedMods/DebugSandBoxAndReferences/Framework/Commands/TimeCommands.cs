using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugSandBoxAndReferences.Framework.Commands
{
    class TimeCommands
    {
        /// <summary>
        /// Registers all time modifying commands.
        /// </summary>
        /// <param name="helper"></param>
        public static void registerCommands(StardewModdingAPI.IModHelper helper)
        {
            helper.ConsoleCommands.Add("debug_world_settime", "Sets the in-game time to be any valid int. Allows for setting time to be early in the morning, i.e. 3 A.M", world_settime);
            helper.ConsoleCommands.Add("debug_world_modifytimetick", "Sets the time interval in seconds between 10-minute in-game update ticks.", world_modifyTimeTick);
        }

        /// <summary>
        /// Sets the time of day to the first argument passed into this command. This argument should be an integer value.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="args">The parameters to pass in. Only the first one is read as that should be the integer valuve for time.</param>
        public static void world_settime(string name, string[] args)
        {
            Game1.timeOfDay = Convert.ToInt32(args[0]);
        }

        /// <summary>
        /// Sets the time interval in seconds between 10-minute in-game update ticks.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public static void world_modifyTimeTick(string name, string[] args)
        {
            Game1.gameTimeInterval = Convert.ToInt32(args[0]);
        }



    }
}
