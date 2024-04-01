/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Diagnostics;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoProperty

namespace SpriteMaster.Types.Spans;

internal sealed class PinnedSpanDebugView<T> where T : unmanaged {
	private readonly T[] _array;

	public PinnedSpanDebugView(PinnedSpan<T> span) {
		_array = span.ToArray();
	}

	public PinnedSpanDebugView(ReadOnlyPinnedSpan<T> span) {
		_array = span.ToArray();
	}

	public PinnedSpanDebugView(PinnedSpan<T>.FixedSpan span) : this(span.AsSpan) {
	}

	public PinnedSpanDebugView(ReadOnlyPinnedSpan<T>.FixedSpan span) : this(span.AsSpan) {
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public T[] Items => _array;
}
