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

namespace ChatCommands.Commands
{
    /// <summary>Base class for commands.</summary>
    internal abstract class BaseCommand : ICommand
    {
        protected IMonitor Monitor;

        protected BaseCommand(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        public abstract void Register(ICommandHelper helper);
    }
}
