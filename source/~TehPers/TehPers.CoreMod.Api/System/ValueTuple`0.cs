/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections;

// ReSharper disable once CheckNamespace
namespace System {
    [Serializable]
    public readonly partial struct ValueTuple : IEquatable<ValueTuple>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple> {
        internal static int CombineHashes(params object[] items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            int hashCode = 0;
            foreach (var item in items) {
                hashCode = unchecked((hashCode * 397) ^ (item?.GetHashCode() ?? 0));
            }

            return hashCode;
        }

        public int CompareTo(object other, IComparer comparer) {
            return other is ValueTuple ? 0 : throw new ArgumentException();
        }

        public int CompareTo(object obj) {
            return obj is ValueTuple ? 0 : throw new ArgumentException();
        }

        public int CompareTo(ValueTuple other) {
            return 0;
        }

        public bool Equals(ValueTuple other) {
            return true;
        }

        public bool Equals(object other, IEqualityComparer comparer) {
            return other is ValueTuple;
        }

        public override bool Equals(object obj) {
            return obj is ValueTuple;
        }

        public int GetHashCode(IEqualityComparer comparer) {
            return 0;
        }

        public override int GetHashCode() {
            return 0;
        }
    }
}
