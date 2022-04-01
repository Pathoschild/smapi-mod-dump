/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI.SimpleLayout {
	public class FlowNode : ISimpleNode {

		public Alignment Alignment { get; }
		public IEnumerable<IFlowNode> Nodes { get; }
		public bool WrapText { get; }
		public float MinWidth { get; }

		private CachedFlow? Flow;

		public FlowNode(IEnumerable<IFlowNode> nodes, bool wrapText = true, float minWidth = -1, Alignment alignment = Alignment.None) {
			Nodes = nodes;
			WrapText = wrapText;
			MinWidth = minWidth;
			Alignment = alignment;
		}

		public bool DeferSize => WrapText;

		private CachedFlow GetFlow(SpriteFont font, float maxWidth) {
			if (Flow.HasValue)
				Flow = FlowHelper.CalculateFlow(Flow.Value, maxWidth, font);
			else
				Flow = FlowHelper.CalculateFlow(Nodes, maxWidth, font);

			return Flow.Value;
		}

		public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
			SpriteFont font = defaultFont ?? Game1.smallFont;
			CachedFlow flow = GetFlow(font, WrapText ? Math.Max(MinWidth, containerSize.X) : -1);

			Vector2 size = new(flow.Width, flow.Height);
			if (WrapText)
				size.X = Math.Max(MinWidth, containerSize.X);

			return size;
		}

		public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
			SpriteFont font = defaultFont ?? Game1.smallFont;
			CachedFlow flow = GetFlow(font, WrapText ? size.X : -1);

			FlowHelper.RenderFlow(batch, flow, position, defaultColor, defaultShadowColor, lineOffset: 0);
		}
	}
}
