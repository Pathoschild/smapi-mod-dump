/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace AutoTrasher.Components.Elements
{
	internal class ReclaimOptionsButton<TButton> : BaseOptionsElement
		where TButton : ReclaimOptionsButton<TButton>
	{
		private readonly Action<TButton> _toggle;
		private readonly Rectangle _reclaimButtonSprite;
		private Rectangle _reclaimButtonBounds;

		public ReclaimOptionsButton(string label, int slotWidth, Action<TButton> toggle, bool disabled = false)
			: base(label, -1, -1, slotWidth + 1, 15 * Game1.pixelZoom)
		{
			_toggle = toggle;
			greyedOut = disabled;

			_reclaimButtonSprite = new(67, 243, 9, 10);
			_reclaimButtonBounds = new(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 9 * Game1.pixelZoom, 10 * Game1.pixelZoom);
		}

		public ReclaimOptionsButton(string label, int slotWidth, Action toggle, bool disabled = false)
			: this(label, slotWidth, _ => toggle(), disabled)
		{ }

		public override void receiveLeftClick(int x, int y)
		{
			if (greyedOut || !_reclaimButtonBounds.Contains(x, y)) return;

			_toggle((TButton)this);
		}

		public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
		{
			DrawElement(b, slotX, slotY, context);
			Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2(_reclaimButtonBounds.X + slotX, _reclaimButtonBounds.Y + slotY), _reclaimButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
		}

		protected virtual void DrawElement(SpriteBatch b, int slotX, int slotY, IClickableMenu? context = null)
		{
			Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.15f);
		}
	}

	internal class ReclaimOptionsButton : ReclaimOptionsButton<ReclaimOptionsButton>
	{
		public ReclaimOptionsButton(string label, int slotWidth, Action<ReclaimOptionsButton> toggle, bool disabled = false)
			: base(label, slotWidth, toggle, disabled)
		{ }

		public ReclaimOptionsButton(string label, int slotWidth, Action toggle, bool disabled = false)
			: base(label, slotWidth, _ => toggle(), disabled)
		{ }
	}
}
