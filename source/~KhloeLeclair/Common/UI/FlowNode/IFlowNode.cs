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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.FlowNode {
	public interface IFlowNode {

		bool IsEmpty();

		Alignment Alignment { get; }

		IFlowNodeSlice Slice(IFlowNodeSlice last, SpriteFont font, float maxWidth, float remaining);

		void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow);

		// Interaction
		bool NoComponent { get; }
		Func<IFlowNodeSlice, bool> OnHover { get; }
		Func<IFlowNodeSlice, bool> OnClick { get; }

	}
}
