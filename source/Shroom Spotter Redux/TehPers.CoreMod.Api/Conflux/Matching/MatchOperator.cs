/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Conflux.Matching {
    public readonly ref struct MatchOperator<TSource, TResult> {
        private readonly TSource _source;
        private readonly TResult _result;
        private readonly bool _matched;

        internal MatchOperator(TSource source) : this(source, default, false) { }
        internal MatchOperator(TSource source, TResult result) : this(source, result, true) { }
        private MatchOperator(TSource source, TResult result, bool matched) {
            this._source = source;
            this._result = result;
            this._matched = matched;
        }

        /// <summary>Transforms the source if it matches a specific case.</summary>
        /// <param name="case">The value which the source must match.</param>
        /// <param name="output">The value to transform the source into if it matches.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When(TSource @case, TResult output) => this.When(item => object.Equals(item, @case), _ => output);

        /// <summary>Transforms the source if it matches a specific case.</summary>
        /// <param name="case">The value which the source must match.</param>
        /// <param name="outputFactory">The function which returns the value to transform the source into if it matches.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When(TSource @case, Func<TResult> outputFactory) => this.When(item => object.Equals(item, @case), _ => outputFactory());

        /// <summary>Transforms the source if it matches a predicate.</summary>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When(Func<TSource, bool> predicate, TResult output) => this.When(predicate, _ => output);

        /// <summary>Transforms the source if it matches a predicate.</summary>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When(Func<TSource, bool> predicate, Func<TSource, TResult> transform) {
            if (!this._matched && predicate(this._source)) {
                return new MatchOperator<TSource, TResult>(this._source, transform(this._source));
            }

            return this;
        }

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When<T>(TResult output) => this.When<T>(_ => true, _ => output);

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When<T>(Func<T, TResult> transform) => this.When(_ => true, transform);

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When<T>(Func<T, bool> predicate, TResult output) => this.When(predicate, _ => output);

        /// <summary>Transforms the source if it is a certain type and matches a predicate.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchOperator<TSource, TResult> When<T>(Func<T, bool> predicate, Func<T, TResult> transform) {
            if (!this._matched && this._source is T casted && predicate(casted)) {
                return new MatchOperator<TSource, TResult>(this._source, transform(casted));
            }

            return this;
        }

        /// <summary>Throws an exception if the source when it is unmatched and returns the result.</summary>
        /// <returns>The result of the match operation.</returns>
        public TResult ElseThrow() => this.Else(obj => throw new InvalidOperationException($"The object {obj} was not matched."));

        /// <summary>Throws an exception if the source when it is unmatched and returns the result.</summary>
        /// <param name="exceptionFactory">A function which transforms the unmatched value into an exception.</param>
        /// <returns>The result of the match operation.</returns>
        public TResult ElseThrow(Func<TSource, Exception> exceptionFactory) => this.Else(item => throw exceptionFactory(item));

        /// <summary>Transforms the source if it is unmatched and returns the result.</summary>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>The result of the match operation.</returns>
        public TResult Else(TResult output) => this.Else(_ => output);

        /// <summary>Transforms the source if it is unmatched and returns the result.</summary>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>The result of the match operation.</returns>
        public TResult Else(Func<TSource, TResult> transform) {
            return this._matched ? this._result : transform(this._source);
        }
    }
}