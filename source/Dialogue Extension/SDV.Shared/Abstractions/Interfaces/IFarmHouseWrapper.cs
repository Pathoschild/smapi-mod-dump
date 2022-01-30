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

namespace SDV.Shared.Abstractions
{
  public interface IFarmHouseWrapper : IWrappedType<FarmHouse>
  {
    IFarmerWrapper owner { get; }
    int upgradeLevel { get; set; }
    IEnumerable<IChildWrapper> getChildren();
    int getChildrenCount();

    bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      ICharacterWrapper character,
      bool pathfinding,
      bool projectile = false,
      bool ignoreCharacterRequirement = false);

    bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      ICharacterWrapper character);

    bool isTileLocationTotallyClearAndPlaceable(Vector2 v);
    void performTenMinuteUpdate(int timeOfDay);
    Point getFrontDoorSpot();
    Point getPorchStandingSpot();
    Point getKitchenStandingSpot();
    IBedFurnitureWrapper GetSpouseBed();
    Point getSpouseBedSpot(string spouseName);
    Point GetSpouseRoomSpot();
    IBedFurnitureWrapper GetBed(BedFurniture.BedType bed_type = BedFurniture.BedType.Any, int index = 0);
    Point GetPlayerBedSpot();
    IBedFurnitureWrapper GetPlayerBed();
    Point getBedSpot(BedFurniture.BedType bed_type = BedFurniture.BedType.Any);
    Point getEntryLocation();
    IBedFurnitureWrapper GetChildBed(int index);
    Point GetChildBedSpot(int index);
    bool isTilePlaceable(Vector2 v, IItemWrapper item = null);
    Point getRandomOpenPointInHouse(Random r, int buffer = 0, int tries = 30);
    bool performAction(string action, IFarmerWrapper who, Location tileLocation);
    bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who);
    bool hasActiveFireplace();
    void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false);
    void UpdateWhenCurrentLocation(GameTime time);
    Point getFireplacePoint();
    bool shouldShowSpouseRoom();
    void showSpouseRoom();
    void AddCellarTiles();
    string GetCellarName();
    void UpdateForRenovation();
    void updateFarmLayout();
    void MakeMapModifications(bool force = false);
    void PlaceInNearbySpace(Vector2 tileLocation, IObjectWrapper o);
    void RefreshFloorObjectNeighbors();
    void moveObjectsForHouseUpgrade(int whichUpgrade);
    void drawAboveFrontLayer(SpriteBatch b);
    void updateMap();
    void setMapForUpgradeLevel(int level);
    void createCellarWarps();
    void updateCellarWarps();
    void loadSpouseRoom();
    Microsoft.Xna.Framework.Rectangle? GetCribBounds();
    Microsoft.Xna.Framework.Rectangle? GetBedBounds(int child_index = 0);
    Microsoft.Xna.Framework.Rectangle? GetChildBedBounds(int child_index = 0);
    void UpdateChildRoom();
    void playerDivorced();
    IEnumerable<Microsoft.Xna.Framework.Rectangle> getForbiddenPetWarpTiles();
    bool canPetWarpHere(Vector2 tile_position);
    IEnumerable<Microsoft.Xna.Framework.Rectangle> getWalls();
    void TransferDataFromSavedLocation(IGameLocationWrapper l);
    IEnumerable<Microsoft.Xna.Framework.Rectangle> getFloors();
    bool CanModifyCrib();
  }
}