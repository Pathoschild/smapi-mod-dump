/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

internal sealed class ConcurrentLinkedListSlim<T> {
	private const MethodImplOptions Inline = Runtime.MethodImpl.Inline;

	private static class ExceptionMakers {
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static InvalidOperationException EmptyList() {
			return new("List is empty");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static ArgumentOutOfRangeException InvalidNodeRef(string nodeRefName, NodeRef value) {
			return new($"NodeRef {nodeRefName} is invalid ({value.Index})");
		}
	}

	[StructLayout(LayoutKind.Auto)]
	internal struct Node {
		internal T Value = default!;
		internal NodeRef Previous = default;
		internal NodeRef Next = default;

		[MethodImpl(Inline)]
		internal Node(in T value) {
			Value = value;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = sizeof(int), Size = sizeof(int))]
	internal readonly struct NodeRef {
		// This is 0 and not -1 because default initialization of `NodeRef` will always initialize this to 0 even if we specify -1.
		private readonly int IndexInternal = 0;

		internal readonly int Index => IndexInternal - 1;

		internal readonly bool IsValid => Index != -1;

		[MethodImpl(Inline)]
		internal NodeRef(int index) {
			index.AssertPositiveOrZero();
			IndexInternal = index + 1;
		}

		/// <summary>
		/// Sets <see cref="NodeRef.IndexInternal"/> to <c>0</c>
		/// </summary>
		[MethodImpl(Inline)]
		internal readonly void Invalidate() {
			Unsafe.AsRef(in IndexInternal) = 0;
		}

		[MethodImpl(Inline)]
		public static bool operator ==(NodeRef lhs, NodeRef rhs) => lhs.Index == rhs.Index;
		[MethodImpl(Inline)]
		public static bool operator !=(NodeRef lhs, NodeRef rhs) => lhs.Index != rhs.Index;

		[MethodImpl(Inline)]
		public readonly bool Equals(NodeRef other) => Index == other.Index;

		[MethodImpl(Inline)]
		public override readonly bool Equals(object? obj) => obj is NodeRef other && Equals(other);

		[MethodImpl(Inline)]
		public override int GetHashCode() => Index;
	}

	private readonly List<int> NodeFreeList = new();
	private Node[] NodeList = Array.Empty<Node>();
	private int NodeListOffset = 0;

	internal int Count { get; private set; } = 0;
	private NodeRef Head = default;
	private NodeRef Tail = default;

	internal Node? Last {
		[MethodImpl(Inline)]
		get {
			NodeRef tail = Tail;
			return tail.IsValid ? GetNode(tail) : null;
		}
	}

	[MethodImpl(Inline)]
	private static int AdjustSize(int neededSize) {
		var newSize = neededSize + (neededSize >> 1);
		newSize.AssertGreaterEqual(neededSize);
		return newSize;
	}

	[MethodImpl(Inline)]
	private ref Node GetNode(NodeRef nodeRef) {
		return ref NodeList[nodeRef.Index];
	}

	[MethodImpl(Inline)]
	private NodeRef GetNewNode() {
		if (NodeFreeList.Count != 0) {
			int freeIndex = NodeFreeList[^1];
			NodeFreeList.RemoveAt(NodeFreeList.Count - 1);
			ref var newNode = ref NodeList[freeIndex];
			newNode.Next = default;
			newNode.Previous = default;
			return new(freeIndex);
		}

		int offset = NodeListOffset++;

		if (offset >= NodeList.Length) {
			Array.Resize(ref NodeList, AdjustSize(NodeListOffset));
		}

		return new(offset);
	}

	internal NodeRef AddFront(in T value) {
		lock (this) {
			ValidateDebug();

			var newNodeRef = GetNewNode();
			ref Node newNode = ref GetNode(newNodeRef);
			newNode.Value = value;

			if (Head.IsValid) {
				ref Node currentHeadNode = ref GetNode(Head);
				newNode.Next = Head;
				currentHeadNode.Previous = newNodeRef;
			}

			Head = newNodeRef;

			if (!Tail.IsValid) {
				Tail = Head;
			}

			++Count;
			Count.AssertPositiveOrZero();

			ValidateDebug();

			return newNodeRef;
		}
	}

	internal void MoveToFront(NodeRef nodeRef) {
		if (!nodeRef.IsValid) {
			throw ExceptionMakers.InvalidNodeRef(nameof(nodeRef), nodeRef);
		}

		lock (this) {
			if (nodeRef == Head) {
				return;
			}

			CheckNode(nodeRef);

			ValidateDebug();

			ref Node node = ref GetNode(nodeRef);

			if (nodeRef == Tail) {
				Tail = node.Previous;
			}

			if (node.Previous.IsValid) {
				ref Node previous = ref GetNode(node.Previous);
				previous.Next = node.Next;
			}

			if (node.Next.IsValid) {
				ref Node next = ref GetNode(node.Next);
				next.Previous = node.Previous;
			}

			ref Node headNode = ref GetNode(Head);
			headNode.Previous = nodeRef;
			node.Previous = default;
			node.Next = Head;
			Head = nodeRef;

			ValidateDebug();
		}
	}

