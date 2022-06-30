/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

internal static class CompareToFastExt {
	#region bool
	// .NET implementation
	/*
				if (this == value)
					return 0;
				return this ? 1 : -1;
	*/

	/// <summary>
	/// Compares two boolean values.
	/// <para>
	/// <c>
	/// (a == b) ? 0 : a ? 1 : -1
	/// </c>
	/// </para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns><c>0</c> if <paramref name="a"/> and <paramref name="b"/> are equal, else <c>1</c> if <paramref name="a"/> is true, otherwise <c>-1</c></returns>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompareToFast(this bool a, bool b) {
		int aValue = a.ReinterpretAs<byte>();
		int bValue = b.ReinterpretAs<byte>();
		return aValue - bValue;
	}

	#endregion
}

internal static class EqualsFastExt {
}