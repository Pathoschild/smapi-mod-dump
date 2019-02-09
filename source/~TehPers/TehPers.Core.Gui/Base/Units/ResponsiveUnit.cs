using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Collections;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui.Base.Units {
    public class ResponsiveUnit<TResolutionInfo> {
        private readonly ImmutableArray<IUnit<TResolutionInfo>> _units;

        public ResponsiveUnit(params IUnit<TResolutionInfo>[] units) : this((IEnumerable<IUnit<TResolutionInfo>>) units) { }
        public ResponsiveUnit(IEnumerable<IUnit<TResolutionInfo>> units) {
            this._units = units.ToImmutableArray();
        }

        /// <summary>Resolves the components of this to a single unified unit used throughout a project. In many cases, this will be pixels.</summary>
        /// <param name="info">The information needed to resolve this unit.</param>
        /// <returns>A <see cref="float"/> containing the sum of the resolved components.</returns>
        public float Resolve(TResolutionInfo info) {
            return this._units.Sum(unit => unit.Resolve(info));
        }

        public static ResponsiveUnit<TResolutionInfo> operator +(ResponsiveUnit<TResolutionInfo> first, ResponsiveUnit<TResolutionInfo> second) {
            return new ResponsiveUnit<TResolutionInfo>(first._units.Concat(second._units).Condense());
        }

        public static ResponsiveUnit<TResolutionInfo> operator -(ResponsiveUnit<TResolutionInfo> first, ResponsiveUnit<TResolutionInfo> second) {
            return new ResponsiveUnit<TResolutionInfo>(first._units.Concat(second._units.Select(unit => unit.Negate())).Condense());
        }

        public static ResponsiveUnit<TResolutionInfo> operator +(ResponsiveUnit<TResolutionInfo> first, IUnit<TResolutionInfo> second) {
            return new ResponsiveUnit<TResolutionInfo>(first._units.Concat(second.Yield()).Condense());
        }

        public static ResponsiveUnit<TResolutionInfo> operator -(ResponsiveUnit<TResolutionInfo> first, IUnit<TResolutionInfo> second) {
            return new ResponsiveUnit<TResolutionInfo>(first._units.Concat(second.Negate().Yield()).Condense());
        }

        public static ResponsiveUnit<TResolutionInfo> operator *(ResponsiveUnit<TResolutionInfo> source, float scalar) {
            return new ResponsiveUnit<TResolutionInfo>(source._units.Select(unit => unit.Multiply(scalar)));
        }

        public static ResponsiveUnit<TResolutionInfo> operator -(ResponsiveUnit<TResolutionInfo> source) {
            return new ResponsiveUnit<TResolutionInfo>(source._units.Select(unit => unit.Negate()));
        }

        /// <summary>An empty unit which resolves to 0.</summary>
        public static ResponsiveUnit<TResolutionInfo> Zero { get; } = new ResponsiveUnit<TResolutionInfo>();
    }
}