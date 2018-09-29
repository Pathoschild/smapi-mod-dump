using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace TreeTransplant
{
	public class TreeWrapper : ITree
	{
		private Tree tree;

		public TreeWrapper(Tree t)
		{
			tree = t;
		}

		public TerrainFeature getTerrainFeature()
		{
			return tree;
		}

		public Texture2D texture
		{
			get 
			{
                if (isAdult() && !tree.stump.Value)
                {
                    if (tree.treeType.Value == Tree.bushyTree || tree.treeType.Value == Tree.leafyTree || tree.treeType.Value == Tree.pineTree)
                        return TreeTransplant.treeTexture;
                    else if (tree.treeType.Value == Tree.mushroomTree || tree.treeType.Value == Tree.palmTree)
                        return TreeTransplant.specialTreeTexture;
                }
                return TreeTransplant.helper.Reflection.GetField<Lazy<Texture2D>>(tree, "texture").GetValue().Value;
			}
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
				if (!isAdult())
				{
					Rectangle rect;
					switch (tree.growthStage.Value)
					{
						// seed
						case 0:
							rect = new Rectangle(32, 128, 16, 16);
							break;
						// sprout
						case 1:
							rect = new Rectangle(0, 128, 16, 16);
							break;
						// full sprout
						case 2:
							rect = new Rectangle(16, 128, 16, 16);
							break;
						// mini tree
						default:
							rect = new Rectangle(0, 96, 16, 32);
							break;
					}
					return rect;
				}

                bool basicTree = (tree.treeType.Value >= 1 && tree.treeType.Value <= 3);
                int xOffset = tree.treeType.Value - (basicTree ? 1 : 6);
                int yOffset = basicTree ? Utility.getSeasonNumber(Game1.currentSeason) : 0;

                return new Rectangle(48 * xOffset, yOffset * 96, 48, 96);
			}
		}

		public Rectangle stumpSourceRect
		{
			get { return Tree.stumpSourceRect; }
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
			return tree.growthStage.Value > 4;
		}

		public bool isStumpSeparate()
		{
			return isAdult();
		}
	}
}
