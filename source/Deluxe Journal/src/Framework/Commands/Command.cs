/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Text;
using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands
{
    internal abstract class Command(string name, string documentation) : ICommand
    {
        /// <inheritdoc/>
        public string Name { get; } = name;

        /// <inheritdoc/>
        public string Documentation { get; } = documentation;

        /// <inheritdoc/>
        public void TryHandle(IMonitor monitor, string command, string[] args)
        {
            try
            {
                Handle(monitor, command, args);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IndexOutOfRangeException || ex is InvalidOperationException)
            {
                LogUsageError(monitor, ex.Message);
            }
        }

        /// <summary>Handle the command.</summary>
        /// <inheritdoc cref="TryHandle(IMonitor, string, string[])"/>
        /// <exception cref="ArgumentException">Invalid or insufficient user-submitted arguments.</exception>
        /// <exception cref="IndexOutOfRangeException">Attempted to access an internal collection with an index that is out of bounds.</exception>
        /// <exception cref="InvalidOperationException">A problem has occurred while processing the command.</exception>
        abstract protected void Handle(IMonitor monitor, string command, string[] args);

        /// <summary>Builds a command documentation string.</summary>
        /// <param name="brief">Brief description.</param>
        /// <param name="usage">Command usage format.</param>
        /// <param name="args">Command argument help strings.</param>
        protected static string BuildDocString(string brief, string usage, params string[] args)
        {
            StringBuilder doc = new(brief);

            if (!string.IsNullOrEmpty(usage))
            {
                doc.Append("\n\n Usage: ");
                doc.Append(usage);

                foreach (string arg in args)
                {
                    doc.Append("\n\t");
                    doc.Append(arg);
                }
            }

            return doc.ToString();
        }

        /// <summary>Assert that a minimum number of arguments are provided.</summary>
        /// <param name="args">Command arguments.</param>
        /// <param name="min">Minimum number of arguments.</param>
        /// <exception cref="ArgumentException">Insufficient number of arguments.</exception>
        protected static void AssertMinArgs(string[] args, int min)
        {
            if (args.Length < min)
            {
                throw new ArgumentException("Insufficient number of arguments.");
            }
        }

        /// <summary>Write an error message to the console with the accompanying usage message.</summary>
        /// <param name="monitor">Writes messages to the console.</param>
        /// <param name="message">Error message that explains the problem.</param>
        protected void LogUsageError(IMonitor monitor, string message)
        {
            monitor.Log($"{message} Type 'help {Name}' for usage.", LogLevel.Error);
        }
    }
}
