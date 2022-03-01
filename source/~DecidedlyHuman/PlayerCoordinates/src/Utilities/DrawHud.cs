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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace PlayerCoordinates.Utilities
{
	public static class DrawHud
	{
		private static int _xPosXOffset = 14;
		private static int _xPosYOffset = 8;
		private static int _yPosXOffset = _xPosXOffset;
		private static int _yPosYOffset = _xPosYOffset + 36;

		public static void Draw(Vector2 hudPos, Coordinates currentCoords, Texture2D coordinateBox, RenderedHudEventArgs world)
		{
#if ANDROID
            hudPos = new Vector2(125, 9); // This is bad but we want to hardcode the HUD position on Android.
            Vector2 topTextPosition = new Vector2(148, 17);
            Vector2 bottomTextPosition = new Vector2(topTextPosition.X, topTextPosition.Y + 36);
#else
			Vector2 topTextPosition = hudPos + new Vector2(_xPosXOffset, _xPosYOffset);
			Vector2 bottomTextPosition = hudPos + new Vector2(_yPosXOffset, _yPosYOffset);
#endif

			world.SpriteBatch.Draw(coordinateBox, hudPos, Color.White);
			Utility.drawTextWithShadow(world.SpriteBatch,
				$"X: {currentCoords.x}",
				Game1.dialogueFont,
				topTextPosition,
				Color.Black);
			Utility.drawTextWithShadow(world.SpriteBatch,
				$"Y: {currentCoords.y}",
				Game1.dialogueFont,
				bottomTextPosition,
				Color.Black);
		}
	}
}