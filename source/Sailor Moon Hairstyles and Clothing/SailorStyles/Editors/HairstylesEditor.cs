using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;

namespace SailorStyles.Editors
{
	internal class HairstylesEditor : IAssetEditor
	{
		private readonly IModHelper _helper;
		
		public HairstylesEditor(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Characters/Farmer/hairstyles");
		}

		public void Edit<T>(IAssetData asset)
		{
			// Append sprites to the asset:
			var src = _helper.Content.Load<Texture2D>(
				Path.Combine("assets", $"{ModConsts.HairstylesSpritesheet}.png"));
			var dest = asset.AsImage();
			var srcRect = new Rectangle(0, 0, src.Width, src.Height);
			var destRect = GetDestRect(dest.Data.Bounds, src.Bounds);
			
			Log.D($"Dest:   (X:{dest.Data.Bounds.X}, Y:{dest.Data.Bounds.Y}"
			      + $", W:{dest.Data.Bounds.Width}, H:{dest.Data.Bounds.Height})"
			      + $"\nSource: (X:{srcRect.X}, Y:{srcRect.Y}, W:{srcRect.Width}, H:{srcRect.Height})"
			      + $"\nResult: (X:{destRect.X}, Y:{destRect.Y}, W:{destRect.Width}, H:{destRect.Height})",
				ModEntry.Instance.Config.DebugMode);

			// Substitute the asset with a taller version to accomodate our sprites.
			var original = dest.Data;
			var texture = new Texture2D(Game1.graphics.GraphicsDevice, original.Width, destRect.Bottom);
			dest.ReplaceWith(texture);
			dest.PatchImage(original);

			// Patch the sprites into the expanded asset.
			dest.PatchImage(src, srcRect, destRect);
		}
		
		private Rectangle GetDestRect(Rectangle dest, Rectangle src)
		{
			var targetY = Math.Min(dest.Height, (int) NearestMultiple(dest.Height, 32));

			// Record the index based on Y position.
			ModConsts.HairstylesInitialIndex = targetY / 32 * 3;
			Log.D($"Hairstyles initial index: {ModConsts.HairstylesInitialIndex}",
				ModEntry.Instance.Config.DebugMode);

			// Align the sprites to the asset tile dimensions.
			return new Rectangle(0, targetY, src.Width, src.Height);
		}

		private float NearestMultiple(float value, float multiple)
		{
			return (float) Math.Round((decimal) value / (decimal) multiple,
				MidpointRounding.AwayFromZero) * multiple;
		}
	}
}