	internal T? Release(in NodeRef nodeRef) {
		if (!nodeRef.IsValid) {
			throw ExceptionMakers.InvalidNodeRef(nameof(nodeRef), nodeRef);
		}

		lock (this) {
			NodeRef nodeRefLocal = nodeRef;

			CheckNode(nodeRefLocal);

			ref Node node = ref GetNode(nodeRefLocal);

			T value = node.Value;
			node.Value = default!;

			if (node.Previous.IsValid) {
				ref Node previous = ref GetNode(node.Previous);
				previous.Next = node.Next;
			}

			if (node.Next.IsValid) {
				ref Node next = ref GetNode(node.Next);
				next.Previous = node.Previous;
			}

			if (nodeRefLocal == Head) {
				Head = node.Next;
			}

			if (nodeRefLocal == Tail) {
				Tail = node.Previous;
			}

			--Count;
			Count.AssertPositiveOrZero();
			NodeFreeList.Add(nodeRefLocal.Index);
			nodeRef.Invalidate();

			ValidateDebug();

			return value;
		}
	}

	[MethodImpl(Inline)]
	internal T RemoveLast() {
		lock (this) {
			NodeRef tail = Tail;
			CheckNode(tail);
			
			if (!tail.IsValid) {
				throw ExceptionMakers.EmptyList();
			}

			return Release(tail)!;
		}
	}

	[Conditional("DEBUG")]
	private void CheckNode(NodeRef nodeRef) {
		try {
			nodeRef.Index.AssertPositiveOrZero(
				$"{nameof(NodeRef)} expected to be valid but has invalid index '{nodeRef.Index}'"
			);
		}
		catch {
			Debugger.Break();
			throw;
		}
	}

	[Conditional("DEBUG")]
	private void ValidateDebug() {
		try {
			Count.AssertPositiveOrZero();

			int nodeListLength = NodeList.Length;

			var indexSet = new HashSet<int>(Count);

			// Forward Check
			{
				NodeRef current = Head;
				while (current != default) {
					CheckNode(current);

					ref var currentNode = ref GetNode(current);

					(currentNode.Previous.IsValid || current == Head).AssertTrue(
						$"Node has no previous link but is not {nameof(Head)}"
					);

					(currentNode.Next.IsValid || current == Tail).AssertTrue($"Node has no next link but is not {nameof(Tail)}");

					indexSet.Add(current.Index).AssertTrue(
						$"Cycle detected in {this.GetType().Name}: index '{current.Index}' seen multiple times"
					);

					current.Index.AssertPositiveOrZero($"Invalid index '{current.Index}' seen in linked list");

					current.Index.AssertLess(nodeListLength, $"Invalid index '{current.Index}' seen in linked list");

					current = currentNode.Next;
				}

				indexSet.Count.AssertEqual(
					Count, $"Active node count '{indexSet.Count}' does not match expected count '{Count}'"
				);
			}

			// Reverse Check
			{
				indexSet.Clear();

				NodeRef current = Tail;
				while (current != default) {
					CheckNode(current);

					ref var currentNode = ref GetNode(current);

					(currentNode.Previous.IsValid || current == Head).AssertTrue(
						$"Node has no previous link but is not {nameof(Head)}"
					);

					(currentNode.Next.IsValid || current == Tail).AssertTrue($"Node has no next link but is not {nameof(Tail)}");

					indexSet.Add(current.Index).AssertTrue(
						$"Cycle detected in {this.GetType().Name}: index '{current.Index}' seen multiple times"
					);

					current.Index.AssertPositiveOrZero($"Invalid index '{current.Index}' seen in linked list");

					current.Index.AssertLess(nodeListLength, $"Invalid index '{current.Index}' seen in linked list");

					current = currentNode.Previous;
				}

				indexSet.Count.AssertEqual(
					Count, $"Active node count '{indexSet.Count}' does not match expected count '{Count}'"
				);
			}

			NodeFreeList.Count.AssertEqual(NodeFreeList.Distinct().Count(), $"{nameof(NodeFreeList.Count)} != distinct");

			foreach (var freeIndex in NodeFreeList) {
				freeIndex.AssertPositiveOrZero($"Negative index '{freeIndex}' in {nameof(NodeFreeList)}");
			}
		}
		catch {
			Debugger.Break();
			throw;
		}
	}

	internal void Clear() {
		lock (this) {
			ValidateDebug();

			if (Count == 0) {
				return;
			}

			int nodeListLength = NodeList.Length;

			NodeFreeList.Clear();
			NodeFreeList.Capacity = Math.Max(NodeFreeList.Capacity, nodeListLength);
			for (int i = 0; i < nodeListLength; i++) {
				NodeFreeList.Add(i);
				NodeList[i].Value = default!;
#if DEBUG || DEVELOPMENT
				NodeList[i].Next = default;
				NodeList[i].Previous = default;
#endif
			}

			Head = default;
			Tail = default;
			Count = 0;

			ValidateDebug();
		}
	}
}
