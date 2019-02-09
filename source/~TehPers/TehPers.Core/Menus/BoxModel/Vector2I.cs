using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TehPers.Core.Menus.BoxModel {
    public readonly struct Vector2I : IEquatable<Vector2I> {
        public int X { get; }
        public int Y { get; }

        public Vector2I(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public double Dot(in Vector2I other) => this.X * other.X + this.Y * other.Y;

        public double Magnitude() => Math.Sqrt(this.X * this.X + this.Y * this.Y);

        public Vector2I Normalize() => this / this.Magnitude();

        public Vector2I Project(in IEnumerable<Vector2I> space) {
            Vector2I tmpThis = this;
            return space.Aggregate(new Vector2I(0, 0), (current, v) => current + tmpThis.Dot(v) * current.Dot(v) * v);
        }

        public Vector2I Translate(int addX, int addY) => new Vector2I(this.X + addX, this.Y + addY);

        public Vector2 ToVector2() => new Vector2(this.X, this.Y);

        public bool Equals(Vector2I other) {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object other) {
            if (other is null)
                return false;
            return other is Vector2I otherVector && this.Equals(otherVector);
        }

        public override int GetHashCode() {
            unchecked {
                return (this.X * 397) ^ this.Y;
            }
        }

        public static Vector2I operator +(in Vector2I a, in Vector2I b) => new Vector2I(a.X + b.X, a.Y + b.Y);
        public static Vector2I operator -(in Vector2I a, in Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
        public static Vector2I operator -(in Vector2I vector) => new Vector2I(-vector.X, -vector.Y);
        public static Vector2I operator *(in Vector2I vector, double scalar) => new Vector2I((int) (vector.X * scalar), (int) (vector.Y * scalar));
        public static Vector2I operator *(double scalar, in Vector2I vector) => new Vector2I((int) (vector.X * scalar), (int) (vector.Y * scalar));
        public static Vector2I operator /(in Vector2I vector, double scalar) => new Vector2I((int) (vector.X / scalar), (int) (vector.Y / scalar));
    }
}