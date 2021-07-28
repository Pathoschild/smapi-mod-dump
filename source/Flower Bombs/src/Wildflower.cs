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
using PlatoTK;
using PlatoTK.Objects;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class Wildflower : PlatoSObject<ColoredObject>
	{
		internal new protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;
		internal protected static IPlatoHelper PlatoHelper => ModEntry.Instance.platoHelper;

		public int flowerIndex => Base?.ParentSheetIndex ?? -1;

		public Color color
		{
			get
			{
				return Base?.color?.Value ?? Color.White;
			}
			private set
			{
				Base.color.Value = value;
			}
		}

		internal static void Register ()
		{
			// Make the magic happen.
			PlatoHelper.Harmony.LinkContruction<ColoredObject, Wildflower> ();
			PlatoHelper.Harmony.LinkTypes (typeof (ColoredObject), typeof (Wildflower));
		}

		// Intentionally not using GetNew here. Once the item is picked up,
		// the special behavior is no longer needed.
		public override Item getOne ()
		{
			return new ColoredObject (flowerIndex, 1, color);
		}

		public static ColoredObject GetNew (int flowerIndex, Color color,
			int stack = 1)
		{
			var newObject = new ColoredObject (flowerIndex, stack, color);
			PlatoObject<SObject>.SetIdentifier (newObject, typeof (Wildflower));
			return newObject;
		}

		// Prevent forage XP and quality from applying to flowers from Flower Bombs.
		public override bool isForage (GameLocation location)
		{
			return false;
		}

		// This is needed to avoid a spurious interaction with the override in
		// the FlowerBomb class.
		public override bool performToolAction (Tool tool, GameLocation location)
		{
			return Link.CallUnlinked<ColoredObject, bool> (o =>
				o.performToolAction (tool, location));
		}

		public override void draw (SpriteBatch b, int x, int y, float alpha = 1f)
		{
			if (Game1.eventUp && (Game1.CurrentEvent?.isTileWalkedOn (x, y) ?? false))
				return;

			b.Draw (Game1.shadowTexture,
				Base.getLocalPosition (Game1.viewport) + new Vector2 (32f, 53f),
				Game1.shadowTexture.Bounds, Color.White, 0f,
				Utility.PointToVector2 (Game1.shadowTexture.Bounds.Center),
				4f, SpriteEffects.None, 1E-07f);

			Vector2 position = Game1.GlobalToLocal (Game1.viewport,
				new Vector2 (x * 64 + 32, y * 64 + 32));
			Vector2 origin = new (8f, 8f);
			float scale = (Base.scale.Y > 1f) ? Base.getScale ().Y : 4f;
			float layerDepth = Base.getBoundingBox (new Vector2 (x, y)).Bottom / 10000f;
			var fx = Base.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			b.Draw (Game1.objectSpriteSheet, position,
				GameLocation.getSourceRectForObject (flowerIndex),
				Color.White, 0f, origin, scale, fx, layerDepth);

			if (color != Color.White)
			{
				b.Draw (Game1.objectSpriteSheet, position,
					GameLocation.getSourceRectForObject (flowerIndex + 1),
					color, 0f, origin, scale, fx, layerDepth + 2E-05f);
			}
		}
	}
}
