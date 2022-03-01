/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.SimpleLayout {
	public class SpaceNode : ISimpleNode {

		private readonly LayoutNode Parent;
		public float Size { get; }
		public bool Expand { get; }

		public Alignment Alignment => Alignment.None;

		public bool DeferSize => false;

		public SpaceNode(LayoutNode parent, bool expand = true, float size = 16) {
			Parent = parent;
			Expand = expand;
			Size = size;
		}

		public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
			return Parent.Direction switch {
				LayoutDirection.Horizontal => new Vector2(Size, 0),
				_ => new Vector2(0, Size)
			};
		}

		public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
			/* spaces don't draw ~ */
		}
	}
}
