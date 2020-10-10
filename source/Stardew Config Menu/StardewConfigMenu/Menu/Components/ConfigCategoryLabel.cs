/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework.Options;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace StardewConfigMenu.Components {

	sealed class ConfigCategoryLabel: SCMControl {
		private readonly IConfigHeader ModData;
		public sealed override string Label => ModData.Label;

		private Rectangle Bounds = new Rectangle();
		public sealed override int X { get => Bounds.X; set => Bounds.X = value; }
		public sealed override int Y { get => Bounds.Y; set => Bounds.Y = value; }
		public sealed override int Height => SpriteText.getHeightOfString(Label);
		public sealed override int Width => SpriteText.getWidthOfString(Label);

		public sealed override bool Enabled => true;

		public ConfigCategoryLabel(IConfigHeader option) : this(option, 0, 0) { }

		public ConfigCategoryLabel(IConfigHeader option, int x, int y) : base(option.Label) {
			ModData = option;
			X = x;
			Y = y;
		}

		public override void Draw(SpriteBatch b) {
			SpriteText.drawString(b, Label, X, Y - 4 * Game1.pixelZoom);
		}
	}
}
