using System;
using System.Collections.Generic;

namespace TehPers.Core.Menus.BoxModel {
    public readonly struct BoxRectangle : IEquatable<BoxRectangle> {
        public BoxVector Location { get; }
        public BoxVector Size { get; }

        public BoxRectangle(BoxVector location, in BoxVector size) {
            this.Location = location;
            this.Size = size;
        }

        public Rectangle2I ToAbsolute(in Rectangle2I parentBounds) => this.ToAbsolute(parentBounds.Location.X, parentBounds.Location.Y, parentBounds.Size.X, parentBounds.Size.Y);
        public Rectangle2I ToAbsolute(int parentX, int parentY, int parentWidth, int parentHeight) {
            Vector2I loc = this.Location.ToAbsolute(parentWidth, parentHeight);
            Vector2I size = this.Size.ToAbsolute(parentWidth, parentHeight);
            return new Rectangle2I(loc.X + parentX, loc.Y + parentY, size.X, size.Y);
        }

        public bool Equals(BoxRectangle other) {
            return this.Location.Equals(other.Location) && this.Size.Equals(other.Size);
        }

        public override bool Equals(object other) {
            if (other is null)
                return false;
            return other is BoxRectangle otherRect && this.Equals(otherRect);
        }

        public override int GetHashCode() {
            unchecked {
                return (this.Location.GetHashCode() * 397) ^ this.Size.GetHashCode();
            }
        }
    }
}