/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cropbeasts
{
	public class GuestCropTile
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;
		protected static object JsonAssetsMod => ModEntry.Instance.jsonAssetsMod;

		public readonly Crop crop;

		public readonly int harvestIndex;
		public readonly int seedIndex;

		public readonly bool giantCrop;
		public readonly bool repeatingCrop;

		public readonly Point tileLocation;

		public Mapping mapping => Mappings.Get (harvestIndex, giantCrop);

		public virtual string logDescription =>
			$"{mapping?.harvestName ?? "Crop"} (#{harvestIndex}) at ({tileLocation.X},{tileLocation.Y})";

		private Texture2D cropTexture_;
		public Texture2D cropTexture
		{
			get
			{
				loadCropTexture ();
				return cropTexture_;
			}
		}

		private Rectangle cropSourceRect_;
		public Rectangle cropSourceRect
		{
			get
			{
				loadCropTexture ();
				return cropSourceRect_;
			}
		}

		protected GuestCropTile (Crop crop, bool giantCrop, Point tileLocation)
		{
			this.crop = crop;

			harvestIndex = crop.indexOfHarvest.Value;
			seedIndex = crop.netSeedIndex.Value;

			this.giantCrop = giantCrop;
			repeatingCrop = !giantCrop && crop.regrowAfterHarvest.Value != -1;

			this.tileLocation = tileLocation;
		}

		public static GuestCropTile Create (int harvestIndex, bool giantCrop,
			Point tileLocation)
		{
			Crop crop = Utilities.MakeNonceCrop (harvestIndex, tileLocation);
			return new GuestCropTile (crop, giantCrop, tileLocation);
		}

		protected void loadCropTexture ()
		{
			// This is correct for regular crops (stock and custom) and is an
			// adequate fallback for giant crops whose textures cannot be loaded.
			cropTexture_ = Game1.cropSpriteSheet;
			cropSourceRect_ = Helper.Reflection.GetMethod (crop, "getSourceRect")
				.Invoke<Rectangle> (tileLocation.X * 7 + tileLocation.Y * 11);
			if (!giantCrop)
				return;

			// Stock giant crops are elsewhere on the same spritesheet.
			if (Beasts.GiantCropbeast.StockWhich
				.TryGetValue (harvestIndex, out int which))
			{
				cropSourceRect_ = new Rectangle (112 + which * 48, 512, 48, 63);
				return;
			}

			// Support custom giant crops from Json Assets content packs, with
			// some ugly reflection.
			try
			{
				var crops = Helper.Reflection.GetField<List<object>>
					(JsonAssetsMod, "crops").GetValue ();
				var resolver = Helper.Reflection.GetMethod
					(JsonAssetsMod, "ResolveObjectId");
				var crop = crops.Single ((crop) =>
				{
					var product = Helper.Reflection.GetField<object>
						(crop, "Product").GetValue ();
					return resolver.Invoke<int> (product) == harvestIndex;
				});

				cropTexture_ = Helper.Reflection.GetField<Texture2D>
					(crop, "giantTex").GetValue ();
				cropSourceRect_ = new Rectangle (0, 0,
					cropTexture_.Width, cropTexture_.Height);
			}
			catch (Exception e)
			{
				Monitor.LogOnce ($"Json Assets didn't provide a giant crop texture for crop ID {harvestIndex}: {e}",
					LogLevel.Warn);
			}
		}
	}
}
