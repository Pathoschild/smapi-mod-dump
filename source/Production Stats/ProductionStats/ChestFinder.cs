/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;
using ProductionStats.Common;
using ProductionStats.Containers;

namespace ProductionStats;

internal class ChestFinder(IMultiplayerHelper multiplayer)
{
    private readonly IMultiplayerHelper _multiplayer = multiplayer;

    public IEnumerable<ManagedStorage> GetChests()
    {
        IEnumerable<SObject> objects = GetAccessibleLocations()
            .Select(x => x.objects.Pairs)
            .SelectMany(x => x)
            .Select(x => x.Value);
        
        // chests in location
        foreach (SObject obj in objects)
        {
            // chests
            if (obj is Chest chest && chest.playerChest.Value)
            {
                yield return new ManagedStorage(
                    container: new ChestContainer(chest)
                );
            }
            // auto-grabbers
            else if (obj.QualifiedItemId == "(BC)165" && obj.heldObject.Value is Chest grabberChest)
            {
                yield return new ManagedStorage(
                    container: new ChestContainer(grabberChest)
                );
            }
        }

        foreach (GameLocation location in GetAccessibleLocations())
        {
            // farmhouse fridge
            Chest? fridge = GetStaticFridge(location);
            if (fridge != null)
            {
                yield return new ManagedStorage(
                    container: new ChestContainer(fridge)
                );
            }

            // dressers
            foreach (StorageFurniture furniture in location.furniture.OfType<StorageFurniture>())
            {
                yield return new ManagedStorage(
                    container: new FurnitureContainer(furniture)
                );
            }

            // buildings
            foreach (Building building in location.buildings)
            {
                if (building is JunimoHut hut)
                {
                    yield return new ManagedStorage(
                        container: new ChestContainer(hut.GetOutputChest())
                    );
                }
            }

            // shipping bin
            if (HasShippingBin(location))
            {
                yield return new ManagedStorage(
                    container: new ShippingBinContainer(location)
                );
            }
        }
    }

    /// <summary>Get the static fridge for a location, if any.</summary>
    /// <param name="location">The location to check.</param>
    private static Chest? GetStaticFridge(GameLocation location)
    {
        // main farmhouse or cabin
        if (location is FarmHouse house && house.fridgePosition != Point.Zero)
        {
            return house.fridge.Value;
        }

        // island farmhouse
        if (location is IslandFarmHouse islandHouse && islandHouse.visited.Value)
        {
            return islandHouse.fridge.Value;
        }

        return null;
    }

    /// <summary>Whether the location has a predefined shipping bin.</summary>
    /// <param name="location">The location to check.</param>
    private static bool HasShippingBin(GameLocation location)
    {
        return location switch
        {
            Farm => true,
            IslandWest islandFarm => islandFarm.farmhouseRestored.Value,
            _ => false
        };
    }

    /// <summary>
    ///     Get the locations which are accessible to the current player (regardless of settings).
    /// </summary>
    private IEnumerable<GameLocation> GetAccessibleLocations()
    {
        return Context.IsMainPlayer
            ? CommonHelper.GetLocations()
            : _multiplayer.GetActiveLocations();
    }

    public override bool Equals(object? obj)
    {
        return obj is ChestFinder finder &&
               EqualityComparer<IMultiplayerHelper>.Default.Equals(_multiplayer, finder._multiplayer);
    }

    public override int GetHashCode() => base.GetHashCode();
}