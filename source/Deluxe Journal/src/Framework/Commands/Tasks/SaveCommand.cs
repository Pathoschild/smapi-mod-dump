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

namespace DeluxeJournal.Framework.Commands.Tasks
{
    internal class SaveCommand : Command
    {
        public SaveCommand() : base("dj_tasks_save", "Save the current state of the tasks list.")
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            DeluxeJournalMod.TaskManager?.Save();
        }
    }
}
