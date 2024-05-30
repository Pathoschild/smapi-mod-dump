/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace LetsMoveIt
{
    internal class Mod1
    {
        /// <summary>Get the local cursor tile with local offset.</summary>
        /// <param name="x">Offset X</param>
        /// <param name="y">Offset Y</param>
        public static Vector2 LocalCursorTile(float x = 0, float y = 0)
        {
            return Game1.GlobalToLocal(new Vector2(x, y) + Game1.currentCursorTile * 64);
        }

        /// <summary>Get the local cursor tile with local offset.</summary>
        /// <param name="offset">Offset</param>
        public static Vector2 LocalCursorTile(Vector2 offset)
        {
            return Game1.GlobalToLocal(offset + Game1.currentCursorTile * 64);
        }

        /// <summary>Get the global Mouse Position.</summary>
        public static Point GetGlobalMousePosition()
        {
            return Game1.getMousePosition() + new Point(Game1.viewport.X, Game1.viewport.Y);
        }

        public static Response[] YesNoResponses()
        {
            return
            [
            new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")).SetHotKey(Keys.Y).SetHotKey(Keys.Enter),
            new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
            ];
        }
    }
}
