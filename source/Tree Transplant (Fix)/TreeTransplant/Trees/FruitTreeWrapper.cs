/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/TreeTransplant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using Vector2 = System.Numerics.Vector2;

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
			get { return tree.texture; }
		}

		public bool flipped
		{
			get { return tree.flipped.Value; }
			set { tree.flipped.Set(value); }
		}

		public Rectangle getBoundingBox(Vector2 tileLocation)
		{
			return tree.getBoundingBox();
		}

		public string treeType
		{
			get { return tree.treeId.Value; }
		}

		public Rectangle treeTopSourceRect
		{
			get
			{
				bool adult = tree.growthStage.Value > 3;
				int season = Utility.getSeasonNumber(Game1.currentSeason);

				return new Rectangle(
					tree.growthStage.Value * 48 + (adult ? season * 48 : 4), // offset the small trees because idk
					int.Parse(tree.treeId.Value) * 80,
					48,
					80
				);
			}
		}

		public Rectangle stumpSourceRect
		{
			get { return new Rectangle(384, (int.Parse(tree.treeId.Value)  * 80) + 56, 48, 24); }
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
