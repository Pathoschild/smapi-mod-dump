using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	class BigCraftablesTilesheetEditor : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"TileSheets/Craftables");
		}

		public void Edit<T>(IAssetData asset)
		{
			// Expand the base tilesheet if needed.
			var src = ModEntry.Instance.Helper.Content.Load<Texture2D>(Data.MachinePath);
			var dest = asset.AsImage();
			var srcRect = new Rectangle(0, 0, 16, 32);
			var destRect = Propagator.getSourceRectForBigCraftable(Data.PropagatorIndex);

			if (destRect.Bottom > dest.Data.Height)
			{
				Log.T("Expanding bigCraftables tilesheet.");

				var original = dest.Data;
				var texture = new Texture2D(Game1.graphics.GraphicsDevice, original.Width, destRect.Bottom);
				dest.ReplaceWith(texture);
				dest.PatchImage(original);
			}

			// Append machine sprite onto the default tilesheet.
			dest.PatchImage(src, srcRect, destRect);
		}
	}
}
