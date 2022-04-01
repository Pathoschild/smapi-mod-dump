/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Extensions;

static class Reinterpret {
	internal static unsafe U ReinterpretAs<T, U>(this T value) where T : unmanaged where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(T));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this bool value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(bool));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this byte value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(byte));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this sbyte value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(sbyte));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this ushort value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(ushort));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this short value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(short));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this uint value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(uint));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this int value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(int));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this ulong value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(ulong));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this long value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(long));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this half value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(half));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this float value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(float));
		return *(U*)&value;
	}

	internal static unsafe U ReinterpretAs<U>(this double value) where U : unmanaged {
		Contracts.AssertLessEqual(sizeof(U), sizeof(double));
		return *(U*)&value;
	}
}
