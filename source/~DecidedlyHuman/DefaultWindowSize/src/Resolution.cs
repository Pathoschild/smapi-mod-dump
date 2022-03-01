/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Text.RegularExpressions;

namespace DefaultWindowSize
{
	public class Resolution
	{
		private int _x;
		private int _y;

		public int X { get { return _x; } }
		public int Y { get { return _y; } }

		public Resolution(string resolution)
		{
			ParseResolution(resolution);
		}

		// TODO: Consider refactoring this to be outside of the Resolution class?
		private void ParseResolution(string resolution)
		{
			// We want to split our (potential) resolution string into two strings, and from there, parse to int.
			string pattern = @"(\d+).*?(\d+)";
			Regex regex = new Regex(pattern);
			MatchCollection matches = regex.Matches(resolution);

			if (matches[0].Groups.Count != 3) // At this point, the resolution was invalid, or not sensible, so we use plain old 720p.
				SetResolution(1280, 720);

			int x = int.Parse(matches[0].Groups[1].Value);
			int y = int.Parse(matches[0].Groups[2].Value);

			SetResolution(x, y);
		}

		private void SetResolution(int x, int y)
		{
			_x = x;
			_y = y;
		}
	}
}
