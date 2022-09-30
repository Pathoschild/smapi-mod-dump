/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewMods.BetterChests.Framework.Features;
using StardewMods.BetterChests.Framework.Handlers;
using StardewMods.BetterChests.Framework.Models;
using StardewMods.Common.Extensions;
using StardewMods.Common.Helpers;
using StardewMods.Common.Integrations.BetterChests;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

/// <summary>
///     Provides access to all supported storages in the game.
/// </summary>
internal sealed class Storages
{
#nullable disable
    private static Storages Instance;
#nullable enable

    private readonly ModConfig _config;
    private readonly Lazy<Dictionary<object, BaseStorage>> _referenceContext;

    private EventHandler<IStorageTypeRequestedEventArgs>? _storageTypeRequested;

    private Storages(ModConfig config)
    {
        this._config = config;
        this._referenceContext = new(
            () =>
            {
                var referenceContext = new Dictionary<object, BaseStorage>();
                foreach (var location in CommonHelpers.AllLocations)
                {
                    switch (location)
                    {
                        // Shipping Bin for Chests Anywhere
                        case Farm farm when !referenceContext.ContainsKey(farm):
                            var shippingBin = farm.buildings.OfType<ShippingBin>().FirstOrDefault();
                            if (shippingBin is not null)
                            {
                                referenceContext.Add(
                                    farm,
                                    new ShippingBinStorage(
                                        farm,
                                        new(
                                            shippingBin.tileX.Value + shippingBin.tilesWide.Value / 2,
                                            shippingBin.tileY.Value + shippingBin.tilesHigh.Value / 2)));
                            }

                            break;

                        // Fridge
                        case FarmHouse { fridge.Value: { } fridge, fridgePosition: var fridgePosition } farmHouse
                            when !referenceContext.ContainsKey(fridge) && !fridgePosition.Equals(Point.Zero):
                            referenceContext.Add(fridge, new FridgeStorage(farmHouse, fridgePosition.ToVector2()));
                            break;

                        // Island Fridge
                        case IslandFarmHouse
                            {
                                fridge.Value: { } islandFridge, fridgePosition: var islandFridgePosition,
                            } islandFarmHouse when !referenceContext.ContainsKey(islandFridge)
                                                && !islandFridgePosition.Equals(Point.Zero):
                            referenceContext.Add(
                                islandFridge,
                                new FridgeStorage(islandFarmHouse, islandFridgePosition.ToVector2()));
                            break;
                    }
                }

                return referenceContext;
            });
    }

    /// <summary>
    ///     Event for when a storage type is assigned to a storage object.
    /// </summary>
    public static event EventHandler<IStorageTypeRequestedEventArgs>? StorageTypeRequested
    {
        add => Storages.Instance._storageTypeRequested += value;
        remove => Storages.Instance._storageTypeRequested -= value;
    }


    /// <summary>
    ///     Gets storages from all locations and farmer inventory in the game.
    /// </summary>
    public static IEnumerable<BaseStorage> All
    {
        get
        {
            var excluded = new HashSet<object>();
            var storages = new List<BaseStorage>();

            // Iterate Inventory
            foreach (var storage in Storages.FromPlayer(Game1.player, excluded))
            {
                storages.Add(storage);
                yield return storage;
            }

            // Iterate Locations
            foreach (var location in CommonHelpers.AllLocations)
            {
                foreach (var storage in Storages.FromLocation(location, excluded))
                {
                    storages.Add(storage);
                    yield return storage;
                }
            }

            // Sub Storage
            foreach (var storage in storages.SelectMany(
                         managedStorage => Storages.FromStorage(managedStorage, excluded)))
            {
                yield return storage;
            }
        }
    }

    /// <summary>
    ///     Gets the current storage item from the farmer's inventory.
    /// </summary>
    public static BaseStorage? CurrentItem =>
        Game1.player.CurrentItem is not null && Storages.TryGetOne(Game1.player.CurrentItem, out var storage)
            ? Storages.Instance.GetStorageType(storage)
            : null;

    /// <summary>
    ///     Gets all placed storages in the current location.
    /// </summary>
    public static IEnumerable<BaseStorage> CurrentLocation => Storages.FromLocation(Game1.currentLocation);

    /// <summary>
    ///     Gets storages in the farmer's inventory.
    /// </summary>
    public static IEnumerable<BaseStorage> Inventory => Storages.FromPlayer(Game1.player);

    private static ModConfig Config => Storages.Instance._config;

    private static Dictionary<object, BaseStorage> ReferenceContext =>
        Storages.Instance._referenceContext.Value;

