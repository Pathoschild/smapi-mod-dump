/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SkillPrestige.Framework.InputHandling
{
    /// <summary>Handles mouse interactions with Stardew Valley.</summary>
    internal static class Mouse
    {
        public static bool ForceUseGamepadSelectionMouse;

        /// <summary>Draws the mouse cursor, which should be called last in any draw command to ensure the mouse is on top of the content.</summary>
        /// <param name="spriteBatch">The spriteBatch to draw to.</param>
        public static void DrawCursor(SpriteBatch spriteBatch)
        {
            if (Game1.options.hardwareCursor)
                return;
            var mousePosition = new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY());

            var cursorTextureLocation = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls || ForceUseGamepadSelectionMouse ? 44 : 0, 16, 16);
            spriteBatch.Draw(Game1.mouseCursors, mousePosition, cursorTextureLocation, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }
    }
}
