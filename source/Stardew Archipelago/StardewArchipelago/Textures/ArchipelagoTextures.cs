/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StardewArchipelago.Textures
{
    public static class ArchipelagoTextures
    {
        public const string COLOR = "color";
        public const string WHITE = "white";
        public const string BLUE = "blue";
        public const string BLACK = "black";
        public const string RED = "red";
        public const string PLEADING = "pleading";
        public const string PROGRESSION = "progression";

        public const string ICON_SET_CUSTOM = "Custom";
        public const string ICON_SET_ORIGINAL = "Original";

        public static readonly string[] ValidLogos = { COLOR, WHITE, BLUE, BLACK, RED, PLEADING };

        public static Texture2D GetArchipelagoLogo(IMonitor monitor, IModHelper modHelper, int size, string color, string preferredIconSet = null)
        {
            var archipelagoFolder = "Archipelago";
            preferredIconSet = GetChosenIconSet(preferredIconSet);
            var fileName = $"{size}x{size} {color} icon.png";
            var relativePathToTexture = Path.Combine(archipelagoFolder, preferredIconSet, fileName);
            var texture = TexturesLoader.GetTexture(monitor, modHelper, relativePathToTexture);
            if (texture == null)
            {
                // Let's try to get the icon from the other set
                preferredIconSet = GetOtherIconSet(preferredIconSet);
                fileName = $"{size}x{size} {color} icon.png";
                relativePathToTexture = Path.Combine(archipelagoFolder, preferredIconSet, fileName);
                texture = TexturesLoader.GetTexture(monitor, modHelper, relativePathToTexture);
                if (texture == null)
                {
                    throw new InvalidOperationException($"Could not find texture {fileName}");
                }
            }

            return texture;
        }

        private static string GetPreferredIconSet()
        {
            return ModEntry.Instance.Config.UseCustomArchipelagoIcons? ICON_SET_CUSTOM : ICON_SET_ORIGINAL;
        }

        private static string GetChosenIconSet(string iconSet)
        {
            if (iconSet == ICON_SET_ORIGINAL || iconSet == ICON_SET_CUSTOM)
            {
                return iconSet;
            }
            return GetPreferredIconSet();
        }

        private static string GetOtherIconSet(string iconSet)
        {
            if (iconSet == ICON_SET_CUSTOM)
            {
                return ICON_SET_ORIGINAL;
            }
            if (iconSet == ICON_SET_ORIGINAL)
            {
                return ICON_SET_CUSTOM;
            }
            return GetPreferredIconSet();
        }
    }
}
