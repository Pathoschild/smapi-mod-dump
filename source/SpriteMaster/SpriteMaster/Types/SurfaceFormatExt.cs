/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpriteMaster.Types;
static class SurfaceFormatExt {
	internal static bool HasSurfaceFormat(string name) {
		var EnumType = typeof(SurfaceFormat);
		var Enums = EnumType.GetFields();
		foreach (var enumerator in Enums) {
			if (name == enumerator.Name) {
				return true;
			}
		}
		return false;
	}

	private static int LastArtificialSurfaceFormat = -1;
	internal static SurfaceFormat? GetSurfaceFormat(string name) {
		var EnumType = typeof(SurfaceFormat);
		var Enums = EnumType.GetFields();
		var largestValue = (SurfaceFormat)0;
		foreach (var enumerator in Enums) {
			if (enumerator.FieldType != EnumType.UnderlyingSystemType) {
				continue;
			}
			var enumValue = (SurfaceFormat?)Convert.ChangeType(enumerator.GetValue(EnumType), typeof(SurfaceFormat));
			if (enumValue is null) {
				return null;
			}
			if (name == enumerator.Name) {
				return enumValue;
			}
			largestValue = (SurfaceFormat)Math.Max((int)enumValue, (int)largestValue);
		}
		// Return the 'next' enumerator value, so we can potentially inject it later
		if (LastArtificialSurfaceFormat == -1) {
			LastArtificialSurfaceFormat = (int)largestValue + 1;
			return (SurfaceFormat)LastArtificialSurfaceFormat;

		}
		else {
			return (SurfaceFormat)(++LastArtificialSurfaceFormat);
		}
	}
}