    /// <summary>
    ///     Gets all storages placed in a particular location.
    /// </summary>
    /// <param name="location">The location to get storages from.</param>
    /// <param name="excluded">A list of storage contexts to exclude to prevent iterating over the same object.</param>
    /// <returns>An enumerable of all placed storages at the location.</returns>
    public static IEnumerable<BaseStorage> FromLocation(GameLocation location, ISet<object>? excluded = null)
    {
        excluded ??= new HashSet<object>();
        if (excluded.Contains(location))
        {
            return Array.Empty<BaseStorage>();
        }

        excluded.Add(location);

        IEnumerable<BaseStorage> GetAll()
        {
            // Mod Integrations
            foreach (var storage in Integrations.FromLocation(location, excluded))
            {
                yield return storage;
            }

            // Special Locations
            switch (location)
            {
                case FarmHouse { fridge.Value: { } fridge } farmHouse
                    when !excluded.Contains(fridge) && !farmHouse.fridgePosition.Equals(Point.Zero):
                    excluded.Add(fridge);
                    yield return new FridgeStorage(farmHouse, farmHouse.fridgePosition.ToVector2());
                    break;
                case IslandFarmHouse { fridge.Value: { } fridge } islandFarmHouse when !excluded.Contains(fridge)
                 && !islandFarmHouse.fridgePosition.Equals(Point.Zero):
                    excluded.Add(fridge);
                    yield return new FridgeStorage(islandFarmHouse, islandFarmHouse.fridgePosition.ToVector2());
                    break;
                case IslandWest islandWest:
                    excluded.Add(islandWest);
                    yield return new ShippingBinStorage(islandWest, islandWest.shippingBinPosition.ToVector2());
                    break;
            }

            if (location is BuildableGameLocation buildableGameLocation)
            {
                // Buildings
                foreach (var building in buildableGameLocation.buildings)
                {
                    // Special Buildings
                    switch (building)
                    {
                        case JunimoHut junimoHut when !excluded.Contains(junimoHut):
                            excluded.Add(junimoHut);
                            yield return new JunimoHutStorage(
                                junimoHut,
                                location,
                                new(
                                    building.tileX.Value + building.tilesWide.Value / 2,
                                    building.tileY.Value + building.tilesHigh.Value / 2));
                            break;
                        case ShippingBin shippingBin when !excluded.Contains(shippingBin):
                            excluded.Add(shippingBin);
                            yield return new ShippingBinStorage(
                                shippingBin,
                                location,
                                new(
                                    building.tileX.Value + building.tilesWide.Value / 2,
                                    building.tileY.Value + building.tilesHigh.Value / 2));
                            break;
                    }
                }
            }

            // Objects
            foreach (var (position, obj) in location.Objects.Pairs)
            {
                if (!Storages.TryGetOne(obj, location, position, out var subStorage)
                 || excluded.Contains(subStorage.Context))
                {
                    continue;
                }

                excluded.Add(subStorage.Context);
                yield return subStorage;
            }
        }

        return Storages.Instance.GetStorageTypes(GetAll());
    }

    /// <summary>
    ///     Gets all storages placed in a particular farmer's inventory.
    /// </summary>
    /// <param name="player">The farmer to get storages from.</param>
    /// <param name="excluded">A list of storage contexts to exclude to prevent iterating over the same object.</param>
    /// <param name="limit">Limit the number of items from the farmer's inventory.</param>
    /// <returns>An enumerable of all held storages in the farmer's inventory.</returns>
    public static IEnumerable<BaseStorage> FromPlayer(Farmer player, ISet<object>? excluded = null, int? limit = null)
    {
        excluded ??= new HashSet<object>();
        if (excluded.Contains(player))
        {
            return Array.Empty<BaseStorage>();
        }

        excluded.Add(player);

        IEnumerable<BaseStorage> GetAll()
        {
            // Mod Integrations
            foreach (var storage in Integrations.FromPlayer(player, excluded))
            {
                yield return storage;
            }

            limit ??= player.MaxItems;
            var position = player.getTileLocation();
            for (var index = 0; index < limit; ++index)
            {
                var item = player.Items[index];
                if (!Storages.TryGetOne(item, player, position, out var storage) || excluded.Contains(storage.Context))
                {
                    continue;
                }

                excluded.Add(storage.Context);
                yield return storage;
            }
        }

        return Storages.Instance.GetStorageTypes(GetAll());
    }

    /// <summary>
    ///     Initialized <see cref="Storages" />.
    /// </summary>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="Storages" /> class.</returns>
    public static Storages Init(ModConfig config)
    {
        return Storages.Instance ??= new(config);
    }

    /// <summary>
    ///     Attempt to gets a placed storage at a specific position.
    /// </summary>
    /// <param name="location">The location to get the storage from.</param>
    /// <param name="pos">The position to get the storage from.</param>
    /// <param name="storage">The storage object.</param>
    /// <returns>Returns true if a storage could be found at the location and position..</returns>
    public static bool TryGetOne(GameLocation location, Vector2 pos, [NotNullWhen(true)] out BaseStorage? storage)
    {
        storage = Storages.FromLocation(location)
                          .FirstOrDefault(locationStorage => locationStorage.Position.Equals(pos));
        return storage is not null;
    }

