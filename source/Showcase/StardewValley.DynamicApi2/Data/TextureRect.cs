using System.Diagnostics;

namespace Igorious.StardewValley.DynamicApi2.Data
{
    [DebuggerDisplay("{Index} [{Length}x{Height}]")]
    public sealed class TextureRect
    {
        public int Index { get; }
        public int Length { get; }
        public int Height { get; }

        public TextureRect(int index, int length = 1, int height = 1)
        {
            Index = index;
            Length = length;
            Height = height;
        }

        private bool Equals(TextureRect other)
        {
            return other != null
                && Index == other.Index 
                && Length == other.Length 
                && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as TextureRect);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index;
                hashCode = (hashCode * 397) ^ Length;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }

        public static bool operator ==(TextureRect left, TextureRect right) => Equals(left, right);
        public static bool operator !=(TextureRect left, TextureRect right) => !Equals(left, right);
    }
}