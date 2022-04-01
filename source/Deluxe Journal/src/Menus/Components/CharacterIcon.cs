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
using Microsoft.Xna.Framework.Graphics;

namespace DeluxeJournal.Menus.Components
{
    public class CharacterIcon
    {
        public const int IconSize = 9;

        private const int RowLength = IconSize * 14;

        private static readonly Dictionary<string, int> Characters = new Dictionary<string, int>()
        {
            { "?", 0 },
            { "abigail", 1 },
            { "penny", 2 },
            { "maru", 3 },
            { "leah", 4 },
            { "haley", 5 },
            { "emily", 6 },
            { "alex", 7 },
            { "shane", 8 },
            { "sebastian", 9 },
            { "sam", 10 },
            { "harvey", 11 },
            { "elliott", 12 },
            { "sandy", 13 },
            { "marnie", 14 },
            { "caroline", 15 },
            { "robin", 16 },
            { "pierre", 17 },
            { "pam", 18 },
            { "jodi", 19 },
            { "lewis", 20 },
            { "linus", 21 },
            { "willy", 23 },
            { "wizard", 24 },
            { "jas", 26 },
            { "vincent", 27 },
            { "krobus", 28 },
            { "dwarf", 29 },
            { "gus", 30 },
            { "george", 32 },
            { "evelyn", 33 },
            { "demetrius", 34 },
            { "clint", 35 },
            { "kent", 36 },
            { "leo", 37 }
        };

        public static void DrawIcon(SpriteBatch b, string character, Rectangle destination)
        {
            DrawIcon(b, character, destination, Color.White);
        }

        public static void DrawIcon(SpriteBatch b, string character, Rectangle destination, Color color)
        {
            int pixels = Characters.GetValueOrDefault(character.ToLowerInvariant()) * IconSize;

            b.Draw(DeluxeJournalMod.CharacterIconsTexture,
                destination,
                new Rectangle(pixels % RowLength, (pixels / RowLength) * IconSize, IconSize, IconSize),
                color);
        }
    }
}
