/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BuildableGingerIslandFarm.Utilities
{
	class PointComparer : IEqualityComparer<Point>
	{
		public bool Equals(Point x, Point y)
		{
			return x.X == y.X && x.Y == y.Y;
		}

		public int GetHashCode(Point obj)
		{
			return (obj.Y << 16) ^ obj.X;
		}
	}
}
