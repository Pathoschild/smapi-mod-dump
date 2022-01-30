/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley.Objects;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SDV.Shared.Abstractions
{
  public class FarmHouseWrapper : IFarmHouseWrapper
  {
    public FarmHouseWrapper(FarmHouse item) => GetBaseType = item;
    public FarmHouse GetBaseType { get; }
    public IFarmerWrapper owner { get; }
    public int upgradeLevel { get; set; }
    public IEnumerable<IChildWrapper> getChildren()
    {
      yield break;
    }

    public int getChildrenCount() => 0;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider,
      ICharacterWrapper character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false) =>
      false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider,
      ICharacterWrapper character) =>
      false;

    public bool isTileLocationTotallyClearAndPlaceable(Vector2 v) => false;

    public void performTenMinuteUpdate(int timeOfDay)
    {
    }

    public Point getFrontDoorSpot() => default;

    public Point getPorchStandingSpot() => default;

    public Point getKitchenStandingSpot() => default;

    public IBedFurnitureWrapper GetSpouseBed() => null;

    public Point getSpouseBedSpot(string spouseName) => default;

    public Point GetSpouseRoomSpot() => default;

    public IBedFurnitureWrapper GetBed(BedFurniture.BedType bed_type = BedFurniture.BedType.Any, int index = 0) => null;

    public Point GetPlayerBedSpot() => default;

    public IBedFurnitureWrapper GetPlayerBed() => null;

    public Point getBedSpot(BedFurniture.BedType bed_type = BedFurniture.BedType.Any) => default;

    public Point getEntryLocation() => default;

    public IBedFurnitureWrapper GetChildBed(int index) => null;

    public Point GetChildBedSpot(int index) => default;

    public bool isTilePlaceable(Vector2 v, IItemWrapper item = null) => false;

    public Point getRandomOpenPointInHouse(Random r, int buffer = 0, int tries = 30) => default;

    public bool performAction(string action, IFarmerWrapper who, Location tileLocation) => false;

    public bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who) => false;

    public bool hasActiveFireplace() => false;

    public void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
    }

    public void UpdateWhenCurrentLocation(GameTime time)
    {
    }

    public Point getFireplacePoint() => default;

    public bool shouldShowSpouseRoom() => false;

    public void showSpouseRoom()
    {
    }

    public void AddCellarTiles()
    {
    }

    public string GetCellarName() => null;

    public void UpdateForRenovation()
    {
    }

    public void updateFarmLayout()
    {
    }

    public void MakeMapModifications(bool force = false)
    {
    }

    public void PlaceInNearbySpace(Vector2 tileLocation, IObjectWrapper o)
    {
    }

    public void RefreshFloorObjectNeighbors()
    {
    }

    public void moveObjectsForHouseUpgrade(int whichUpgrade)
    {
    }

    public void drawAboveFrontLayer(SpriteBatch b)
    {
    }

    public void updateMap()
    {
    }

    public void setMapForUpgradeLevel(int level)
    {
    }

    public void createCellarWarps()
    {
    }

    public void updateCellarWarps()
    {
    }

    public void loadSpouseRoom()
    {
    }

    public Rectangle? GetCribBounds() => null;

    public Rectangle? GetBedBounds(int child_index = 0) => null;

    public Rectangle? GetChildBedBounds(int child_index = 0) => null;

    public void UpdateChildRoom()
    {
    }

    public void playerDivorced()
    {
    }

    public IEnumerable<Rectangle> getForbiddenPetWarpTiles() => null;

    public bool canPetWarpHere(Vector2 tile_position) => false;

    public IEnumerable<Rectangle> getWalls() => null;

    public void TransferDataFromSavedLocation(IGameLocationWrapper l)
    {
    }

    public IEnumerable<Rectangle> getFloors() => null;

    public bool CanModifyCrib() => false;
  }
}
