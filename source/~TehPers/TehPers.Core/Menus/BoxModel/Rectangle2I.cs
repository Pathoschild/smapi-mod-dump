using System;
using Microsoft.Xna.Framework;

namespace TehPers.Core.Menus.BoxModel {
    public readonly struct Rectangle2I : IEquatable<Rectangle2I> {
        /// <summary>The top, left point of this rectangle.</summary>
        public Vector2I Location { get; }
        /// <summary>The size of this rectangle.</summary>
        public Vector2I Size { get; }

        public int X => this.Location.X;
        public int Y => this.Location.Y;
        public int Width => this.Size.X;
        public int Height => this.Size.Y;

        public Rectangle2I(int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }
        public Rectangle2I(Vector2I location, Vector2I size) {
            this.Location = location;
            this.Size = size;
        }

        public bool Contains(in Vector2I location) {
            Vector2I bottomRight = this.Location + this.Size;
            return this.Location.X <= location.X && this.Location.Y <= location.Y && bottomRight.X > location.X && bottomRight.Y > location.Y;
        }

        public Rectangle ToRectangle() => new Rectangle(this.Location.X, this.Location.Y, this.Size.X, this.Size.Y);

        public bool Equals(Rectangle2I other) {
            return this.Location.Equals(other.Location) && this.Size.Equals(other.Size);
        }

        public override bool Equals(object other) {
            if (other is null)
                return false;
            return other is Rectangle2I otherRect && this.Equals(otherRect);
        }

        public override int GetHashCode() {
            unchecked {
                return (this.Location.GetHashCode() * 397) ^ this.Size.GetHashCode();
            }
        }
    }
}
