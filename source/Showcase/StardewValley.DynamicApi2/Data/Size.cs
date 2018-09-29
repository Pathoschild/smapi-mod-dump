using System.Diagnostics;

namespace Igorious.StardewValley.DynamicApi2.Data
{
    [DebuggerDisplay("{Width}x{Height}")]
    public sealed class Size
    {
        public static Size Default => new Size(-1, -1);

        public Size() { }
        public Size(int width, int height = 1)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return Width != -1 ? $"{Width} {Height}" : "-1";
        }

        private bool Equals(Size other)
        {
            return other != null
                && Width == other.Width 
                && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Size);
        }

        public override int GetHashCode()
        {
            unchecked { return (Width * 397) ^ Height; }
        }

        public static bool operator ==(Size left, Size right) => Equals(left, right);
        public static bool operator !=(Size left, Size right) => !Equals(left, right);
    }
}