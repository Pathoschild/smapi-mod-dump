using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	internal class BigCraftablesTilesheetEditor : IAssetEditor
	{
		private readonly bool _isDebugging;

		public BigCraftablesTilesheetEditor()
		{
			_isDebugging = ModEntry.Instance.Config.DebugMode;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"TileSheets/Craftables");
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Editing {asset.AssetName}.",
				_isDebugging);

			// Expand the base tilesheet if needed.
			var src = ModEntry.Instance.Helper.Content.Load<Texture2D>(Const.MachinePath);
			var dest = asset.AsImage();
			var srcRect = new Rectangle(0, 0, 16, 32);
			var destRect = Propagator.getSourceRectForBigCraftable(Data.PropagatorIndex);

			if (destRect.Bottom > dest.Data.Height)
			{
				Log.D("Expanding bigCraftables tilesheet.",
					_isDebugging);

				var original = dest.Data;
				var texture = new Texture2D(Game1.graphics.GraphicsDevice, original.Width, destRect.Bottom);

				Log.D($"Original: {original.Width}x{original.Height}"
					+ $"\nExpanded: {texture.Width}x{texture.Height}",
					_isDebugging);

				dest.ReplaceWith(texture);
				dest.PatchImage(original);
			}

			// Append machine sprite onto the default tilesheet.
			dest.PatchImage(src, srcRect, destRect);
		}
	}
}
