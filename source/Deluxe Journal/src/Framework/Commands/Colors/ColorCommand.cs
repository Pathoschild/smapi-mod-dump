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
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal abstract class ColorCommand(string name, string documentation) : Command(name, documentation)
    {
        /// <summary>Get a <see cref="Color"/> from a hex code argument.</summary>
        /// <param name="arg">Hex string color code argument.</param>
        /// <returns>A <see cref="Color"/> decoded from the user-submitted hex code.</returns>
        /// <exception cref="ArgumentException">The argument could not be decoded as a hex color code.</exception>
        protected static Color ColorFromArg(string arg)
        {
            if (ColorSchema.HexToColor(arg) is not Color color)
            {
                throw new ArgumentException("Invalid hex color code.");
            }

            return color;
        }

        /// <summary>Get a valid <see cref="ColorSchema"/> index from an integer string argument.</summary>
        /// <param name="arg">An index argument.</param>
        /// <param name="allowAppend">Allow an index equal to the length of the list for appending.</param>
        /// <returns>An index of a schema in the <see cref="DeluxeJournalMod.ColorSchemas"/> list.</returns>
        /// <exception cref="ArgumentException">The argument could not be parsed as an integer or was rejected.</exception>
        /// <exception cref="IndexOutOfRangeException">The requested index was out of bounds.</exception>
        protected static int ColorIndexFromArg(string arg, bool allowAppend = false)
        {
            int count = DeluxeJournalMod.ColorSchemas.Count;
            int index;

            if (arg == "0")
            {
                throw new ArgumentException("Index 0 is reserved for the auto-generated default color schema and should not be modified! To bypass this, use index '0!'.");
            }
            else if (!int.TryParse(arg.TrimEnd('!'), out index))
            {
                throw new ArgumentException("Could not parse color schema index.");
            }

            if (index < 0 || index > count || (!allowAppend && index == count))
            {
                throw new IndexOutOfRangeException($"Invalid color schema index. Number of loaded color schemas: {count}.");
            }

            return index;
        }
    }
}
