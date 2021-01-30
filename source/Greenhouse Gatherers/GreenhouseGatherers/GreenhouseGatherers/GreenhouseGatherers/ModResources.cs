/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using StardewModdingAPI;
using System.Text.RegularExpressions;

namespace GreenhouseGatherers
{
    public static class ModResources
    {
        private static IMonitor monitor;

		public static void LoadMonitor(IMonitor iMonitor)
		{
			monitor = iMonitor;
		}

		public static IMonitor GetMonitor()
		{
			return monitor;
		}

		public static string SplitCamelCaseText(string input)
        {
			return string.Join(" ", Regex.Split(input, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
		}
	}
}
