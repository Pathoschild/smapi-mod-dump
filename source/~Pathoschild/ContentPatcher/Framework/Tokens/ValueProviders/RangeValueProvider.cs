/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ContentPatcher.Framework.Conditions;

namespace ContentPatcher.Framework.Tokens.ValueProviders
{
    /// <summary>A value provider which provides a range of integer values.</summary>
    internal class RangeValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>The maximum number of entries to allow in a range.</summary>
        private const int MaxCount = 5000;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public RangeValueProvider()
            : base(ConditionType.Range, mayReturnMultipleValuesForRoot: true, isDeterministicForInput: true)
        {
            this.EnableInputArguments(required: true, mayReturnMultipleValues: true, maxPositionalArgs: 2);
            this.MarkReady(true);
        }

        /// <inheritdoc />
        public override bool UpdateContext(IContext context)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool TryValidateInput(IInputArguments input, [NotNullWhen(false)] out string? error)
        {
            if (!base.TryValidateInput(input, out error))
                return false;

            if (input.IsReady)
            {
                return
                    base.TryValidateInput(input, out error)
                    && this.TryParseRange(input, out _, out _, out error);
            }

            return true;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            return this.TryParseRange(input, out int min, out int max, out _)
                ? Enumerable.Range(start: min, count: max - min + 1).Select(p => p.ToString())
                : InvariantSets.Empty; // error will be shown in validation
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse the numeric min/max values from a range specifier if it's valid.</summary>
        /// <param name="input">The input arguments containing the range specifier.</param>
        /// <param name="min">The parsed min value, if valid.</param>
        /// <param name="max">The parsed max value, if valid.</param>
        /// <param name="error">The error indicating why the range is invalid, if applicable.</param>
        private bool TryParseRange(IInputArguments input, out int min, out int max, [NotNullWhen(false)] out string? error)
        {
            min = 0;
            max = 0;

            // check if input provided
            if (!input.HasPositionalArgs)
                return this.ParseError(input, $"token {this.Name} requires input arguments", out error);

            // validate length
            if (input.PositionalArgs.Length != 2)
                return this.ParseError(input, $"must specify a minimum and maximum value like {{{{{this.Name}:0,20}}}}", out error);

            // parse min/max values
            if (!int.TryParse(input.PositionalArgs[0], out min))
                return this.ParseError(input, $"can't parse min value '{input.PositionalArgs[0]}' as an integer", out error);
            if (!int.TryParse(input.PositionalArgs[1], out max))
                return this.ParseError(input, $"can't parse max value '{input.PositionalArgs[1]}' as an integer", out error);

            // validate range
            if (min > max)
                return this.ParseError(input, $"min value '{min}' can't be greater than max value '{max}'", out error);

            int count = (max - min) + 1;
            if (count > RangeValueProvider.MaxCount)
                return this.ParseError(input, $"range can't exceed {RangeValueProvider.MaxCount} numbers (specified range would contain {count} numbers)", out error);

            error = null;
            return true;
        }

        /// <summary>Build an error message for a parse issue.</summary>
        /// <param name="input">The input for which parsing failed.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="error">The constructed error message.</param>
        /// <returns>Returns <c>false</c> for convenience.</returns>
        private bool ParseError(IInputArguments input, string message, out string error)
        {
            error = $"invalid input('{input.TokenString}'), {message}.";
            return false;
        }
    }
}
