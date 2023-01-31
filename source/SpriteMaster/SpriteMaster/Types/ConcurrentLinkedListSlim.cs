/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using Microsoft.Toolkit.HighPerformance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Types;

internal sealed class ConcurrentLinkedListSlim<T> {
	private const MethodImplOptions Inline = Runtime.MethodImpl.Inline;

	//[StructLayout(LayoutKind.Auto)]
	internal struct Node {
		internal T Value = default!;
		internal NodeRef Previous = default;
		internal NodeRef Next = default;

		[MethodImpl(Inline)]
		public Node() {}
	}

	[StructLayout(LayoutKind.Sequential, Pack = sizeof(int), Size = sizeof(int))]
	internal readonly struct NodeRef {
#if VALIDATE_CLLSLIM
		private readonly ConcurrentLinkedListSlim<T> Owner;
#endif

		// This is 0 and not -1 because default initialization of `NodeRef` will always initialize this to 0 even if we specify -1.
		private readonly int IndexInternal = 0;

		internal readonly int Index => IndexInternal - 1;

		internal readonly bool IsValid => Index != -1;

		[Conditional("VALIDATE_CLLSLIM")]
		internal void ValidateOwner(ConcurrentLinkedListSlim<T> @this) {
#if VALIDATE_CLLSLIM
			if (@this != Owner) {
				Debug.Break();
			}

			if (Index > @this.NodeListOffset) {
				Debug.Break();
			}
#endif
		}

		[MethodImpl(Inline)]
#if VALIDATE_CLLSLIM
		internal NodeRef(int index, ConcurrentLinkedListSlim<T> owner) {
			Owner = owner;
#else
		internal NodeRef(int index) {
#endif
			index.AssertPositiveOrZero();
			IndexInternal = index + 1;
		}

		[MethodImpl(Inline)]
		public static implicit operator bool(NodeRef node) => node.IsValid;

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

	[StructLayout(LayoutKind.Auto)]
	private ref struct NodePair {
		private readonly Ref<Node> RefNode;
		internal ref Node Node => ref RefNode.Value;
		internal readonly NodeRef Ref;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal NodePair(ref Node node, NodeRef nodeRef) {
			RefNode = new(ref node);
			Ref = nodeRef;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator NodeRef(in NodePair pair) => pair.Ref;
	}

	private readonly List<int> NodeFreeList = new();
	private Node[] NodeArray = Array.Empty<Node>();
	private int NodeListOffset = -1;

	internal int Count { get; private set; } = 0;
	private NodeRef Head = default;
	private NodeRef Tail = default;

	// Only for debugging purposes
	private ref Node HeadNode => ref GetNode(Head);
	private ref Node TailNode => ref GetNode(Tail);

	[Conditional("VALIDATE_CLLSLIM")]
	private void CheckCycle() {
		HashSet<NodeRef> nodes = new(Count);

		var current = Head;
		while (current) {
			if (!nodes.Add(current)) {
				Debug.Break();
			}

			ref var node = ref GetNode(current);
			current = node.Next;
		}
	}

	[Conditional("VALIDATE_CLLSLIM")]
	private void CheckCycle(NodeRef nodeRef) {
		var current = Head;
		while (current) {
			if (current == nodeRef) {
				Debug.Break();
			}

			ref var node = ref GetNode(current);
			current = node.Next;
		}
	}

	private int GetListCount() {
		int count = 0;
		var current = Head;
		while (current) {
			ref var node = ref GetNode(current);
			current = node.Next;
			++count;
		}

		return count;
	}

	internal Node? Last {
		[MethodImpl(Inline)]
		get {
			NodeRef tail = Tail;
			return tail.IsValid ? GetNode(tail) : null;
		}
	}

	[Pure, MustUseReturnValue, MethodImpl(Inline)]
	private static int AdjustSize(int neededSize) {
		var newSize = neededSize + (neededSize >> 1);
		newSize.AssertGreaterEqual(neededSize);
		return newSize;
	}

	[Pure, MustUseReturnValue, MethodImpl(Inline)]
	private ref Node GetNode(NodeRef nodeRef) {
#if SHIPPING && !CONTRACTS_FULL
		return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(NodeArray), nodeRef.Index);
#else
		return ref NodeArray[nodeRef.Index];
#endif
	}

	[Pure, MustUseReturnValue, MethodImpl(Inline)]
	private bool TryGetNode(NodeRef nodeRef, out NodePair node) {
		if (nodeRef) {
			node = new(ref GetNode(nodeRef), nodeRef);
			return true;
		}

		node = default;
		return false;
	}

	/// <summary>
	/// Return a new, clean NodeRef from the array.
	/// As it can enlarge the array, it must be called _before_ any references are taken internally.
	/// </summary>
	[MustUseReturnValue, MethodImpl(Inline)]
	private NodePair GetNewNode() {
		NodePair nodePair;

		// If the NodeFreeList is not empty, then simply pull one out of it.
		if (NodeFreeList.Count != 0) {
			// Pop the last element off of the free list.
			int lastIndex = NodeFreeList.Count - 1;
			int freeIndex = NodeFreeList[lastIndex];
			NodeFreeList.RemoveAt(lastIndex);

			// Get the referenced node at that index.
			ref var newNode = ref NodeArray[freeIndex];
			// Clear it out.
			newNode.Next = default;
			newNode.Previous = default;
#if VALIDATE_CLLSLIM
			nodePair = new(ref newNode, new(freeIndex, this));
#else
			nodePair = new(ref newNode, new(freeIndex));
#endif
			CheckCycle(nodePair.Ref);

			nodePair.Ref.ValidateOwner(this);
		}
		else {
			// Otherwise, we need a new node.
			int offset = Interlocked.Increment(ref NodeListOffset);

			// If the new offset is larger than the current array, enlarge the array.
			if (offset >= NodeArray.Length) {
				Array.Resize(ref NodeArray, AdjustSize(offset + 1));
			}

#if VALIDATE_CLLSLIM
			nodePair = new(ref NodeArray[offset], new(offset, this));
#else
			nodePair = new(ref NodeArray[offset], new(offset));
#endif
			CheckCycle(nodePair.Ref);
		}

		return nodePair;
	}

	/// <summary>
	/// Add a new node representing the provided <paramref name="value"/> to the front of the list.
	/// </summary>
	internal NodeRef AddFront(in T value) {
		lock (this) {
			ValidateDebug();

			// Get a new node
			var newNode = GetNewNode();
			newNode.Node.Value = value;

			// If a head node exists, make the head node the next node to the new node,
			// and make the previous node to the head node the new node.
			if (TryGetNode(Head, out var headNode)) {
				newNode.Node.Next = headNode;
				headNode.Node.Previous = newNode;
			}

			// Make the new node the head node.
			Head = newNode;

			// If no tail node exists, then head and tail are the same node.
			if (!Tail) {
				Tail = Head;
			}

			// Increment and validate the current node count.
			++Count;
			Count.AssertPositiveOrZero();
			ValidateDebug();
#if (!SHIPPING || CONTRACTS_FULL) && !DEVELOPMENT
			Count.AssertEqual(GetListCount());
#endif

			return newNode;
		}
	}

	/// <summary>
	/// Moves the given <paramref name="nodeRef">node</paramref> to the front of the list.
	/// </summary>
	internal void MoveToFront(NodeRef nodeRef) {
		// If the node is invalid, then do nothing.
		if (!nodeRef) {
			return;
		}

		nodeRef.ValidateOwner(this);

		lock (this) {
			// If the node is invalid, then do nothing.
			if (!nodeRef) {
				return;
			}

			var headRef = Head;

			// If it is already the head node, then there is nothing to do.
			if (nodeRef == headRef) {
				return;
			}

			CheckNode(nodeRef);

			ValidateDebug();

			ref Node node = ref GetNode(nodeRef);

			// Remove the node from the list
			{
				// If the node is the Tail node, then move the previous node to the Tail.
				if (nodeRef == Tail) {
					Tail = node.Previous;
				}

				// Update the previous node.
				if (TryGetNode(node.Previous, out var previous)) {
					previous.Node.Next = node.Next;
				}

				// Update the next node.
				if (TryGetNode(node.Next, out var next)) {
					next.Node.Previous = node.Previous;
				}
			}

			// Insert the node at Head.
			ref Node head = ref GetNode(headRef);
			head.Previous = nodeRef;
			node.Previous = default;
			node.Next = headRef;
			Head = nodeRef;

			ValidateDebug();
		}
	}

	/// <summary>
	/// Remove the given <paramref name="nodeRef">node</paramref> from the list
	/// </summary>
	internal T? Release(ref NodeRef nodeRef) {
		// If the node is invalid, then do nothing.
		if (!nodeRef) {
			return default;
		}

		nodeRef.ValidateOwner(this);

		lock (this) {
			NodeRef nodeRefLocal = nodeRef;

			// If the node is invalid, then do nothing
			if (!nodeRefLocal) {
				return default;
			}

			CheckNode(nodeRefLocal);

			ref Node node = ref GetNode(nodeRefLocal);

#if (!SHIPPING || CONTRACTS_FULL) && !DEVELOPMENT
			if (nodeRef != Tail && nodeRef != Head && node.Previous == default && node.Next == default) {
				ThrowHelper.ThrowInvalidOperationException("Dead Node being released");
			}
#endif

			// Cache the current stored value, and clear it out of the node.
			T value = node.Value;
			node.Value = default!;

			if (nodeRefLocal == Head) {
				Head = node.Next;
			}

			if (nodeRefLocal == Tail) {
				Tail = node.Previous;
			}

			// Remove the node from the list.
			if (TryGetNode(node.Previous, out var previous)) {
				previous.Node.Next = node.Next;
			}

			if (TryGetNode(node.Next, out var next)) {
				next.Node.Previous = node.Previous;
			}

			// Decrement and validate the current node count.
			--Count;
			CheckCycle(nodeRefLocal);
			nodeRefLocal.ValidateOwner(this);
			// Add the index to the node free list.
			NodeFreeList.Add(nodeRefLocal.Index);
			// Clear out the index.
			nodeRef = default;

			Count.AssertPositiveOrZero();
			ValidateDebug();
#if (!SHIPPING || CONTRACTS_FULL) && !DEVELOPMENT
			Count.AssertEqual(GetListCount());
#endif

			return value;
		}
	}

	[MethodImpl(Inline)]
	internal T RemoveLast() {
		lock (this) {
			// Not copied by ref as 'Release' will update 'Tail' and we do not want to inadvertently clobber the result.
			NodeRef tail = Tail;

			CheckNode(tail);

			if (!tail) {
				ThrowHelper.ThrowInvalidOperationException("List is empty");
			}

			return Release(ref tail)!;
		}
	}

	[MethodImpl(Inline)]
	internal bool TryRemoveLast([NotNullWhen(true)] out T? value) {
		lock (this) {
			// Not copied by ref as 'Release' will update 'Tail' and we do not want to inadvertently clobber the result.
			NodeRef tail = Tail;

			if (!tail) {
				value = default;
				return false;
			}

			value = Release(ref tail)!;
			return true;
		}
	}

	[Conditional("VALIDATE_CLLSLIM")]
	private void CheckNode(NodeRef nodeRef) {
		nodeRef.ValidateOwner(this);

		try {
			nodeRef.Index.AssertPositiveOrZero(
				$"{nameof(NodeRef)} expected to be valid but has invalid index '{nodeRef.Index}'"
			);
		}
		catch {
			Debug.Break();
			throw;
		}
	}

	[Conditional("VALIDATE_CLLSLIM")]
	private void ValidateDebug() {
		try {
			Count.AssertPositiveOrZero();

			int nodeListLength = NodeArray.Length;

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
			Debug.Break();
			throw;
		}
	}

	// Removes all nodes from the list.
	// All outstanding NodeRefs will need to be cleared as well as they are no longer valid
	internal void Clear() {
		lock (this) {
			ValidateDebug();

			if (Count == 0) {
				return;
			}

			int nodeListLength = NodeArray.Length;

			NodeFreeList.Clear();
			NodeFreeList.Capacity = Math.Max(NodeFreeList.Capacity, nodeListLength);
			for (int i = 0; i < nodeListLength; i++) {
				NodeFreeList.Add(i);
				ref Node node = ref NodeArray[i];
				node.Value = default!;
#if (!SHIPPING || CONTRACTS_FULL) && !DEVELOPMENT
				node.Next = default;
				node.Previous = default;
#endif
			}

			Head = default;
			Tail = default;
			Count = 0;

			NodeListOffset = nodeListLength;

			ValidateDebug();
		}
	}
}
