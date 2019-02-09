namespace TehPers.Core.Gui.Base.Units {
    public interface IUnit<TResolutionInfo> {
        /// <summary>The quantity of this unit.</summary>
        float Quantity { get; }

        /// <summary>Resolves this unit to a single unified unit used throughout a project. In many cases, this will be pixels.</summary>
        /// <param name="info">The information needed to resolve this unit.</param>
        /// <returns>The resolved unit.</returns>
        float Resolve(TResolutionInfo info);

        /// <summary>Negates this unit and quantity.</summary>
        /// <returns>A new <see cref="IUnit{TResolutionInfo}"/> which when added to this <see cref="IUnit{TResolutionInfo}"/> should result in an empty unit.</returns>
        IUnit<TResolutionInfo> Negate();

        /// <summary>Tries to add this to another <see cref="IUnit{TResolutionInfo}"/>. If the two are incompatible, returns false.</summary>
        /// <param name="other">The other unit to add this with.</param>
        /// <param name="sum">The sum of this unit and the other unit, if compatible.</param>
        /// <returns>True if this was successfully added to the other, false if the units are incompatible.</returns>
        bool TryAdd(IUnit<TResolutionInfo> other, out IUnit<TResolutionInfo> sum);

        /// <summary>Multiplies this unit and quantity by a scalar amount and returns the product as a new <see cref="IUnit{TResolutionInfo}"/>.</summary>
        /// <param name="scalar">The scalar being multiplied by.</param>
        /// <returns>A new <see cref="IUnit{TResolutionInfo}"/> which is the product.</returns>
        IUnit<TResolutionInfo> Multiply(float scalar);
    }
}