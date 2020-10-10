/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
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
            return Index == other.Index && Length == other.Length && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TextureRect && Equals((TextureRect)obj);
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

        public static bool operator ==(TextureRect left, TextureRect right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TextureRect left, TextureRect right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Index} [{Length}x{Height}]";
        }
    }
}