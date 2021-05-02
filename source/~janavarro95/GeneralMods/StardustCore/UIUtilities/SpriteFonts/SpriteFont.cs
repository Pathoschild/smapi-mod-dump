/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.IO;
using StardustCore.UIUtilities.SpriteFonts.Fonts;

namespace StardustCore.UIUtilities.SpriteFonts
{
    /// <summary>Manages Fonts for Stardust core. While all fonts variables can be accessed from their classes, they can also hold a reference here.</summary>
    public class SpriteFonts
    {
        public static string FontDirectory;

        public static VanillaFont vanillaFont;

        public static void initialize()
        {
            FontDirectory = Path.Combine(ModCore.ContentDirectory, "Fonts");
            vanillaFont = new VanillaFont();
        }
    }
}
