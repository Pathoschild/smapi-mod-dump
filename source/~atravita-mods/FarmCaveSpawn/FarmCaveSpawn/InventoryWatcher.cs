/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraShared.Wrappers;

using StardewModdingAPI.Events;

namespace FarmCaveSpawn;

/// <summary>
/// The inventory data model.
/// </summary>
public sealed class InventoryManagerModel
{
    public HashSet<string> Saplings { get; set; } = new();
}

/// <summary>
/// Manages watching the inventory for progression mode.
/// </summary>
internal static class InventoryWatcher
{
    private const string SaveString = "InventoryModel";
    private const string DATAPACKAGE = "DATAPACKAGE";
    private const string SAPLING = "SAPLING";
    private const string FRUIT = "FRUIT";

    private static InventoryManagerModel? model;
    private static string UniqueID = null!;

    /// <summary>
    /// Sets useful fields.
    /// </summary>
    /// <param name="uniqueID">Unique ID of this mod.</param>
    internal static void Initialize(string uniqueID) => UniqueID = uniqueID;

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    internal static void Load(IMultiplayerHelper multi, IDataHelper data)
    {
        if (Context.IsMainPlayer)
        {
            model = data.ReadSaveData<InventoryManagerModel>(SaveString) ?? new();

            multi.SendMessage(
                message: model,
                messageType: DATAPACKAGE,
                modIDs: new[] { UniqueID },
                playerIDs: multi.GetConnectedPlayers().Where(p => !p.IsSplitScreen).Select(p => p.PlayerID).ToArray());
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    internal static void Saving(IDataHelper data)
    {
        if (Context.IsMainPlayer && model is not null)
        {
            data.WriteSaveData(SaveString, model);
        }
    }

    /// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
    /// <remarks>Is used to keep track of saplings that have entered the inventory.</remarks>
    internal static void Watch(InventoryChangedEventArgs e, IMultiplayerHelper multi)
    {
        foreach (Item item in e.Added)
        {
            if (item is SObject obj && obj.isSapling() && Game1Wrappers.ObjectInfo.TryGetValue(obj.ParentSheetIndex, out string? data))
            {
                string name = data.GetNthChunk('/').ToString();
                if (!string.IsNullOrWhiteSpace(name) && model?.Saplings?.Add(name) == true)
                {
                    multi.SendMessage(name, SAPLING, new[] { UniqueID });
                    ModEntry.RequestFruitListReset();
                }
            }
        }
    }

    /// <summary>
    /// Checks whether or not a specific <paramref name="parentSheetIndex"/> corresponds to a sapling seen before.
    /// </summary>
    /// <param name="parentSheetIndex">Parent sheet index to check.</param>
    /// <returns>If the sapling has been viewed.</returns>
    internal static bool HaveSeen(int parentSheetIndex)
    {
        if (Game1Wrappers.ObjectInfo.TryGetValue(parentSheetIndex, out string? data))
        {
            string name = data.GetNthChunk('/').ToString();
            return model?.Saplings?.Contains(name) == true;
        }
        return false;
    }

    internal static void OnPeerConnected(PeerConnectedEventArgs e, IMultiplayerHelper multi)
    {
        if (Context.IsMainPlayer && model is not null)
        {
           multi.SendMessage(
                message: model,
                messageType: DATAPACKAGE,
                modIDs: new[] { UniqueID },
                playerIDs: new[] { e.Peer.PlayerID });
        }
    }

    internal static void OnModMessageRecieved(ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueID || Context.ScreenId != 0)
        {
            return;
        }

        switch (e.Type)
        {
            case DATAPACKAGE:
            {
                model = e.ReadAs<InventoryManagerModel>();
                break;
            }
            case SAPLING:
            {
                string name = e.ReadAs<string>();
                if (model?.Saplings?.Add(name) == true)
                {
                    ModEntry.RequestFruitListReset();
                }
                break;
            }
        }
    }
}
