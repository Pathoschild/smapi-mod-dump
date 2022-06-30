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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using xTile;


namespace MiniDungeons.Data
{
	internal class Challenge
	{
		public string ChallengeName { get; set; } = string.Empty;
		public int Timer { get; set; } = -1;
		public List<Wave> MonsterWaves { get; set; } = new List<Wave>();
		public int TimerBeforeNextWaveSpawns { get; set; } = -1;
		public List<Point> SpawnPoints { get; set; } = new List<Point>();
		public List<SpawnedObject> SpawnedObjects { get; set; } = new List<SpawnedObject>();
		public List<ItemInfo> Rewards { get; set; } = new List<ItemInfo>();
	}
}
