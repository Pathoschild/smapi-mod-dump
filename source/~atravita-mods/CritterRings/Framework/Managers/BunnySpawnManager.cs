/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using StardewModdingAPI.Events;

using StardewValley.TerrainFeatures;

namespace CritterRings.Framework.Managers;

/// <summary>
/// Monitors bushes.
/// The vast majority of players will not see a bush change at all in their game, definitely not every ten in-game minutes
/// So this tracker exists to prevent us from repeatedly allocating.
/// </summary>
internal sealed class BunnySpawnManager : IDisposable
{
    private IPlayerEvents playerEvents;
    private WeakReference<Farmer> farmerRef;
    private WeakReference<LargeObjectListener> listener;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BunnySpawnManager"/> class.
    /// </summary>
    /// <param name="monitor">Logging instance.</param>
    /// <param name="farmer">Farmer to follow.</param>
    /// <param name="events">Player warped events.</param>
    internal BunnySpawnManager(IMonitor monitor, Farmer farmer, IPlayerEvents events)
    {
        this.farmerRef = new(farmer);
        this.listener = new(new(monitor, farmer.currentLocation));
        this.playerEvents = events;

        this.playerEvents.Warped += this.OnWarp;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="BunnySpawnManager"/> class.
    /// </summary>
    ~BunnySpawnManager() => this.Dispose(disposing: false);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Checks to see if this instance is still valid.
    /// </summary>
    /// <returns>True if not disposed and the farmer reference is still valid.</returns>
    internal bool IsValid()
        => !this.disposedValue
        && this.farmerRef?.TryGetTarget(out Farmer? farmer) == true && farmer is not null;

    /// <summary>
    /// Gets a list of bushes tracked by this instances.
    /// </summary>
    /// <returns>The bushes tracked by this instance.</returns>
    internal List<Bush>? GetTrackedBushes()
    {
        if (this.farmerRef?.TryGetTarget(out Farmer? farmer) != true || farmer is null)
        {
            return null;
        }
        if (this.listener?.TryGetTarget(out LargeObjectListener? watcher) != true || watcher is null)
        {
            ModEntry.ModMonitor.DebugOnlyLog($"Refreshing watcher on {farmer.Name}");
            watcher = new(ModEntry.ModMonitor, farmer.currentLocation);
            this.listener = new(watcher);
        }
        return watcher.GetBushes();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// <param name="disposing">Whether or not this is called from the Dispose function or by the finalizer.</param>
    private void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (this.listener?.TryGetTarget(out LargeObjectListener? watcher) == true)
            {
                watcher?.Dispose();
            }
            this.playerEvents.Warped -= this.OnWarp;
            this.playerEvents = null!;
            this.farmerRef = null!;
            this.listener = null!;
            this.disposedValue = true;
        }
    }

    private void OnWarp(object? sender, WarpedEventArgs e)
    {
        if (this.farmerRef?.TryGetTarget(out Farmer? farmer) != true || this.disposedValue || farmer is null
            || !ReferenceEquals(farmer, e.Player))
        {
            ModEntry.ModMonitor.DebugOnlyLog($"Warp event raised, not for us", LogLevel.Info);
            return;
        }

        if (this.listener?.TryGetTarget(out LargeObjectListener? watcher) == true)
        {
            watcher.ChangeLocation(e.NewLocation);
        }
    }

    /// <summary>
    /// A class to hook into <see cref="GameLocation.largeTerrainFeatures" />'s events.
    /// Do not maintain a strong reference to this to allow the GC to collect if needed.
    /// </summary>
    private sealed class LargeObjectListener : IDisposable
    {
        private IMonitor monitor;
        private List<Bush>? watchedBushes;
        private GameLocation? location;
        private bool disposedValue;

        public LargeObjectListener(IMonitor monitor, GameLocation? location)
        {
            this.monitor = monitor;
            this.location = location;
            if (location?.largeTerrainFeatures is not null)
            {
                location.largeTerrainFeatures.OnValueAdded += this.OnValueAdded;
                location.largeTerrainFeatures.OnValueRemoved += this.OnValueRemoved;
            }
        }

        ~LargeObjectListener() => this.Dispose(false);

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Changes this watcher to watch a new location.
        /// </summary>
        /// <param name="newLocation">new location to watch.</param>
        /// <returns>this.</returns>
        internal LargeObjectListener ChangeLocation(GameLocation newLocation)
        {
            this.monitor.DebugOnlyLog($"(Bunny Ring) Changing bushwatcher {this.location?.NameOrUniqueName} -> {newLocation.NameOrUniqueName}");

            if (newLocation.largeTerrainFeatures is not null)
            {
                newLocation.largeTerrainFeatures.OnValueAdded += this.OnValueAdded;
                newLocation.largeTerrainFeatures.OnValueRemoved += this.OnValueRemoved;
            }

            if (this.location is not null)
            {
                this.location.largeTerrainFeatures.OnValueAdded -= this.OnValueAdded;
                this.location.largeTerrainFeatures.OnValueRemoved -= this.OnValueRemoved;
            }
            this.location = newLocation;
            this.watchedBushes = null;
            return this;
        }

        internal List<Bush>? GetBushes()
        {
            if (this.watchedBushes is null && this.location?.largeTerrainFeatures?.Count is > 0)
            {
                this.watchedBushes = this.location?.largeTerrainFeatures?.OfType<Bush>()?.ToList();
            }
            return this.watchedBushes;
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (this.location?.largeTerrainFeatures is { } features)
                {
                    features.OnValueAdded -= this.OnValueAdded;
                    features.OnValueRemoved -= this.OnValueRemoved;
                }
                this.location = null!;
                this.monitor = null!;
                this.watchedBushes = null;
                this.disposedValue = true;
            }
        }

        private void OnValueAdded(LargeTerrainFeature feature)
        {
            if (feature is Bush bush)
            {
                this.monitor.DebugOnlyLog($"Bush added at {bush.tilePosition}");
                this.watchedBushes ??= this.location?.largeTerrainFeatures?.OfType<Bush>()?.ToList() ?? new();
                this.watchedBushes.Add(bush);
            }
        }

        private void OnValueRemoved(LargeTerrainFeature feature)
        {
            if (feature is Bush bush && this.watchedBushes?.Remove(bush) == true)
            {
                this.monitor.DebugOnlyLog($"Bush removed at {bush.tilePosition}");
            }
        }
    }
}
