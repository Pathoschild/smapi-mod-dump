/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.xBRZ.Common {
	internal static class ColorConstant {
		internal static class Shift {
			public const int Alpha = 24;
			public const int Red = 0;
			public const int Green = 8;
			public const int Blue = 16;
		}

		internal static class Mask {
			public const uint Alpha = (0xFFU << Shift.Alpha);
			public const uint Red = (0xFFU << Shift.Red);
			public const uint Green = (0xFFU << Shift.Green);
			public const uint Blue = (0xFFU << Shift.Blue);
		}
	}
}
