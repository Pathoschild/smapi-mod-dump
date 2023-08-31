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

		// Console
		public static readonly string ConsoleCommandPrefix
			= "bb.mm.";
		public static readonly string GiveConsoleCommand
			= ModValues.ConsoleCommandPrefix + "give";
		public static readonly string GrowConsoleCommand
			= ModValues.ConsoleCommandPrefix + "grow";
		public static readonly string StatusConsoleCommand
			= ModValues.ConsoleCommandPrefix + "status";
		public static readonly string FixIdsConsoleCommand
			= ModValues.ConsoleCommandPrefix + "fix_ids";

		// Objects
		public const int OverlayMushroomFrames = 4;
		public const string ObjectDataFormat = "{0}/0/-300/Crafting -9/{1}/true/true/0";
		public const string RecipeDataFormat = "388 20 709 1/Home/{0}/true/null";

		public static int PropagatorIndex { get; set; } = 0;
		public static string ObjectData { get; set; } = null;
		public static string RecipeData { get; set; } = null;

		// Events
		public const int EventId = 46370001;
	}
}
