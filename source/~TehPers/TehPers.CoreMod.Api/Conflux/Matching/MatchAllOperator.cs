using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.CoreMod.Api.Conflux.Matching {
    public readonly ref struct MatchAllOperator<TSource, TResult> {
        private readonly IEnumerable<MatchItem> _source;

        internal MatchAllOperator(IEnumerable<TSource> source) : this(source.Select(item => new MatchItem(item, default, false))) { }
        private MatchAllOperator(IEnumerable<MatchItem> source) {
            this._source = source;
        }

        /// <summary>Transforms the source if it matches a specific case.</summary>
        /// <param name="case">The value which the source must match.</param>
        /// <param name="output">The value to transform the source into if it matches.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When(TSource @case, TResult output) => this.When(item => object.Equals(item, @case), _ => output);

        /// <summary>Transforms the source if it matches a specific case.</summary>
        /// <param name="case">The value which the source must match.</param>
        /// <param name="outputFactory">The function which returns the value to transform the source into if it matches.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When(TSource @case, Func<TResult> outputFactory) => this.When(item => object.Equals(item, @case), _ => outputFactory());

        /// <summary>Transforms the source if it matches a predicate.</summary>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When(Func<TSource, bool> predicate, TResult output) => this.When(predicate, _ => output);

        /// <summary>Transforms the source if it matches a predicate.</summary>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When(Func<TSource, bool> predicate, Func<TSource, TResult> transform) {
            return new MatchAllOperator<TSource, TResult>(this._source.Select(item => {
                if (!item.Matched && predicate(item.Source)) {
                    return new MatchItem(item.Source, transform(item.Source), true);
                }

                return item;
            }));
        }

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When<T>(TResult output) => this.When<T>(_ => true, _ => output);

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When<T>(Func<T, TResult> transform) => this.When(_ => true, transform);

        /// <summary>Transforms the source if it is a certain type.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When<T>(Func<T, bool> predicate, TResult output) => this.When(predicate, _ => output);

        /// <summary>Transforms the source if it is a certain type and matches a predicate.</summary>
        /// <typeparam name="T">The type the source must be to match.</typeparam>
        /// <param name="predicate">The predicate which returns true if the source matches.</param>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>A <see cref="MatchAllOperator{TSource,TResult}"/> to complete the operation with.</returns>
        public MatchAllOperator<TSource, TResult> When<T>(Func<T, bool> predicate, Func<T, TResult> transform) {
            return new MatchAllOperator<TSource, TResult>(this._source.Select(item => {
                if (!item.Matched && item.Source is T casted && predicate(casted)) {
                    return new MatchItem(item.Source, transform(casted), true);
                }

                return item;
            }));
        }

        /// <summary>Transforms the source if it is unmatched and returns the result.</summary>
        /// <param name="output">The value to transform the source into.</param>
        /// <returns>The result of the match operation.</returns>
        public IEnumerable<TResult> Else(TResult output) => this.Else(_ => output);

        /// <summary>Transforms the source if it is unmatched and returns the result.</summary>
        /// <param name="transform">The function which transforms the source.</param>
        /// <returns>The result of the match operation.</returns>
        public IEnumerable<TResult> Else(Func<TSource, TResult> transform) {
            return this._source.Select(item => item.Matched ? item.Result : transform(item.Source));
        }

        /// <summary>Throws an exception if the source when it is unmatched and returns the result.</summary>
        /// <returns>The result of the match operation.</returns>
        public IEnumerable<TResult> ElseThrow() => this.Else(obj => throw new InvalidOperationException($"The object {obj} was not matched."));

        /// <summary>Throws an exception if the source when it is unmatched and returns the result.</summary>
        /// <param name="exceptionFactory">A function which transforms the unmatched value into an exception.</param>
        /// <returns>The result of the match operation.</returns>
        public IEnumerable<TResult> ElseThrow(Func<TSource, Exception> exceptionFactory) => this.Else(item => throw exceptionFactory(item));

        private readonly struct MatchItem {
            public TSource Source { get; }
            public TResult Result { get; }
            public bool Matched { get; }

            public MatchItem(TSource source, TResult result, bool matched) {
                this.Source = source;
                this.Result = result;
                this.Matched = matched;
            }
        }
    }
}