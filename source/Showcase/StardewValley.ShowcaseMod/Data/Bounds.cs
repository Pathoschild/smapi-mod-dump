using System.Diagnostics;

namespace Igorious.StardewValley.ShowcaseMod.Data
{
    [DebuggerDisplay("top:{Top}, left:{Left}, rigth:{Right}, bottom:{Bottom}")]
    public sealed class Bounds
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static Bounds Empty => new Bounds();

        public int Top { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }

        private bool Equals(Bounds other)
        {
            return other != null 
                && Top == other.Top
                && Left == other.Left 
                && Bottom == other.Bottom 
                && Right == other.Right;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Bounds);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Top;
                hashCode = (hashCode * 397) ^ Left;
                hashCode = (hashCode * 397) ^ Bottom;
                hashCode = (hashCode * 397) ^ Right;
                return hashCode;
            }
        }

        public static bool operator ==(Bounds left, Bounds right) => Equals(left, right);
        public static bool operator !=(Bounds left, Bounds right) => !Equals(left, right);
    }
}