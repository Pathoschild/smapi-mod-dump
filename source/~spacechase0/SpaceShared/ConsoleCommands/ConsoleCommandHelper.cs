/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace SpaceShared.ConsoleCommands
{
    /// <summary>Provides helper methods for implementing SMAPI console commands.</summary>
    internal static class ConsoleCommandHelper
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Find all console commands in the mod's assembly.</summary>
        /// <param name="mod">The mod whose assembly to scan.</param>
        public static IEnumerable<IConsoleCommand> FindCommandsInAssembly(Mod mod)
        {
            return (
                from type in mod.GetType().Assembly.GetTypes()
                where !type.IsAbstract && typeof(IConsoleCommand).IsAssignableFrom(type)
                select (IConsoleCommand)Activator.CreateInstance(type)
            );
        }

        /// <summary>Find all console commands in the mod's assembly and register them with SMAPI.</summary>
        /// <param name="mod">The mod whose assembly to scan.</param>
        public static void RegisterCommandsInAssembly(Mod mod)
        {
            foreach (var command in ConsoleCommandHelper.FindCommandsInAssembly(mod))
                mod.Helper.ConsoleCommands.Add(command.Name, command.Description, (name, args) => command.Handle(mod.Monitor, name, args));
        }
    }
}
