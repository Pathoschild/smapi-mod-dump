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
    internal class GenCommand : ColorCommand
    {
        public GenCommand() : base("dj_colors_gen",
            BuildDocString("Generate a simple color schema from a single source color.",
                "dj_colors_gen <index> <color>",
                "index: Index of the color schema to replace. Appends a new schema if index equals the length of the list.",
                "color: Color hex code."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            AssertMinArgs(args, 2);

            int index = ColorIndexFromArg(args[0], true);
            Color main = ColorFromArg(args[1]);

            ColorSchema.ColorToHSV(main, out float _, out float _, out float mainValue);
            float luminance = ColorSchema.Luminance(main);
            float satBoost = 1f - mainValue;
            float valueShift = Math.Min(0, ColorSchema.ValueShiftLumThreshold - luminance);

            Color hover = ColorSchema.HSVShiftColor(main, ColorSchema.HueShift, ColorSchema.HoverMaxSatShift * luminance + satBoost, valueShift, 250f);
            Color header = ColorSchema.HSVShiftColor(main, ColorSchema.HueShift, ColorSchema.HeaderMaxSatShift * luminance + satBoost, valueShift, 250f);
            Color accent = ColorSchema.HSVShiftColor(main, 14f, 0.45f, -0.52f, 250f);
            Color shadow = ColorSchema.HSVShiftColor(main, 5f, 0.18f, -0.1f, 250f);
            ColorSchema schema = new(main, hover, header, accent, shadow);

            if (index == DeluxeJournalMod.ColorSchemas.Count)
            {
                DeluxeJournalMod.ColorSchemas.Add(schema);
            }
            else
            {
                DeluxeJournalMod.ColorSchemas[index] = schema;
            }

            monitor.Log($"Generated color schema ({index}): {schema}", LogLevel.Info);
        }
    }
}
