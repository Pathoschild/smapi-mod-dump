/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.Constants;
using Pathoschild.Stardew.Common.Utilities;
using StardewValley;

namespace ContentPatcher.Framework.Tokens.ValueProviders.Players
{
    /// <summary>A value provider for a built-in condition whose value may change with the context and which c, and accepts flags to indicate whether to check the host or local player.</summary>
    internal class PerPlayerValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>Handles reading info from the current save.</summary>
        private readonly TokenSaveReader SaveReader;

        /// <summary>The allowed root values (or <c>null</c> if any value is allowed).</summary>
        private readonly InvariantHashSet AllowedRootValues;

        /// <summary>Get the current values for a player.</summary>
        private readonly Func<Farmer, InvariantHashSet> FetchValues;

        /// <summary>The values as of the last context update.</summary>
        private readonly IDictionary<long, InvariantHashSet> Values = new Dictionary<long, InvariantHashSet>();

        /// <summary>The player ID for the host player.</summary>
        private long HostPlayerId;

        /// <summary>The player ID for the local player.</summary>
        private long LocalPlayerId;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="type">The condition type.</param>
        /// <param name="values">Get the current values.</param>
        /// <param name="saveReader">Handles reading info from the current save.</param>
        /// <param name="mayReturnMultipleValues">Whether the root may contain multiple values.</param>
        /// <param name="allowedValues">The allowed values (or <c>null</c> if any value is allowed).</param>
        public PerPlayerValueProvider(ConditionType type, Func<Farmer, IEnumerable<string>> values, TokenSaveReader saveReader, bool mayReturnMultipleValues = false, IEnumerable<string> allowedValues = null)
            : base(type, mayReturnMultipleValues)
        {
            this.SaveReader = saveReader;
            this.AllowedRootValues = allowedValues != null ? new InvariantHashSet(allowedValues) : null;
            this.FetchValues = player => new InvariantHashSet(values(player));
            this.EnableInputArguments(required: false, mayReturnMultipleValues: mayReturnMultipleValues, maxPositionalArgs: null);
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="type">The condition type.</param>
        /// <param name="value">Get the current value.</param>
        /// <param name="saveReader">Handles reading info from the current save.</param>
        /// <param name="mayReturnMultipleValues">Whether the root may contain multiple values.</param>
        /// <param name="allowedValues">The allowed values (or <c>null</c> if any value is allowed).</param>
        public PerPlayerValueProvider(ConditionType type, Func<Farmer, string> value, TokenSaveReader saveReader, bool mayReturnMultipleValues = false, IEnumerable<string> allowedValues = null)
            : this(type, player => new[] { value(player) }, saveReader, mayReturnMultipleValues, allowedValues) { }

        /// <inheritdoc />
        public override bool UpdateContext(IContext context)
        {
            return this.IsChanged(() =>
            {
                bool changed = false;

                if (this.MarkReady(this.SaveReader.IsReady))
                {
                    // update host/local player IDs
                    long hostId = this.SaveReader.GetPlayer(PlayerType.HostPlayer).UniqueMultiplayerID;
                    long localId = this.SaveReader.GetPlayer(PlayerType.CurrentPlayer).UniqueMultiplayerID;

                    changed |= this.HostPlayerId != hostId || this.LocalPlayerId != localId;

                    this.HostPlayerId = hostId;
                    this.LocalPlayerId = localId;

                    // update values by player ID
                    HashSet<long> removeIds = new HashSet<long>(this.Values.Keys);
                    foreach (Farmer player in this.SaveReader.GetAllPlayers())
                    {
                        // get values
                        long id = player.UniqueMultiplayerID;
                        InvariantHashSet newValues = this.FetchValues(player);
                        if (!this.Values.TryGetValue(id, out InvariantHashSet oldValues))
                            oldValues = null;

                        // track changes
                        removeIds.Remove(id);
                        changed |= oldValues == null || this.IsChanged(oldValues, newValues);

                        // update
                        this.Values[id] = newValues;
                    }

                    // remove players if needed
                    foreach (long id in removeIds)
                    {
                        this.Values.Remove(id);
                        changed = true;
                    }
                }
                else
                {
                    this.Values.Clear();
                    this.HostPlayerId = 0;
                    this.LocalPlayerId = 0;
                }

                return changed;
            });
        }

        /// <inheritdoc />
        public override bool TryValidateInput(IInputArguments input, out string error)
        {
            return
                base.TryValidateInput(input, out error)
                && (
                    !input.HasPositionalArgs
                    || this.TryParseInput(input, out _, out error, testOnly: true)
                );
        }

        /// <inheritdoc />
        public override bool HasBoundedValues(IInputArguments input, out InvariantHashSet allowedValues)
        {
            allowedValues = this.AllowedRootValues;
            return allowedValues != null;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            return this.TryParseInput(input, out ISet<long> playerIds, out _)
                ? this.GetValuesFor(playerIds)
                : Enumerable.Empty<string>();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse the input arguments if valid.</summary>
        /// <param name="input">The input arguments.</param>
        /// <param name="playerIds">The parsed player IDs.</param>
        /// <param name="error">The error indicating why the input is invalid, if applicable.</param>
        /// <param name="testOnly">Whether we're only testing that the input is valid, so <paramref name="playerIds"/> doesn't need to be populated.</param>
        /// <returns>Returns whether the input is valid.</returns>
        private bool TryParseInput(IInputArguments input, out ISet<long> playerIds, out string error, bool testOnly = false)
        {
            playerIds = !testOnly
                ? new HashSet<long>()
                : null;

            foreach (string arg in input.PositionalArgs)
            {
                if (long.TryParse(arg, out long playerId))
                {
                    if (!testOnly)
                        playerIds.Add(playerId);
                }
                else if (Enum.TryParse(arg, ignoreCase: true, out PlayerType type))
                {
                    if (!testOnly)
                    {
                        switch (type)
                        {
                            case PlayerType.HostPlayer:
                                playerIds.Add(this.HostPlayerId);
                                break;

                            case PlayerType.CurrentPlayer:
                                playerIds.Add(this.LocalPlayerId);
                                break;

                            case PlayerType.AnyPlayer:
                                playerIds.AddMany(this.Values.Keys);
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown player type {type}.");
                        }
                    }
                }
                else
                {
                    error = $"invalid input arguments ({input.TokenString}) for {this.Name} token, expected any combination of '{string.Join("', '", Enum.GetNames(typeof(PlayerType)))}', or player IDs.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        /// <summary>Get the cached values for the given player IDs.</summary>
        /// <param name="playerIds">The player IDs.</param>
        private IEnumerable<string> GetValuesFor(ISet<long> playerIds)
        {
            // default to current player
            if (!playerIds.Any())
                playerIds.Add(this.LocalPlayerId);

            // get single value (avoids copying the collection in most cases)
            if (playerIds.Count == 1)
            {
                return this.Values.TryGetValue(playerIds.First(), out InvariantHashSet set)
                    ? set
                    : Enumerable.Empty<string>();
            }

            // get multiple values
            HashSet<string> values = new();
            foreach (long id in playerIds)
            {
                if (this.Values.TryGetValue(id, out InvariantHashSet set))
                    values.AddMany(set);
            }
            return values;
        }
    }
}
