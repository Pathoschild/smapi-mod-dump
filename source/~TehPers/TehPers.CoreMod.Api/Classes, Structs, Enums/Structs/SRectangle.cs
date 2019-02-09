using System;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct SRectangle : IEquatable<SRectangle>, IEquatable<Rectangle> {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public int Left => this.X;
        public int Right => this.X + this.Width;
        public int Top => this.Y;
        public int Bottom => this.Y + this.Height;
        public SPoint Location => new SPoint(this.X, this.Y);
        public SPoint Center => new SPoint(this.X + this.Width / 2, this.Y + this.Height / 2);
        public SPoint MaxCorner => new SPoint(this.X + this.Width, this.Y + this.Height);
        public bool IsEmpty => this.Width == 0 && this.Height == 0 && this.X == 0 && this.Y == 0;

        public SRectangle(int x, int y, int width, int height) {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Equals(SRectangle other) {
            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
        }

        public bool Equals(Rectangle other) {
            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
        }

        public override bool Equals(object obj) {
            return obj.Match<object, bool>()
                .When<SRectangle>(this.Equals)
                .When<Rectangle>(this.Equals)
                .Else(false);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.X;
                hashCode = (hashCode * 397) ^ this.Y;
                hashCode = (hashCode * 397) ^ this.Width;
                hashCode = (hashCode * 397) ^ this.Height;
                return hashCode;
            }
        }

        public override string ToString() {
            return $"{{{{X:{this.X} Y:{this.Y} Width:{this.Width} Height:{this.Height}}}}}";
        }

        public static implicit operator Rectangle(in SRectangle source) {
            return new Rectangle(source.X, source.Y, source.Width, source.Height);
        }

        public static implicit operator SRectangle(Rectangle source) {
            return new SRectangle(source.X, source.Y, source.Width, source.Height);
        }

        public static implicit operator xTile.Dimensions.Rectangle(in SRectangle source) {
            return new xTile.Dimensions.Rectangle(source.X, source.Y, source.Width, source.Height);
        }

        public static implicit operator SRectangle(xTile.Dimensions.Rectangle source) {
            return new SRectangle(source.X, source.Y, source.Width, source.Height);
        }

        public static bool operator ==(in SRectangle first, in SRectangle second) {
            return first.Equals(second);
        }

        public static bool operator !=(in SRectangle first, in SRectangle second) {
            return !first.Equals(second);
        }

        public static SRectangle Empty => new SRectangle();
    }
}