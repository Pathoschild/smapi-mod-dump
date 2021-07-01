/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace ScryingOrb
{
	internal class CursorEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public static readonly int TilePosition = 49;

		protected readonly Texture2D cursor;

		public CursorEditor ()
		{
			cursor = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "cursor.png"));

			Helper.Events.Display.RenderedHud +=
				(_sender, _e) => apply ();
			Helper.Events.Display.RenderingActiveMenu += onRenderingActiveMenu;
			Helper.Events.Display.RenderedActiveMenu += onRenderedActiveMenu;
		}

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.DataType == typeof (Texture2D) &&
				asset.AssetNameEquals ("LooseSprites\\Cursors");
		}

		public void Edit<_T> (IAssetData asset)
		{
			IAssetDataForImage editor = asset.AsImage ();
			editor.PatchImage (cursor, targetArea:
				Game1.getSourceRectForStandardTileSheet (editor.Data,
					TilePosition, 16, 16));
		}

		public bool active =>
			ModEntry.Instance.orbHovered || ModEntry.Instance.orbsIlluminated > 0;

		public void apply ()
		{
			if (active)
				Game1.mouseCursor = TilePosition;
			else if (Game1.mouseCursor == TilePosition)
				Game1.mouseCursor = 0;
		}

		private void onRenderingActiveMenu (object _sender,
			RenderingActiveMenuEventArgs _e)
		{
			// When active, prevent the normal software cursor from being drawn
			// by the menu.
			if (active && !Game1.options.hardwareCursor)
				Game1.mouseCursorTransparency = 0f;
		}

		private void onRenderedActiveMenu (object _sender,
			RenderedActiveMenuEventArgs e)
		{
			// When active, draw the special cursor instead. Restoring the
			// regular mouseCursorTransparency is apparently not helpful.
			if (active && !Game1.options.hardwareCursor)
			{
				e.SpriteBatch.Draw (Game1.mouseCursors,
					new Vector2 (Game1.getMouseX (), Game1.getMouseY ()),
					Game1.getSourceRectForStandardTileSheet
						(Game1.mouseCursors, TilePosition, 16, 16),
					Color.White, 0f, Vector2.Zero,
					4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			}
		}
	}
}
