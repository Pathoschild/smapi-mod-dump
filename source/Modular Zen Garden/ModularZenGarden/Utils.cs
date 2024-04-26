/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/MZG
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace ModularZenGarden {
	
	class Utils
	{
		public static readonly string[] seasons = {"spring", "summer", "fall", "winter"};
		public static IMonitor? monitor;
		public static IModHelper? helper;
		public static ModConfig? config ;
		public static bool USJB_installed = false;

		public static Point get_pos<T>(T garden_source) where T : notnull
		{
			if (garden_source is Furniture)
			{
				var bb = ((Furniture)(object)garden_source).boundingBox;
				return new Point(bb.X/64, bb.Y/64);
			}
			else if (garden_source is Building)
			{
				var bb = ((Building)(object)garden_source).GetBoundingBox();
				return new Point(bb.X/64, bb.Y/64);
			}
			else throw new ArgumentException("The given object is neither a Furniture or a Building.");
		}

		public static void log(string message, LogLevel log_level = LogLevel.Info)
		{
			if (monitor == null)
				throw new NullReferenceException("Monitor was not set.");
			
			monitor.Log(message, log_level);
		}

		public static void invalidate_cache(string asset_name)
		{
			if (helper == null)
				throw new NullReferenceException("Helper was not set");
			
			helper.GameContent.InvalidateCache(asset_name);
		}
	}

}