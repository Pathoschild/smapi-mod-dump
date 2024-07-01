/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Text;

namespace RadialMenu.Gmcm;

// Assorted utilities for GMCM customization that are not dependent on the internals.
internal class UiHelper
{
    public static string BreakParagraph(string text, int maxWidth, bool bold = false)
    {
        // Copied almost verbatim from SpecificModConfigMenu.cs. We want it to look the same.
        var sb = new StringBuilder(text.Length + 50);
        string nextLine = "";
        foreach (string word in text.Split(' '))
        {
            if (word == "\n")
            {
                sb.AppendLine(nextLine);
                nextLine = "";
                continue;
            }
            if (nextLine == "")
            {
                nextLine = word;
                continue;
            }
            string possibleLine = $"{nextLine} {word}".Trim();
            if (MeasureString(possibleLine, bold, font: Game1.smallFont).X <= maxWidth)
            {
                nextLine = possibleLine;
                continue;
            }
            sb.AppendLine(nextLine);
            nextLine = word;
        }
        if (nextLine != "")
            sb.AppendLine(nextLine);
        return sb.ToString();
    }

    private static Vector2 MeasureString(string text, bool bold, SpriteFont? font)
    {
        return bold
            ? new(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text))
            : (font ?? Game1.dialogueFont).MeasureString(text);
    }
}
