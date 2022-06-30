/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

namespace MiniDungeons.Data
{
	internal class DungeonMap
	{
		public string MapFile { get; set; } = string.Empty;
		public int SpawnWeight { get; set; }
		public int EntryX { get; set; }
		public int EntryY { get; set; }
		public string Challenge { get; set; } = string.Empty;
	}
}
