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

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct CachedFlowLine {

	public IFlowNodeSlice[] Slices;

	public float Width { get; }
	public float Height { get; }

	public CachedFlowLine(IFlowNodeSlice[] slices, float width, float height) {
		Slices = slices;
		Width = width;
		Height = height;
	}
}

#endif
