/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal class EditCommand : ColorCommand
    {
        public EditCommand() : base("dj_colors_edit",
            BuildDocString("Edit a color in an existing color schema.",
                "dj_colors_edit <index> <key> <color>",
                "index: Index of the color schema to edit.",
                "key: Name of the color property key. E.g. 'Main'.",
                "color: Color hex code."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            AssertMinArgs(args, 3);

            int index = ColorIndexFromArg(args[0]);
            string key = args[1];
            Color color = ColorFromArg(args[2]);
            ColorSchema schema = DeluxeJournalMod.ColorSchemas[index];

            foreach (var property in typeof(ColorSchema).GetProperties())
            {
                if (property.Name.Equals(key, StringComparison.OrdinalIgnoreCase) && property.PropertyType == typeof(Color))
                {
                    property.SetValue(schema, color);
                    monitor.Log($"Modified color schema ({index}): {schema}", LogLevel.Info);
                    return;
                }
            }

            throw new ArgumentException($"Invalid property name '{key}'.");
        }
    }
}
