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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI.FlowNode {
	public struct NestedNode : IFlowNode {

		public IFlowNode[] Nodes { get; }
		public Alignment Alignment { get; }

		public bool NoComponent { get; }
		public Func<IFlowNodeSlice, bool> OnClick { get; }
		public Func<IFlowNodeSlice, bool> OnHover { get; }

		public NestedNode(IFlowNode[] nodes, Alignment? alignment = null, Func<IFlowNodeSlice, bool> onClick = null, Func<IFlowNodeSlice, bool> onHover = null, bool noComponent = false) {
			Nodes = nodes;
			Alignment = alignment ?? Alignment.None;
			NoComponent = noComponent;
			OnClick = onClick;
			OnHover = onHover;
		}

		public bool IsEmpty() {
			return Nodes == null || Nodes.Length == 0 || Nodes.All(val => val.IsEmpty());
		}

		public IFlowNodeSlice Slice(IFlowNodeSlice last, SpriteFont font, float maxWidth, float remaining) {
			IFlowNodeSlice previous = null;
			int index = 0;

			if (last is NestedNodeSlice tslice) {
				index = tslice.Index;
				previous = tslice.Slice;
			}

			while (index < Nodes.Length) {
				IFlowNodeSlice result = Nodes[index].Slice(previous, font, maxWidth, remaining);
				if (result != null)
					return new NestedNodeSlice(this, index, result);

				previous = null;
				index++;
			}

			return null;
		}

		public void Draw(IFlowNodeSlice slice, SpriteBatch batch, Vector2 position, float scale, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor, CachedFlowLine line, CachedFlow flow) {
			if (slice is NestedNodeSlice tslice)
				tslice.Slice.Node.Draw(tslice.Slice, batch, position, scale, defaultFont, defaultColor, defaultShadowColor, line, flow);
		}

		public override bool Equals(object obj) {
			return obj is NestedNode node &&
				   EqualityComparer<IFlowNode[]>.Default.Equals(Nodes, node.Nodes) &&
				   Alignment == node.Alignment &&
				   NoComponent == node.NoComponent &&
				   EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.Equals(OnClick, node.OnClick) &&
				   EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.Equals(OnHover, node.OnHover);
		}

		public override int GetHashCode() {
			int hashCode = 449514427;
			hashCode = hashCode * -1521134295 + EqualityComparer<IFlowNode[]>.Default.GetHashCode(Nodes);
			hashCode = hashCode * -1521134295 + Alignment.GetHashCode();
			hashCode = hashCode * -1521134295 + NoComponent.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.GetHashCode(OnClick);
			hashCode = hashCode * -1521134295 + EqualityComparer<Func<IFlowNodeSlice, bool>>.Default.GetHashCode(OnHover);
			return hashCode;
		}
	}

	public struct NestedNodeSlice : IFlowNodeSlice {

		public NestedNode TNode { get; }
		public IFlowNode Node { get => TNode; }

		public float Width { get => Slice.Width; }
		public float Height { get => Slice.Height; }
		public WrapMode ForceWrap { get => Slice.ForceWrap; }

		public int Index { get; }
		public IFlowNodeSlice Slice { get; }

		public NestedNodeSlice(NestedNode node, int index, IFlowNodeSlice slice) {
			TNode = node;
			Index = index;
			Slice = slice;
		}

		public bool IsEmpty() {
			return Slice.IsEmpty();
		}

		public override bool Equals(object obj) {
			return obj is NestedNodeSlice slice &&
				   EqualityComparer<NestedNode>.Default.Equals(TNode, slice.TNode) &&
				   EqualityComparer<IFlowNode>.Default.Equals(Node, slice.Node) &&
				   Width == slice.Width &&
				   Height == slice.Height &&
				   ForceWrap == slice.ForceWrap &&
				   Index == slice.Index &&
				   EqualityComparer<IFlowNodeSlice>.Default.Equals(Slice, slice.Slice);
		}

		public override int GetHashCode() {
			int hashCode = -493577651;
			hashCode = hashCode * -1521134295 + TNode.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<IFlowNode>.Default.GetHashCode(Node);
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + ForceWrap.GetHashCode();
			hashCode = hashCode * -1521134295 + Index.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<IFlowNodeSlice>.Default.GetHashCode(Slice);
			return hashCode;
		}
	}
}
