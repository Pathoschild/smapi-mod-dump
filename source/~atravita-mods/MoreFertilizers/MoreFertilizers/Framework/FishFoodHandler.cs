/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using AtraShared.Utils.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace MoreFertilizers.Framework;

public record LocationRecord(string locationName, int days);

/// <summary>
/// Handles fish food for multiplayer.
/// </summary>
internal static class FishFoodHandler
{
    private const string FISHFOODSAVESTRING = "atravita.MoreFertilizers.FishFood";

    private const string PACKAGENAME = "DATAPACKAGE";

    private const string LOCATIONBROADCAST = "LOCATIONPACKAGE";

    /// <summary>
    /// Gets or sets an instance of the location map.
    /// </summary>
    internal static UnsavedLocationsHandler UnsavedLocHandler { get; set; } = new();

    /// <summary>
    /// Loads the unsaved location handler from the save.
    /// And sends it to other players in multiplayer.
    /// </summary>
    /// <param name="helper">data helper.</param>
    /// <param name="multiplayer">multiplayer helper.</param>
    internal static void LoadHandler(IDataHelper helper, IMultiplayerHelper multiplayer)
    {
        if (Context.IsMainPlayer)
        {
            UnsavedLocHandler = helper.ReadSaveData<UnsavedLocationsHandler>(FISHFOODSAVESTRING) ?? new();
            BroadcastHandler(multiplayer);
        }
    }

    /// <summary>
    /// Broadcasts the saved data from one player to others.
    /// </summary>
    /// <param name="multiplayer">SMAPI's multiplayer helper.</param>
    /// <param name="playerIDs">Player IDs to send to.</param>
    internal static void BroadcastHandler(IMultiplayerHelper multiplayer, long[]? playerIDs = null)
    {
        if (Context.IsMultiplayer)
        {
            multiplayer.SendMessage(
                    UnsavedLocHandler,
                    PACKAGENAME,
                    new[] { ModEntry.UNIQUEID },
                    playerIDs ?? multiplayer.GetConnectedPlayers().Select((player) => player.PlayerID).ToArray());
        }
    }

    internal static void BroadcastSingle(IMultiplayerHelper multiplayer, string locName, int days, long[]? playerIDs = null)
    {
        if (Context.IsMultiplayer)
        {
            multiplayer.SendMessage(
                new LocationRecord(locName, days),
                LOCATIONBROADCAST,
                new[] { ModEntry.UNIQUEID },
                playerIDs ?? multiplayer.GetConnectedPlayers().Select((player) => player.PlayerID).ToArray());
        }
    }

    /// <summary>
    /// Recieves data from a different player and updates the saved locations.
    /// </summary>
    /// <param name="e">Mod message recieved event args.</param>
    internal static void RecieveHandler(ModMessageReceivedEventArgs e)
    {
        if (e.Type is PACKAGENAME)
        {
            UnsavedLocHandler = e.ReadAs<UnsavedLocationsHandler>();
        }
        else if (e.Type is LOCATIONBROADCAST)
        {
            LocationRecord? data = e.ReadAs<LocationRecord>();
            UnsavedLocHandler.FishFoodLocationMap[data.locationName] = data.days;
        }
    }

    /// <summary>
    /// Whether or not it's a location the game saves.
    /// </summary>
    /// <param name="loc">Game location.</param>
    /// <returns>true if it's not persisted.</returns>
    /// <remarks>Carries the no-inlining flag so other mods can patch this if necessary.</remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool IsUnsavedLocation(this GameLocation loc)
        => loc is MineShaft or VolcanoDungeon;

    /// <summary>
    /// Re-adds location data for places that get wiped, like the MineShaft and VolcanoDungeon.
    /// </summary>
    /// <param name="e">OnWarped event args.</param>
    internal static void HandleWarp(WarpedEventArgs e)
    {
        if (e.IsLocalPlayer && e.NewLocation.IsUnsavedLocation()
            && UnsavedLocHandler.FishFoodLocationMap.TryGetValue(e.NewLocation.NameOrUniqueName, out int val) && val > 0)
        {
            e.NewLocation.modData?.SetInt(CanPlaceHandler.FishFood, val);
            e.NewLocation.waterColor.Value = ModEntry.Config.WaterOverlayColor;
        }
    }

    /// <summary>
    /// Called at the end of the day, the main player decrements the counter for FishFood for each location.
    /// </summary>
    /// <param name="helper">Data helper.</param>
    /// <param name="multiplayer">Multiplayer helper.</param>
    internal static void DecrementAndSave(IDataHelper helper, IMultiplayerHelper multiplayer)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }
        UnsavedLocationsHandler newHandler = new();
        foreach ((string loc, int value) in UnsavedLocHandler.FishFoodLocationMap)
        {
            if (value > 1)
            {
                newHandler.FishFoodLocationMap[loc] = value - 1;
            }
        }
        UnsavedLocHandler = newHandler;

        Task broadcast = Task.Run(() => BroadcastHandler(multiplayer));

        helper.WriteSaveData(FISHFOODSAVESTRING, newHandler);

        Utility.ForAllLocations(static (GameLocation? loc) =>
        {
            if (loc?.modData?.GetInt(CanPlaceHandler.FishFood) is int value)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Decrementing FishFood at {loc.NameOrUniqueName} - {value}", LogLevel.Debug);
                loc.modData.SetInt(CanPlaceHandler.FishFood, Math.Max(value - 1, 0), defaultVal: 0);
            }
        });

        broadcast.Wait();
    }

    /// <summary>
    /// A class that holds information to save into the save folder.
    /// </summary>
    public sealed class UnsavedLocationsHandler
    {
        /// <summary>
        /// Gets or sets holds a map from unsaved locations to the FishFood values.
        /// </summary>
        public Dictionary<string, int> FishFoodLocationMap { get; set; } = new();
    }
}