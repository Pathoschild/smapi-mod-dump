/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using DeluxeJournal.Task;
using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal class NewCommand : ColorCommand
    {
        public NewCommand() : base("dj_colors_new",
            BuildDocString("Add a new color schema.",
                "dj_colors_new <color:main> <color:hover> <color:header> <color:accent> <color:shadow> [color:padding] [color:corner]",
                "color: Color hex code."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            AssertMinArgs(args, 5);

            ColorSchema schema = new(
                ColorFromArg(args[0]),
                ColorFromArg(args[1]),
                ColorFromArg(args[2]),
                ColorFromArg(args[3]),
                ColorFromArg(args[4]),
                args.Length > 5 ? ColorFromArg(args[5]) : null,
                args.Length > 6 ? ColorFromArg(args[6]) : null);

            DeluxeJournalMod.ColorSchemas.Add(schema);
            monitor.Log($"Added new color schema ({DeluxeJournalMod.ColorSchemas.Count - 1}): {schema}", LogLevel.Info);
        }
    }
}
