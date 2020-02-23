using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Scalers {
	internal static class ScaleSize {
		private static readonly IScaler[] Scalers =
		{
			new Scaler2X(),
			new Scaler3X(),
			new Scaler4X(),
			new Scaler5X(),
			new Scaler6X()
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IScaler ToIScaler (this uint scaleSize) {
			// MJY: Need value checks to assure scaleSize is between 2-5 inclusive.
			return Scalers.Single(s => s.Scale == scaleSize);
		}
	}
}
