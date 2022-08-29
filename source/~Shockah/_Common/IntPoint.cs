/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shockah.CommonModCode
{
	public readonly struct IntPoint : IEquatable<IntPoint>
	{
		public static readonly IntPoint Zero = new(0, 0);
		public static readonly IntPoint One = new(1, 1);
		public static readonly IntPoint Left = new(-1, 0);
		public static readonly IntPoint Right = new(1, 0);
		public static readonly IntPoint Top = new(0, -1);
		public static readonly IntPoint Bottom = new(0, 1);

		private static readonly IntPoint[] NeighborOffsetsArray = new[] { Left, Right, Top, Bottom };

		public static IEnumerable<IntPoint> NeighborOffsets
			=> NeighborOffsetsArray;

		public readonly int X { get; init; }
		public readonly int Y { get; init; }

		[JsonIgnore]
		public IEnumerable<IntPoint> Neighbors
		{
			get
			{
				var self = this;
				return Array.ConvertAll(NeighborOffsetsArray, n => self + n);
			}
		}

		public IntPoint(int x, int y) : this()
		{
			this.X = x;
			this.Y = y;
		}

		public IntPoint(int v) : this(v, v)
		{
		}

		public override string ToString()
			=> $"[{X}, {Y}]";

		public override bool Equals(object? obj)
			=> obj is IntPoint point && Equals(point);

		public bool Equals(IntPoint other)
			=> X == other.X && Y == other.Y;

		public override int GetHashCode()
			=> (X * 256) ^ Y;

		public void Deconstruct(out int x, out int y)
		{
			x = X;
			y = Y;
		}

		public static IntPoint operator +(IntPoint a, IntPoint b)
			=> new(a.X + b.X, a.Y + b.Y);

		public static IntPoint operator -(IntPoint a, IntPoint b)
			=> new(a.X - b.X, a.Y - b.Y);

		public static IntPoint operator *(IntPoint point, int scalar)
			=> new(point.X * scalar, point.Y * scalar);

		public static IntPoint operator -(IntPoint point)
			=> new(-point.X, -point.Y);

		public static bool operator ==(IntPoint lhs, IntPoint rhs)
			=> lhs.Equals(rhs);

		public static bool operator !=(IntPoint lhs, IntPoint rhs)
			=> !lhs.Equals(rhs);

		public IEnumerable<IntPoint> GetSpiralingTiles(int minDistanceFromCenter = 1, int maxDistanceFromCenter = int.MaxValue)
		{
			if (minDistanceFromCenter == 0)
				yield return this;
			for (int i = Math.Max(minDistanceFromCenter, 1); i <= maxDistanceFromCenter; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					yield return new IntPoint(X - j, Y - i);
					if (j != 0)
						yield return new IntPoint(X + j, Y - i);

					yield return new IntPoint(X + i, Y - j);
					if (j != 0)
						yield return new IntPoint(X + i, Y + j);

					yield return new IntPoint(X + j, Y + i);
					if (j != 0)
						yield return new IntPoint(X - j, Y + i);

					yield return new IntPoint(X - i, Y + j);
					if (j != 0)
						yield return new IntPoint(X - i, Y - j);
				}
			}
		}
	}
}