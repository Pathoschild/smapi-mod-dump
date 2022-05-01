/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

namespace Leclair.Stardew.Common.UI.FlowNode;

public struct UnslicedNode : IFlowNodeSlice {

	public IFlowNode Node { get; }
	public float Width { get; }
	public float Height { get; }
	public WrapMode ForceWrap { get; }

	public UnslicedNode(IFlowNode node, float width, float height, WrapMode forceWrap) {
		Node = node;
		Width = width;
		Height = height;
		ForceWrap = forceWrap;
	}

	public bool IsEmpty() {
		return Node.IsEmpty();
	}

	public override bool Equals(object? obj) {
		return obj is UnslicedNode node &&
			   EqualityComparer<IFlowNode>.Default.Equals(Node, node.Node) &&
			   Width == node.Width &&
			   Height == node.Height &&
			   ForceWrap == node.ForceWrap;
	}

	public override int GetHashCode() {
		return HashCode.Combine(Node, Width, Height, ForceWrap);
	}

	public static bool operator ==(UnslicedNode left, UnslicedNode right) {
		return left.Equals(right);
	}

	public static bool operator !=(UnslicedNode left, UnslicedNode right) {
		return !(left == right);
	}
}
