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
using System.Collections.Generic;
using System.Linq;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;

namespace TehPers.CoreMod.ContentPacks.Values {
    internal class ConditionalContentPackValueProvider<T> : IContentPackValueProvider<T> {
        private readonly ICase[] _cases;

        public ConditionalContentPackValueProvider(UnconditionalCase fallback) : this(Enumerable.Empty<ConditionalCase>(), fallback) { }
        public ConditionalContentPackValueProvider(IEnumerable<ConditionalCase> conditionalCases, UnconditionalCase fallback) {
            if (conditionalCases == null) {
                throw new ArgumentNullException(nameof(conditionalCases));
            }

            if (fallback == null) {
                throw new ArgumentNullException(nameof(fallback));
            }

            this._cases = conditionalCases.Append<ICase>(fallback).ToArray();
        }

        public T GetValue(ITokenHelper helper) {
            foreach (ICase @case in this._cases) {
                if (@case.IsConditionMet(helper)) {
                    return @case.ValueProvider.GetValue(helper);
                }
            }

            throw new InvalidOperationException("The state of this conditional value is invalid. Please post your log to https://smapi.io/ and send the link to the mod author.");
        }

        public bool IsValidInContext(IContext context) {
            return this._cases.All(@case => @case.IsValidInContext(context));
        }

        public interface ICase : IContextSpecific {
            /// <summary>Provides the value this case returns when its conditions are met.</summary>
            IContentPackValueProvider<T> ValueProvider { get; }

            /// <summary>Checks if this case's conditions are met.</summary>
            /// <param name="helper">The token helper.</param>
            /// <returns>True if this case's conditions are met and the value should be used, false otherwise.</returns>
            bool IsConditionMet(ITokenHelper helper);
        }

        public class UnconditionalCase : ICase {
            public IContentPackValueProvider<T> ValueProvider { get; }

            public UnconditionalCase(IContentPackValueProvider<T> valueProvider) {
                this.ValueProvider = valueProvider;
            }

            public bool IsValidInContext(IContext context) {
                // Check if the value provider is valid
                return this.ValueProvider.IsValidInContext(context);
            }

            public bool IsConditionMet(ITokenHelper helper) {
                return true;
            }
        }

        public class ConditionalCase : ICase {
            public IContentPackValueProvider<T> ValueProvider { get; }
            public Dictionary<IContentPackValueProvider<string>, string>[] ConditionGroups { get; }

            public ConditionalCase(IContentPackValueProvider<T> valueProvider, TokenParser parser, IEnumerable<Dictionary<string, string>> conditionGroups) {
                this.ValueProvider = valueProvider;

                this.ConditionGroups = conditionGroups.Select(conditionGroup => {
                    Dictionary<IContentPackValueProvider<string>, string> parsedGroup = new Dictionary<IContentPackValueProvider<string>, string>();

                    // Parse each condition
                    foreach ((string tokenStr, string expectedValue) in conditionGroup) {
                        parsedGroup.Add(parser.ParseToken(tokenStr), expectedValue);
                    }

                    return parsedGroup;
                }).ToArray();
            }

            public bool IsValidInContext(IContext context) {
                // Check if the value provider and all conditions are valid
                return this.ValueProvider.IsValidInContext(context) && this.ConditionGroups.All(conditionGroup => conditionGroup.Keys.All(token => token.IsValidInContext(context)));
            }

            public bool IsConditionMet(ITokenHelper helper) {
                // If there are no condition groups, then it is met
                if (!this.ConditionGroups.Any()) {
                    return true;
                }

                // Check if at least one condition group's conditions are met
                return this.ConditionGroups.Any(conditionGroup => conditionGroup.All(condition => string.Equals(condition.Key.GetValue(helper), condition.Value, StringComparison.OrdinalIgnoreCase)));
            }
        }
    }
}