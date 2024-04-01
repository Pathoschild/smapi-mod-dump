/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmarks.Strings.Benchmarks.Sources;

public abstract class RandomText : StringSource {
	private const int RandSeed = 0x13377113;
	private static readonly long MinSize = 0;
	private static readonly long MaxSize = 4096;

	private static ReadOnlySpan<char> CharsRef => new[] {
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
		'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
		'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
		'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
		'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
	};

	private static readonly char[] CharsArray;
	private static readonly unsafe char* Chars;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe char RandomChar(Random rand) {
		return Chars[rand.Next(0, CharsArray.Length)];
	}

	static RandomText() {
		unsafe {
			var charsRef = CharsRef;
			CharsArray = GC.AllocateUninitializedArray<char>(charsRef.Length, pinned: true);
			charsRef.CopyTo(CharsArray.AsSpan());
			Chars = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(CharsArray));
		}

		var random = new Random(RandSeed);

		List<string> strings = new();

		var min = MinSize;

		if (min == 0 && min != MaxSize) {
			strings.Add("");

			min = 1;
		}

		for (var i = min; i <= MaxSize; i *= 2) {
			var sb = new StringBuilder((int)i);
			for (var j = 0; j < i; ++j) {
				sb.Append(RandomChar(random));
			}

			strings.Add(sb.ToString());
		}

		if (DefaultDataSetsStatic.Last().Data.Length != MaxSize) {
			var sb = new StringBuilder((int)MaxSize);
			for (var j = 0; j < MaxSize; ++j) {
				sb.Append(RandomChar(random));
			}

			strings.Add(sb.ToString());
		}

		AddSet(random, strings);
	}
}
