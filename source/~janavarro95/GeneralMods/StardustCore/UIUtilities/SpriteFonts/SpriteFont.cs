using StardustCore.UIUtilities.SpriteFonts.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.SpriteFonts
{
    /// <summary>
    /// Manages Fonts for Stardust core. While all fonts variables can be accessed from their classes, they can also hold a reference here.
    /// </summary>
    public class SpriteFonts
    {
        public static string FontDirectory;

        public static VanillaFont vanillaFont;
        
        public static void initialize()
        {
            FontDirectory = Path.Combine(StardustCore.ModCore.ContentDirectory, "Fonts");
            if (!Directory.Exists(FontDirectory)) Directory.CreateDirectory(FontDirectory);
            vanillaFont = new VanillaFont();
        }
    }
}
