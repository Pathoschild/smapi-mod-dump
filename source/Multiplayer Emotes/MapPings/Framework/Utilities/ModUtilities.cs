using Microsoft.Xna.Framework;

namespace MapPings.Framework.Utilities {

	public static class ModUtilities {

		public static Color GetColorFromName(string name) {

			switch(name) {
				case "aqua":
					return Color.MediumTurquoise;
				case "blue":
					return Color.DodgerBlue;
				case "brown":
					return new Color(160, 80, 30);
				case "cream":
					return new Color(byte.MaxValue, byte.MaxValue, 180);
				case "gray":
					return Color.Gray;
				case "green":
					return new Color(0, 180, 10);
				case "jade":
					return new Color(50, 230, 150);
				case "jungle":
					return Color.SeaGreen;
				case "orange":
					return new Color(byte.MaxValue, 100, 0);
				case "peach":
					return new Color(byte.MaxValue, 180, 120);
				case "pink":
					return Color.HotPink;
				case "plum":
					return new Color(190, 0, 190);
				case "purple":
					return new Color(138, 43, 250);
				case "red":
					return new Color(220, 20, 20);
				case "salmon":
					return Color.Salmon;
				case "yellow":
					return new Color(240, 200, 0);
				case "yellowgreen":
					return new Color(182, 214, 0);
				default:
					return Color.White;
			}

		}

	}

}
