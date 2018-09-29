using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreeTransplant
{
	public class FruitTreeWrapper : ITree
	{
		private FruitTree tree;

		public FruitTreeWrapper(FruitTree fruitTree)
		{
			tree = fruitTree;
		}

		public TerrainFeature getTerrainFeature()
		{
			return tree;
		}

		public Texture2D texture
		{
			get { return FruitTree.texture; }
		}

		public bool flipped
		{
			get { return tree.flipped.Value; }
			set { tree.flipped.Set(value); }
		}

		public int treeType
		{
			get { return tree.treeType.Value; }
		}

		public Rectangle treeTopSourceRect
		{
			get 
			{
				bool adult = tree.growthStage.Value > 3;
				int season = Utility.getSeasonNumber(Game1.currentSeason);

				return new Rectangle(
					tree.growthStage.Value * 48 + (adult ? season * 48 : 4), // offset the small trees because idk
					tree.treeType.Value * 80, 
					48, 
					80
				); 
			}
		}

		public Rectangle stumpSourceRect
		{
			get { return new Rectangle(384, (tree.treeType.Value * 80) + 56, 48, 24); }
		}

		public Rectangle getBoundingBox(Vector2 tileLocation)
		{
			return tree.getBoundingBox(tileLocation);
		}

		public bool stump
		{
			get { return tree.stump.Value; }
		}

		public bool isAdult()
		{
			return tree.growthStage.Value > 3;
		}

		public bool isStumpSeparate()
		{
			return false;
		}
	}
}
