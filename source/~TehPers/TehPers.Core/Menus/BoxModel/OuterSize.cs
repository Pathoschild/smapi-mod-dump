using System;

namespace TehPers.Core.Menus.BoxModel {
    public readonly struct OuterSize : IEquatable<OuterSize> {
        public static OuterSize Zero { get; } = new OuterSize(0);

        /// <summary>Absolute size on the left</summary>
        public int Left { get; }

        /// <summary>Absolute size on the right</summary>
        public int Right { get; }

        /// <summary>Absolute size on the top</summary>
        public int Top { get; }

        /// <summary>Absolute size on the bottom</summary>
        public int Bottom { get; }

        public Vector2I TopLeft => new Vector2I(this.Left, this.Top);

        public Vector2I BottomRight => new Vector2I(this.Right, this.Bottom);

        public OuterSize(int left, int right, int top, int bottom) {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public OuterSize(int leftRight, int topBottom) : this(leftRight, leftRight, topBottom, topBottom) { }

        public OuterSize(int size) : this(size, size, size, size) { }

        public OuterSize(OuterSize source, int? left = null, int? right = null, int? top = null, int? bottom = null) : this(left ?? source.Left, right ?? source.Right, top ?? source.Top, bottom ?? source.Bottom) { }

        public bool Equals(OuterSize other) {
            return this.Left == other.Left && this.Right == other.Right && this.Top == other.Top && this.Bottom == other.Bottom;
        }

        public override bool Equals(object other) {
            if (other is null)
                return false;
            return other is OuterSize otherSize && this.Equals(otherSize);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.Left;
                hashCode = (hashCode * 397) ^ this.Right;
                hashCode = (hashCode * 397) ^ this.Top;
                hashCode = (hashCode * 397) ^ this.Bottom;
                return hashCode;
            }
        }
    }
}
