/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using DeluxeJournal.Framework.Commands;

namespace DeluxeJournal.Framework
{
    internal static class ConsoleCommands
    {
        /// <summary>Add all console commands.</summary>
        /// <param name="helper">Console command helper API.</param>
        /// <param name="monitor">Writes messages to the console.</param>
        public static void AddCommands(ICommandHelper helper, IMonitor monitor)
        {
            foreach (ICommand command in CreateCommands())
            {
                helper.Add(command.Name, command.Documentation, (name, args) => command.TryHandle(monitor, name, args));
            }
        }

        /// <summary>Create and return all command handlers via reflection.</summary>
        private static IEnumerable<ICommand> CreateCommands()
        {
            return typeof(ConsoleCommands).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(ICommand).IsAssignableFrom(type) && type.Namespace?.StartsWith(typeof(ICommand).Namespace!) == true)
                .Select(type => (ICommand)Activator.CreateInstance(type)!);
        }
    }
}
