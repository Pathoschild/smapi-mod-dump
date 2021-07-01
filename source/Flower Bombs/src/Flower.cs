/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Linq;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public class Flower : ColoredObject
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;

		public static readonly int SpecialFlag = 79400700;

		public Flower ()
		{
			SpecialVariable = SpecialFlag;
		}

		public Flower (int parentSheetIndex, int stack, Color color)
		: base (parentSheetIndex, stack, color)
		{
			SpecialVariable = SpecialFlag;
		}

		public override bool isForage (GameLocation location)
		{
			// Prevent forage XP and quality from applying to bomb flowers.
			return false;
		}

		public override void draw (SpriteBatch b, int x, int y, float alpha = 1f)
		{
			if (Game1.eventUp && !(Game1.CurrentEvent?.isTileWalkedOn (x, y) ?? true))
				return;

			b.Draw (Game1.shadowTexture,
				getLocalPosition (Game1.viewport) + new Vector2 (32f, 53f),
				Game1.shadowTexture.Bounds, Color.White, 0f,
				Utility.PointToVector2 (Game1.shadowTexture.Bounds.Center),
				4f, SpriteEffects.None, 1E-07f);

			Vector2 position = Game1.GlobalToLocal (Game1.viewport,
				new Vector2 (x * 64 + 32, y * 64 + 32));
			Vector2 origin = new (8f, 8f);
			float scale = (this.scale.Y > 1f) ? getScale ().Y : 4f;
			float layerDepth = getBoundingBox (new Vector2 (x, y)).Bottom / 10000f;
			var fx = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			b.Draw (Game1.objectSpriteSheet, position,
				GameLocation.getSourceRectForObject (ParentSheetIndex),
				Color.White, 0f, origin, scale, fx, layerDepth);

			if (color.Value != Color.White)
			{
				b.Draw (Game1.objectSpriteSheet, position,
					GameLocation.getSourceRectForObject (ParentSheetIndex + 1),
					color.Value, 0f, origin, scale, fx, layerDepth + 2E-05f);
			}
		}

		public override Item getOne ()
		{
			if (color.Value != Color.White)
				return base.getOne ();

			SObject @object = new (ParentSheetIndex, 1);
			@object.Quality = Quality;
			@object.Price = Price;
			@object.HasBeenInInventory = HasBeenInInventory;
			@object.HasBeenPickedUpByFarmer = HasBeenPickedUpByFarmer;
			@object.SpecialVariable = SpecialVariable;
			@object.Name = Name;
			return @object;
		}

		private ColoredObject getSerializable ()
		{
			ColoredObject co = (ColoredObject) base.getOne ();
			co.IsSpawnedObject = IsSpawnedObject;
			co.CanBeGrabbed = CanBeGrabbed;
			return co;
		}

		public static void ConvertAll ()
		{
			foreach (GameLocation location in Game1.locations)
			{
				foreach (var kvp in location.objects.Pairs.ToArray ())
				{
					if (kvp.Value is ColoredObject co &&
						co.SpecialVariable == SpecialFlag)
					{
						Flower flower = new (co.ParentSheetIndex,
							co.Stack, co.color?.Value ?? Color.White);
						flower.IsSpawnedObject = co.IsSpawnedObject;
						flower.CanBeGrabbed = co.CanBeGrabbed;
						location.objects[kvp.Key] = flower;
					}
				}
			}
		}

		public static void RevertAll ()
		{
			foreach (GameLocation location in Game1.locations)
			{
				foreach (var kvp in location.objects.Pairs.ToArray ())
				{
					if (kvp.Value is Flower flower)
						location.objects[kvp.Key] = flower.getSerializable ();
				}
			}
		}
	}
}
