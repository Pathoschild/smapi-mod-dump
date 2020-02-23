using System.Collections.Generic;
using System.IO;

namespace BlueberryMushroomMachine
{
	internal class Const
	{
		// Project strings
		internal const string AuthorName
			= "blueberry";
		internal const string PackageName
			= "BlueberryMushroomMachine";
		internal const string PropagatorName
			= "Propagator";

		internal static readonly string PropagatorUniqueId
			= $"{PackageName}.{PropagatorName}";

		// Paths
		internal static readonly string MachinePath
			= Path.Combine("Assets", "propagator.png");
		internal static readonly string OverlayPath
			= Path.Combine("Assets", "overlay.png");
		internal static readonly string EventsPath
			= Path.Combine("Assets", "events.json");

		// Object data
		internal static Dictionary<int, int> MushroomSourceRects =
			new Dictionary<int, int> {
				{257, 0},		// Morel
				{281, 1},		// Chantarelle
				{404, 2},		// Common Mushroom
				{420, 3},		// Red Mushroom
				{422, 4},		// Purple Mushroom
			};

		internal static Dictionary<int, float> MushroomGrowingRates =
			new Dictionary<int, float> {
				{257, 0.5f},	// Morel
				{281, 0.5f},	// Chantarelle
				{404, 1.0f},	// Common Mushroom
				{420, 0.5f},	// Red Mushroom
				{422, 0.25f},	// Purple Mushroom
			};

		internal static Dictionary<int, int> MushroomQuantityLimits =
			new Dictionary<int, int> {
				{257, 4},		// Morel
				{281, 4},		// Chantarelle
				{404, 6},		// Common Mushroom
				{420, 3},		// Red Mushroom
				{422, 2},		// Purple Mushroom
			};
	}
}
