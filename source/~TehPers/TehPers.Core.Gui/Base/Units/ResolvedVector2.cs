namespace TehPers.Core.Gui.Base.Units {
    public readonly struct ResolvedVector2 {
        public float X { get; }
        public float Y { get; }

        public ResolvedVector2(float x, float y) {
            this.X = x;
            this.Y = y;
        }

        public static ResolvedVector2 operator +(in ResolvedVector2 first, in ResolvedVector2 second) {
            return new ResolvedVector2(first.X + second.X, first.Y + second.Y);
        }

        public static ResolvedVector2 operator *(in ResolvedVector2 vector, float scalar) {
            return new ResolvedVector2(vector.X * scalar, vector.Y * scalar);
        }

        public static ResolvedVector2 operator *(float scalar, in ResolvedVector2 vector) {
            return new ResolvedVector2(vector.X * scalar, vector.Y * scalar);
        }

        public static ResolvedVector2 operator -(in ResolvedVector2 vector) {
            return new ResolvedVector2(-vector.X, -vector.Y);
        }

        public static ResolvedVector2 Zero => default;
    }
}
