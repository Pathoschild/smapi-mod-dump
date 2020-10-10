/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct SPoint : IEquatable<SPoint>, IEquatable<Point> {
        public int X { get; }
        public int Y { get; }

        public SPoint(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out int x, out int y) {
            x = this.X;
            y = this.Y;
        }

        public bool Equals(SPoint other) {
            return this.X == other.X && this.Y == other.Y;
        }

        public bool Equals(Point other) {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj) {
            return obj.Match<object, bool>()
                .When<SPoint>(this.Equals)
                .When<Point>(this.Equals)
                .Else(false);
        }

        public override int GetHashCode() {
            return unchecked((this.X * 397) ^ this.Y);
        }

        public override string ToString() {
            return $"{{{{X:{this.X} Y:{this.Y}}}}}";
        }

        public static bool operator ==(in SPoint first, in SPoint second) {
            return first.Equals(second);
        }

        public static bool operator !=(in SPoint first, in SPoint second) {
            return !first.Equals(second);
        }

        public static SPoint Zero => new SPoint(0, 0);
    }
}