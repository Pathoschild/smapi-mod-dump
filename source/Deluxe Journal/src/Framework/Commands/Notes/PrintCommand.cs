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

namespace DeluxeJournal.Framework.Commands.Tasks
{
    internal class PrintCommand : Command
    {
        public PrintCommand() : base("dj_notes_print", "Prints the saved notes text to the console.")
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                monitor.Log("A save file must be loaded before accessing the notes.", LogLevel.Error);
                return;
            }

            monitor.Log(new StringBuilder(DeluxeJournalMod.Instance!.GetNotes()).Replace("\n", "").Replace('\r', '\n').ToString(), LogLevel.Info);
        }
    }
}
