/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.CpuIdParser;
internal sealed class GenericParser {
	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	internal static SystemInfo.ArchitectureResult Parse(in SystemInfo.CpuId id) {
		// I don't know what to do in this situation
		return new(
			"Unknown",
			new() { Avx2 = false, Avx512 = false }
		);
	}
}
