/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace MiniDungeons.Data
{
	internal class Dungeon
	{
		public string DungeonName { get; set; } = string.Empty;
		public string SpawnMapName { get; set; } = string.Empty;
		public float SpawnChance { get; set; } = 0f;
		public int PortalX { get; set; }
		public int PortalY { get; set; }
		public List<DungeonMap> DungeonMaps { get; set; } = new List<DungeonMap>();
	}
}
