using System;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace TreeTransplant
{
	public class TreeRenderer
	{
		public ITree tree;
		public Vector2 location;
		public bool flipped;

		public TreeRenderer(ITree itree)
		{
			Initialize(itree);
		}

		public TreeRenderer(TerrainFeature tf)
		{
			Initialize(Cast(tf));
		}

		void Initialize(ITree itree)
		{
			this.tree = itree;
			this.flipped = tree.flipped;
		}

		public void propFlip()
		{
			tree.flipped = flipped;
		}

		private ITree Cast(TerrainFeature tf)
		{
			// return the appropriate wrapper
			if (tf is Tree)
				return new TreeWrapper((Tree)tf);
			if (tf is FruitTree)
				return new FruitTreeWrapper((FruitTree)tf);
			
			// throw an exception
			throw new InvalidCastException("Type not of Tree or FruitTree cannot cast!");
		}

		public void draw(SpriteBatch b, Vector2 tileLocation)
		{
			// only draw the stump if we need to
			if (tree.stump)
				drawPart(b, tileLocation, tree.stumpSourceRect);
			drawPart(b, tileLocation, tree.treeTopSourceRect);
		}

		private void drawPart(SpriteBatch b, Vector2 tileLocation, Rectangle source)
		{
			// use spritebatch to draw the texture
			b.Draw(
				// drawing the tree texture
				tree.texture,
				// take global space and convert to local screen space
				Game1.GlobalToLocal(
					Game1.viewport,
					new Vector2(
						// middle of the tile horizontally
						(tileLocation.X * Game1.tileSize) + (Game1.tileSize / 2),
						// bottom edge of the tile vertically
						(tileLocation.Y * Game1.tileSize) + Game1.tileSize
					)
				),
				// render from the source rectangle
				source,
				// render with white color
				Color.White * 0.75f,
				// no rotation
				0.0f,
				// set the offset relative to the source rectangle dimensions
				new Vector2(source.Width / 2, source.Height),
				// scale with the pixel zoom
				Game1.pixelZoom,
				// flip the sprite if necessary
				tree.flipped || flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				// depth with z-buffer (not entirely sure of the equation here)
				0.999f
			);
		}
	}
}
