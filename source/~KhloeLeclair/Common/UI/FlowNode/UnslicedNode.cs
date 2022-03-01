/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Leclair.Stardew.Common.UI.FlowNode {
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

		public override bool Equals(object obj) {
			return obj is UnslicedNode node &&
				   EqualityComparer<IFlowNode>.Default.Equals(Node, node.Node) &&
				   Width == node.Width &&
				   Height == node.Height &&
				   ForceWrap == node.ForceWrap;
		}

		public override int GetHashCode() {
			int hashCode = 1723241078;
			hashCode = hashCode * -1521134295 + EqualityComparer<IFlowNode>.Default.GetHashCode(Node);
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + ForceWrap.GetHashCode();
			return hashCode;
		}
	}
}
