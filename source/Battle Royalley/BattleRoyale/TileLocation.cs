/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BattleRoyale
{
	struct TileLocation
	{
		public readonly int tileX;
		public readonly int tileY;
		public readonly string locationName;

		public TileLocation(string locationName, int tileX, int tileY)
		{
			this.tileX = tileX;
			this.tileY = tileY;
			this.locationName = locationName;
		}

		public Warp CreateWarp() => new Warp(0, 0, locationName, tileX, tileY, false);
		public Vector2 CreateVector2() => new Vector2(tileX, tileY);

		public GameLocation GetGameLocation()
		{
			foreach (GameLocation location in Game1.locations)
				if (location.Name == locationName)
					return location;

			return null;
		}
	}
}
