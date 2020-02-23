using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpriteMaster.Types {
	internal static class SurfaceFormatExt {
		internal static bool HasSurfaceFormat (string name) {
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
		internal static SurfaceFormat GetSurfaceFormat (string name) {
			var EnumType = typeof(SurfaceFormat);
			var Enums = EnumType.GetFields();
			var largestValue = (SurfaceFormat)0;
			foreach (var enumerator in Enums) {
				if (enumerator.FieldType != EnumType.UnderlyingSystemType) {
					continue;
				}
				var enumValue = (SurfaceFormat)Convert.ChangeType(enumerator.GetValue(EnumType), typeof(SurfaceFormat));
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
}
