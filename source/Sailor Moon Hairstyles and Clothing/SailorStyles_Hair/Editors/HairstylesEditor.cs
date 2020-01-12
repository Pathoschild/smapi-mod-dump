using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;

namespace SailorStyles_Hair.Editors
{
	class HairstylesEditor : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Characters/Farmer/hairstyles");
		}
		public void Edit<T>(IAssetData asset)
		{
			// Append sprites to the asset:
			var src = ModEntry.SHelper.Content.Load<Texture2D>("Assets/hairstyles.png");
			var dest = asset.AsImage();
			var srcRect = new Rectangle(0, 0, src.Width, src.Height);
			var destRect = getDestRect(dest.Data.Bounds, src.Bounds);

			// Substitute the asset with a taller version to accomodate our sprites.
			var original = dest.Data;
			var texture = new Texture2D(Game1.graphics.GraphicsDevice, original.Width, destRect.Bottom);
			dest.ReplaceWith(texture);
			dest.PatchImage(original);

			// Patch the sprites into the expanded asset.
			dest.PatchImage(src, srcRect, destRect);
		}
		private Rectangle getDestRect(Rectangle dest, Rectangle src)
		{
			// Align the sprites to the asset tile dimensions.
			var ypos = Math.Min(dest.Height, (int) nearestMultiple(dest.Height, 32));
			return new Rectangle(0, ypos, src.Width, src.Height);
		}
		private float nearestMultiple(float value, float multiple)
		{
			return (float) Math.Round((decimal) value / (decimal) multiple,
				MidpointRounding.AwayFromZero) * multiple;
		}
	}
}
