/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using AtraShared.Utils.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles fish food for multiplayer.
/// </summary>
internal static class FishFoodHandler
{
    private const string FISHFOODSAVESTRING = "atravita.MoreFertilizers.FishFood";

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
        => multiplayer.SendMessage(
            UnsavedLocHandler,
            "DATAPACKAGE",
            new[] { ModEntry.UNIQUEID },
            playerIDs ?? multiplayer.GetConnectedPlayers().Select((player) => player.PlayerID).ToArray());

    /// <summary>
    /// Recieves data from a different player and updates the saved locations.
    /// </summary>
    /// <param name="e">Mod message recieved event args.</param>
    internal static void RecieveHandler(ModMessageReceivedEventArgs e)
    {
        if (e.Type is "DATAPACKAGE")
        {
            UnsavedLocHandler = e.ReadAs<UnsavedLocationsHandler>();
        }
    }

    /// <summary>
    /// Re-adds location data for places that get wiped, like the MineShaft and VolcanoDungeon.
    /// </summary>
    /// <param name="e">OnWapred event args.</param>
    internal static void HandleWarp(WarpedEventArgs e)
    {
        if (e.IsLocalPlayer && e.NewLocation is MineShaft or VolcanoDungeon
            && UnsavedLocHandler.FishFoodLocationMap.TryGetValue(e.NewLocation.NameOrUniqueName, out int val) && val > 0)
        {
            e.NewLocation.modData?.SetInt(CanPlaceHandler.FishFood, val);
            e.NewLocation.waterColor.Value = SpecialFertilizerApplication.FedFishWaterColor();
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
        BroadcastHandler(multiplayer);

        helper.WriteSaveData(FISHFOODSAVESTRING, newHandler);
        Utility.ForAllLocations((GameLocation loc) =>
        {
            if (loc.modData?.GetInt(CanPlaceHandler.FishFood) is int value)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Decrementing FishFood at {loc.NameOrUniqueName} - {value}", LogLevel.Debug);
                loc.modData.SetInt(CanPlaceHandler.FishFood, Math.Max(value - 1, 0), defaultVal: 0);
            }
        });
    }

    /// <summary>
    /// A class that holds information to save into the save folder.
    /// </summary>
    public class UnsavedLocationsHandler
    {
        /// <summary>
        /// Gets or sets holds a map from unsaved locations to the FishFood values.
        /// </summary>
        public Dictionary<string, int> FishFoodLocationMap { get; set; } = new();
    }
}