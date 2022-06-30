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
	internal class Portal
	{
		public readonly GameLocation location;
		public readonly Point point;
		public readonly MiniDungeons.Dungeon dungeon;

		public TemporaryAnimatedSprite? sprite;


		public Portal(GameLocation location, Point point, MiniDungeons.Dungeon dungeon)
		{
			this.location = location;
			this.point = point;
			this.dungeon = dungeon;
		}
	}
}
