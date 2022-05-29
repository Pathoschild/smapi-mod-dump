/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.TreasureHunt;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

using Framework.Events.TreasureHunt;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    /// <inheritdoc />
    public bool IsActive => TreasureTile is not null;

    /// <inheritdoc />
    public Vector2? TreasureTile { get; protected set; } = null;

    /// <inheritdoc cref="OnStarted"/>
    internal static event EventHandler<ITreasureHuntStartedEventArgs> Started;

    /// <inheritdoc cref="OnEnded"/>
    internal static event EventHandler<ITreasureHuntEndedEventArgs> Ended;

    protected uint elapsed;
    protected uint timeLimit;
    protected string huntStartedMessage;
    protected string huntFailedMessage;
    protected Rectangle iconSourceRect;
    protected GameLocation huntLocation;
    protected readonly Random random = new(Guid.NewGuid().GetHashCode());
    
    private double _chanceAccumulator = 1.0;

    #region public methods

    /// <inheritdoc />
    public abstract bool TryStart(GameLocation location);

    /// <inheritdoc />
    public abstract void ForceStart(GameLocation location, Vector2 target);

    /// <inheritdoc />
    public abstract void Fail();

    #endregion public methods

    #region internal methods

    /// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
    internal void ResetChanceAccumulator()
    {
        _chanceAccumulator = 1.0;
    }

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    internal void Update(uint ticks)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        if (ticks % 60 == 0 && ++elapsed > timeLimit) Fail();
        else CheckForCompletion();
    }

    #endregion internal methods

    #region protected methods

    /// <summary>Roll the dice for a new treasure hunt or adjust the odds for the next attempt.</summary>
    protected bool TryStart()
    {
        if (IsActive) return false;

        if (random.NextDouble() > ModEntry.Config.ChanceToStartTreasureHunt * _chanceAccumulator)
        {
            _chanceAccumulator *= 1.0 + Game1.player.DailyLuck;
            return false;
        }

        _chanceAccumulator = 1.0;
        return true;
    }

    /// <summary>Check if a treasure hunt can be started immediately and adjust the odds for the next attempt.</summary>
    protected virtual void ForceStart()
    {
        if (IsActive) throw new InvalidOperationException("A Treasure Hunt is already active in this instance.");
        _chanceAccumulator = 1.0;
    }

    /// <summary>Select a random tile and make sure it is a valid treasure target.</summary>
    /// <param name="location">The game location.</param>
    protected abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Check if the player has found the treasure tile.</summary>
    protected abstract void CheckForCompletion();

    /// <summary>Reset treasure tile and release treasure hunt update event.</summary>
    protected abstract void End(bool found);

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    protected void OnStarted(Farmer player, Vector2 target)
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(player, target));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    protected void OnEnded(Farmer player, bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(player, found));
    }

    #endregion protected methods
}