    /// <summary>
    ///     Attempts to retrieve a storage based on a context object.
    /// </summary>
    /// <param name="context">The context object.</param>
    /// <param name="storage">The storage object.</param>
    /// <returns>Returns true if a storage could be found for the context object.</returns>
    public static bool TryGetOne(object? context, [NotNullWhen(true)] out BaseStorage? storage)
    {
        if (context is BaseStorage baseStorage)
        {
            storage = baseStorage;
            return true;
        }

        if (!Integrations.TryGetOne(context, out storage)
         && !Storages.TryGetOne(context, default, default, out storage))
        {
            return false;
        }

        if (storage is ShippingBinStorage { Context: not Chest }
         && Integrations.TestConflicts(nameof(BetterShippingBin), out _))
        {
            storage = null;
            return false;
        }

        Storages.Instance.GetStorageType(storage);
        return true;
    }

    private static IEnumerable<BaseStorage> FromStorage(BaseStorage storage, ISet<object>? excluded = null)
    {
        excluded ??= new HashSet<object>();
        if (excluded.Contains(storage.Context))
        {
            return Array.Empty<BaseStorage>();
        }

        excluded.Add(storage.Context);

        IEnumerable<BaseStorage> GetAll()
        {
            var managedStorages = new List<BaseStorage>();

            foreach (var item in storage.Items.Where(item => item is not null && !excluded.Contains(item)))
            {
                if (!Storages.TryGetOne(item, storage.Source, storage.Position, out var managedStorage)
                 || excluded.Contains(managedStorage.Context))
                {
                    continue;
                }

                excluded.Add(managedStorage.Context);
                managedStorages.Add(managedStorage);
                yield return managedStorage;
            }

            // Sub Storage
            foreach (var subStorage in managedStorages.SelectMany(
                         managedStorage => Storages.FromStorage(managedStorage, excluded)))
            {
                yield return subStorage;
            }
        }

        return Storages.Instance.GetStorageTypes(GetAll());
    }

    private static bool TryGetOne(
        object? context,
        object? parent,
        Vector2 position,
        [NotNullWhen(true)] out BaseStorage? storage)
    {
        if (context is not null && Storages.ReferenceContext.TryGetValue(context, out storage))
        {
            return true;
        }

        switch (context)
        {
            case BaseStorage storageObject:
                storage = storageObject;
                return true;
            case SObject { ParentSheetIndex: 165, heldObject.Value: Chest } heldObj:
                storage = new ObjectStorage(heldObj, parent, position);
                return true;
            case Chest { SpecialChestType: Chest.SpecialChestTypes.MiniShippingBin } shippingChest:
                storage = new ShippingBinStorage(shippingChest, parent, position);
                return true;
            case Chest { playerChest.Value: true } chest:
                storage = new ChestStorage(chest, parent, position);
                return true;
            case ShippingBin or IslandWest when !Storages.Config.BetterShippingBin:
                storage = default;
                return false;
            case ShippingBin shippingBin:
                storage = new ShippingBinStorage(shippingBin, parent, position);
                return true;
            case JunimoHut junimoHut:
                storage = new JunimoHutStorage(junimoHut, parent, position);
                return true;
            case FarmHouse { fridge.Value: { } } farmHouse when !farmHouse.fridgePosition.Equals(Point.Zero):
                storage = new FridgeStorage(farmHouse, position);
                return true;
            case IslandFarmHouse { fridge.Value: { } } islandFarmHouse
                when !islandFarmHouse.fridgePosition.Equals(Point.Zero):
                storage = new FridgeStorage(islandFarmHouse, position);
                return true;
            case IslandWest islandWest:
                storage = new ShippingBinStorage(islandWest, position);
                return true;
            default:
                storage = default;
                return false;
        }
    }

    private BaseStorage GetStorageType(BaseStorage storage)
    {
        if (storage is not StorageNode storageNode)
        {
            return storage;
        }

        var storageTypes = new List<IStorageData>();
        var storageTypeRequestedEventArgs = new StorageTypeRequestedEventArgs(storage.Context, storageTypes);
        this._storageTypeRequested.InvokeAll(this, storageTypeRequestedEventArgs);
        var storageType = storageTypes.FirstOrDefault();
        storageNode.Parent = storageType is not null ? new StorageNode(storageType, this._config) : this._config;
        return storage;
    }

    private IEnumerable<BaseStorage> GetStorageTypes(IEnumerable<BaseStorage> storages)
    {
        return storages.Select(this.GetStorageType);
    }
}