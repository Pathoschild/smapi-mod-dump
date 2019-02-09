using System.Collections.Generic;

namespace TehPers.Core.Collections {
    public static class ImmutableExtensions {
        public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> source) {
            return new ImmutableArray<T>(source);
        }
    }
}