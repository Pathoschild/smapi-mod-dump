/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System.IO;

namespace BlueberryMushroomMachine
{
	public class ModValues
	{
		// Project
		public const string AuthorName
			= "blueberry";
		public const string PackageName
			= "BlueberryMushroomMachine";
		public static readonly string PropagatorInternalName
			= $"{PackageName}.Propagator";

		// Files
		public static readonly string MachinePath
			= Path.Combine("assets", "propagator.png");
		public static readonly string OverlayPath
			= Path.Combine("assets", "overlay.png");
		public static readonly string EventsPath
			= Path.Combine("assets", "events.json");

		// Objects
		public static int PropagatorIndex = 0;
		public static string ObjectData = "{0}/0/-300/Crafting -9/{1}/true/true/0";
		public static string CraftingRecipeData = "388 20 709 1/Home/{0}/true/null";

		public const int OverlayMushroomFrames = 3;
	}
}
