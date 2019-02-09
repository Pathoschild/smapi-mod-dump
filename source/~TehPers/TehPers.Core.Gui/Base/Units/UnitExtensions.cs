using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Gui.Base.Units {
    public static class UnitExtensions {

        /// <summary>Condenses the units down to as few instances of <see cref="IUnit"/> as possible by adding them together.</summary>
        /// <param name="source">The source units.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing the condensed units.</returns>
        public static IEnumerable<IUnit<T>> Condense<T>(this IEnumerable<IUnit<T>> source) {
            return source.Aggregate(new List<IUnit<T>>(), (acc, cur) => {
                for (int i = 0; i < acc.Count; i++) {
                    IUnit<T> unit = acc[i];
                    if (cur.TryAdd(unit, out IUnit<T> sum)) {
                        acc[i] = sum;
                        return acc;
                    }
                }

                acc.Add(cur);
                return acc;
            });
        }
    }
}