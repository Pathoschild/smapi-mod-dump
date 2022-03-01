/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.Common
{
    public class AbsolutePosition {

		public GameLocation Location { get; }
		public Vector2 Position { get; }

		public AbsolutePosition(GameLocation location, Vector2 position) {
			Location = location;
			Position = position;
		}

		public override bool Equals(object obj) {
			return obj is AbsolutePosition position &&
				   EqualityComparer<GameLocation>.Default.Equals(Location, position.Location) &&
				   Position.Equals(position.Position);
		}

		public override int GetHashCode() {
			return HashCode.Combine(Location, Position);
		}
	}
}
