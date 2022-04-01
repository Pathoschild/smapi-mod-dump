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

using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.FlowNode {
	public struct NestedNode : IFlowNode {

		public IFlowNode[] Nodes { get; }
		public Alignment Alignment { get; }
		public object Extra { get; }
		public string UniqueId { get; }

		public Func<IFlowNodeSlice, int, int, bool> OnClick { get; }
		public Func<IFlowNodeSlice, int, int, bool> OnHover { get; }
		public Func<IFlowNodeSlice, int, int, bool> OnRightClick { get; }

		public Func<IFlowNodeSlice, bool?> _wantComponent { get; }

		public NestedNode(
			IFlowNode[] nodes,
			Alignment? align = null,
			Func<IFlowNodeSlice, int, int, bool> onClick = null,
			Func<IFlowNodeSlice, int, int, bool> onHover = null,
			Func<IFlowNodeSlice, int, int, bool> onRightClick = null,
			Func<IFlowNodeSlice, bool?> wantComponent = null,
			object extra = null,
			string id = null
		) {
			Nodes = nodes;
			Alignment = align ?? Alignment.None;
			_wantComponent = wantComponent;
			OnClick = onClick;
			OnHover = onHover;
			OnRightClick = onRightClick;
			Extra = extra;
			UniqueId = id;
		}

		public bool? WantComponent(IFlowNodeSlice slice) {
			if (slice is NestedNodeSlice tslice)
				return tslice.Node.WantComponent(slice);

			return _wantComponent?.Invoke(slice);
		}

		public ClickableComponent UseComponent(IFlowNodeSlice slice) {
			if (slice is NestedNodeSlice tslice)
				return tslice.Node.UseComponent(slice);

			return null;
		}

		public bool IsEmpty() {
			return Nodes == null || Nodes.Length == 0 || Nodes.All(val => val.IsEmpty());
		}

		public IFlowNodeSlice Slice(IFlowNodeSlice last, SpriteFont font, float maxWidth, float remaining, IFlowNodeSlice nextSlice) {
			IFlowNodeSlice previous = null;
			int index = 0;

			if (last is NestedNodeSlice tslice) {
				index = tslice.Index;
				previous = tslice.Slice;
			}

			while (index < Nodes.Length) {
				IFlowNodeSlice ns = nextSlice;
				if (index + 1 < Nodes.Length)
					ns = Nodes[index + 1].Slice(null, font, 0f, 0f, null);

				IFlowNodeSlice result = Nodes[index].Slice(previous, font, maxWidth, remaining, ns);
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
				   EqualityComparer<object>.Default.Equals(Extra, node.Extra) &&
				   UniqueId == node.UniqueId &&
				   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnClick, node.OnClick) &&
				   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnHover, node.OnHover) &&
				   EqualityComparer<Func<IFlowNodeSlice, int, int, bool>>.Default.Equals(OnRightClick, node.OnRightClick) &&
				   EqualityComparer<Func<IFlowNodeSlice, bool?>>.Default.Equals(_wantComponent, node._wantComponent);
		}

		public override int GetHashCode() {
			return HashCode.Combine(Nodes, Alignment, Extra, UniqueId, OnClick, OnHover, OnRightClick, _wantComponent);
		}

		public static bool operator ==(NestedNode left, NestedNode right) {
			return left.Equals(right);
		}

		public static bool operator !=(NestedNode left, NestedNode right) {
			return !(left == right);
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
			return HashCode.Combine(TNode, Node, Width, Height, ForceWrap, Index, Slice);
		}

		public static bool operator ==(NestedNodeSlice left, NestedNodeSlice right) {
			return left.Equals(right);
		}

		public static bool operator !=(NestedNodeSlice left, NestedNodeSlice right) {
			return !(left == right);
		}
	}
}
