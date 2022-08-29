/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SmartBuilding.UI;

namespace SmartBuilding.Utilities
{
    public class Ui
    {
        /// <summary>
        ///     Get the source rectangle for the appropriate Texture2D for each individual button.
        /// </summary>
        /// <param name="button">The ID of the button being checked.</param>
        /// <returns>The <see cref="Rectangle" /> source rect for the button in question.</returns>
        public static Rectangle GetButtonSourceRect(ButtonId button)
        {
            switch (button)
            {
                case ButtonId.Draw:
                    return new Rectangle(0, 0, 16, 16);
                case ButtonId.Erase:
                    return new Rectangle(16, 0, 16, 16);
                case ButtonId.Insert:
                    return new Rectangle(32, 0, 16, 16);
                case ButtonId.Rectangle:
                    return new Rectangle(16, 16, 16, 16);
                case ButtonId.FilledRectangle:
                    return new Rectangle(0, 16, 16, 16);
                case ButtonId.Select:
                    return new Rectangle(32, 16, 16, 16);
                case ButtonId.DrawnLayer:
                    return new Rectangle(0, 48, 16, 16);
                case ButtonId.ObjectLayer:
                    return new Rectangle(0, 32, 16, 16);
                case ButtonId.TerrainFeatureLayer:
                    return new Rectangle(16, 32, 16, 16);
                case ButtonId.FurnitureLayer:
                    return new Rectangle(32, 32, 16, 32);
                case ButtonId.ConfirmBuild:
                    return new Rectangle(16, 64, 16, 16);
                case ButtonId.ClearBuild:
                    return new Rectangle(32, 64, 16, 16);
            }

            return new Rectangle(1, 3, 3, 7);
        }
    }
}
