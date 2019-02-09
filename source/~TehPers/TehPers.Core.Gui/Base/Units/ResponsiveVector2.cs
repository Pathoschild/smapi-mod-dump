using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui.Base.Units {
    public class ResponsiveVector2<TResolutionInfo> {
        public ResponsiveUnit<TResolutionInfo> X { get; }
        public ResponsiveUnit<TResolutionInfo> Y { get; }

        public ResponsiveVector2() : this(Enumerable.Empty<IUnit<TResolutionInfo>>(), Enumerable.Empty<IUnit<TResolutionInfo>>()) { }
        public ResponsiveVector2(IUnit<TResolutionInfo> x, IUnit<TResolutionInfo> y) : this(x.Yield(), y.Yield()) { }
        public ResponsiveVector2(IEnumerable<IUnit<TResolutionInfo>> x, IEnumerable<IUnit<TResolutionInfo>> y) : this(new ResponsiveUnit<TResolutionInfo>(x), new ResponsiveUnit<TResolutionInfo>(y)) { }
        public ResponsiveVector2(ResponsiveUnit<TResolutionInfo> x, ResponsiveUnit<TResolutionInfo> y) {
            this.X = x;
            this.Y = y;
        }

        /// <summary>Resolves the components of this vector to single unified units used throughout a project. In many cases, this will be pixels.</summary>
        /// <param name="xInfo">The information needed to resolve the x component of this vector.</param>
        /// <param name="yInfo">The information needed to resolve the y component of this vector.</param>
        /// <returns>A two-dimensional vector containing this vector's resolved x and y components.</returns>
        public ResolvedVector2 Resolve(TResolutionInfo xInfo, TResolutionInfo yInfo) {
            return new ResolvedVector2(this.X.Resolve(xInfo), this.Y.Resolve(yInfo));
        }

        public static ResponsiveVector2<TResolutionInfo> operator +(ResponsiveVector2<TResolutionInfo> first, ResponsiveVector2<TResolutionInfo> second) {
            return new ResponsiveVector2<TResolutionInfo>(first.X + second.X, first.Y + second.Y);
        }

        public static ResponsiveVector2<TResolutionInfo> operator -(ResponsiveVector2<TResolutionInfo> first, ResponsiveVector2<TResolutionInfo> second) {
            return new ResponsiveVector2<TResolutionInfo>(first.X - second.X, first.Y - second.Y);
        }

        public static ResponsiveVector2<TResolutionInfo> operator *(ResponsiveVector2<TResolutionInfo> source, float scalar) {
            return new ResponsiveVector2<TResolutionInfo>(source.X * scalar, source.Y * scalar);
        }

        public static ResponsiveVector2<TResolutionInfo> operator *(float scalar, ResponsiveVector2<TResolutionInfo> source) {
            return new ResponsiveVector2<TResolutionInfo>(source.X * scalar, source.Y * scalar);
        }

        public static ResponsiveVector2<TResolutionInfo> operator -(ResponsiveVector2<TResolutionInfo> source) {
            return new ResponsiveVector2<TResolutionInfo>(-source.X, -source.Y);
        }
    }
}