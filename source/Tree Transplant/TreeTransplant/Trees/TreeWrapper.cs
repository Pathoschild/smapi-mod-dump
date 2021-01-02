/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeonBlade/TreeTransplant
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace TreeTransplant
{
	public class TreeWrapper : ITree
	{
		private readonly Tree tree;
		private readonly int[] basicTrees = new[] { Tree.bushyTree, Tree.leafyTree, Tree.pineTree, Tree.mahoganyTree };

		public TreeWrapper(Tree t)
		{
			tree = t;
		}

		public TerrainFeature getTerrainFeature() => tree;

		public Texture2D texture
		{
			get
			{
				if (isAdult() && !tree.stump.Value)
				{
					if (basicTrees.Contains(tree.treeType.Value))
						return TreeTransplant.treeTexture;
					else if (tree.treeType.Value == Tree.mushroomTree || tree.treeType.Value == Tree.palmTree)
						return TreeTransplant.specialTreeTexture;
				}
				return TreeTransplant.helper.Reflection.GetField<Lazy<Texture2D>>(tree, "texture").GetValue().Value;
			}
		}

		public bool flipped
		{
			get => tree.flipped.Value;
			set => tree.flipped.Set(value);
		}

		public int treeType => tree.treeType.Value;

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

				// tree index refers to column in the generated texture, mahogany is hard coded as 4
				var treeIndex = tree.treeType.Value == Tree.mahoganyTree ? 4 : tree.treeType.Value;
				var basicTree = treeIndex >= 1 && treeIndex <= 4;
				// non-basic trees are palm and mushroom which are 6 and 7 so we subtract 6 to get 0 and 1
				var xOffset = treeIndex - (basicTree ? 1 : 6);
				var yOffset = basicTree ? Utility.getSeasonNumber(Game1.currentSeason) : 0;

				return new Rectangle(48 * xOffset, yOffset * 96, 48, 96);
			}
		}

		public Rectangle stumpSourceRect => Tree.stumpSourceRect;
		public Rectangle getBoundingBox(Vector2 tileLocation) => tree.getBoundingBox(tileLocation);
		public bool stump => tree.stump.Value;
		public bool isAdult() => tree.growthStage.Value > 4;
		public bool isStumpSeparate() => isAdult();
	}
}
