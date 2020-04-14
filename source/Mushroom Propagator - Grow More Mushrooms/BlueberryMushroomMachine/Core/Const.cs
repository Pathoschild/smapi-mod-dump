using System.Collections.Generic;
using System.IO;

namespace BlueberryMushroomMachine
{
	internal class Const
	{
		// Project
		internal const string AuthorName
			= "blueberry";
		internal const string PackageName
			= "BlueberryMushroomMachine";

		// Files
		internal static readonly string MachinePath
			= Path.Combine("assets", "propagator.png");
		internal static readonly string OverlayPath
			= Path.Combine("assets", "overlay.png");
		internal static readonly string EventsPath
			= Path.Combine("assets", "events.json");

		// Mushroom Machine
		internal static readonly string PropagatorInternalName
			= $"{PackageName}.Propagator";

		// Objects
		private enum Mushrooms
		{
			Morel = 257,
			Chantarelle = 281,
			Common = 404,
			Red = 420,
			Purple = 422
		}

		internal static readonly Dictionary<int, int> MushroomSourceRects =
			new Dictionary<int, int> {
				{(int)Mushrooms.Morel, 0},
				{(int)Mushrooms.Chantarelle, 1},
				{(int)Mushrooms.Common, 2},
				{(int)Mushrooms.Red, 3},
				{(int)Mushrooms.Purple, 4}
			};

		internal static readonly Dictionary<int, float> MushroomGrowingRates =
			new Dictionary<int, float> {
				{(int)Mushrooms.Morel, 0.5f},
				{(int)Mushrooms.Chantarelle, 0.5f},
				{(int)Mushrooms.Common, 1.0f},
				{(int)Mushrooms.Red, 0.5f},
				{(int)Mushrooms.Purple, 0.25f}
			};

		internal static readonly Dictionary<int, int> MushroomQuantityLimits =
			new Dictionary<int, int> {
				{(int)Mushrooms.Morel, 4},
				{(int)Mushrooms.Chantarelle, 4},
				{(int)Mushrooms.Common, 6},
				{(int)Mushrooms.Red, 3},
				{(int)Mushrooms.Purple, 2}
			};
	}
}
