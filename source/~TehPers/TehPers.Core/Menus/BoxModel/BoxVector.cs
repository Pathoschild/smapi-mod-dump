using System;

namespace TehPers.Core.Menus.BoxModel {
    public readonly struct BoxVector : IEquatable<BoxVector> {
        public static BoxVector Zero { get; } = new BoxVector(0, 0, 0, 0);
        public static BoxVector Fill { get; } = new BoxVector(0, 0, 1, 1);

        public int AbsoluteX { get; }
        public int AbsoluteY { get; }
        public float PercentX { get; }
        public float PercentY { get; }

        public BoxVector(int absoluteX, int absoluteY, float percentX, float percentY) {
            this.AbsoluteX = absoluteX;
            this.AbsoluteY = absoluteY;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }

        public Vector2I ToAbsolute(Vector2I parentSize) => this.ToAbsolute(parentSize.X, parentSize.Y);
        public Vector2I ToAbsolute(int parentWidth, int parentHeight) {
            return new Vector2I((int) (this.PercentX * parentWidth) + this.AbsoluteX, (int) (this.PercentY * parentHeight) + this.AbsoluteY);
        }

        public bool Equals(BoxVector other) {
            return this.AbsoluteX == other.AbsoluteX && this.AbsoluteY == other.AbsoluteY && this.PercentX.Equals(other.PercentX) && this.PercentY.Equals(other.PercentY);
        }

        public override bool Equals(object other) {
            if (other is null)
                return false;
            return other is BoxVector otherVector && this.Equals(otherVector);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.AbsoluteX;
                hashCode = (hashCode * 397) ^ this.AbsoluteY;
                hashCode = (hashCode * 397) ^ this.PercentX.GetHashCode();
                hashCode = (hashCode * 397) ^ this.PercentY.GetHashCode();
                return hashCode;
            }
        }
    }
}