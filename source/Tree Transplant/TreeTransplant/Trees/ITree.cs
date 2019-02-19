using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

namespace TreeTransplant
{
	public interface ITree
	{
		TerrainFeature getTerrainFeature();
		Texture2D texture { get; }
		bool flipped { get; set; }
		Rectangle treeTopSourceRect { get; }
		Rectangle stumpSourceRect { get; }
		Rectangle getBoundingBox(Vector2 tileLocation);
		int treeType { get; }
		bool isAdult();
		bool stump { get; }
		bool isStumpSeparate();
	}
}
