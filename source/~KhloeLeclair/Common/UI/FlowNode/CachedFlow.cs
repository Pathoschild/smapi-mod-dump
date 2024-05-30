/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_FLOW

using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct CachedFlow {

	public IFlowNode[] Nodes { get; }
	public CachedFlowLine[] Lines { get; }

	public float Width { get; }
	public float Height { get; }

	public SpriteFont Font { get; }
	public float MaxWidth { get; }

	public CachedFlow(IFlowNode[] nodes, CachedFlowLine[] lines, float width, float height, SpriteFont font, float maxWidth) {
		Nodes = nodes;
		Lines = lines;
		Width = width;
		Height = height;

		Font = font;
		MaxWidth = maxWidth;
	}

	public bool IsCached(SpriteFont font, float maxWidth) {
		return Font == font && maxWidth == MaxWidth;
	}
}

#endif
