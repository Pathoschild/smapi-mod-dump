/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Network;
using xTile.Dimensions;

namespace SDV.Shared.Abstractions
{
  public interface IFarmWrapper : IWrappedType<Farm>
  {
    NetLongDictionary<IFarmAnimalWrapper, NetRef<IFarmAnimalWrapper>> Animals { get; }
    void AddModularShippingBin();
    Microsoft.Xna.Framework.Rectangle GetHouseRect();
    Vector2 GetStarterShippingBinLocation();
    Vector2 GetGreenhouseStartLocation();
    void ClearGreenhouseGrassTiles();
    Point GetPetStartLocation();
    void DayUpdate(int dayOfMonth);
    void doDailyMountainFarmUpdate();
    bool catchOceanCrabPotFishFromThisSpot(int x, int y);
    float getExtraTrashChanceForCrabPot(int x, int y);
    void addCrows();
    void performTenMinuteUpdate(int timeOfDay);
    void spawnGroundMonsterOffScreen();
    void spawnFlyingMonstersOffScreen();
    bool performToolAction(IToolWrapper t, int tileX, int tileY);
    void timeUpdate(int timeElapsed);

    bool placeAnimal(
      IBluePrintWrapper blueprint,
      Vector2 tileLocation,
      bool serverCommand,
      long ownerID);

    int tryToAddHay(int num);

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

    bool CheckPetAnimal(Vector2 position, IFarmerWrapper who);
    bool CheckPetAnimal(Microsoft.Xna.Framework.Rectangle rect, IFarmerWrapper who);
    bool CheckInspectAnimal(Vector2 position, IFarmerWrapper who);
    bool CheckInspectAnimal(Microsoft.Xna.Framework.Rectangle rect, IFarmerWrapper who);
    void requestGrandpaReevaluation();
    bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who);
    void grandpaStatueCallback(IItemWrapper item, IFarmerWrapper who);
    void TransferDataFromSavedLocation(IGameLocationWrapper l);
    NetCollection<IItemWrapper> getShippingBin(IFarmerWrapper who);
    void shipItem(IItemWrapper i, IFarmerWrapper who);
    bool leftClick(int x, int y, IFarmerWrapper who);
    void showShipment(IObjectWrapper o, bool playThrowSound = true);
    int getFishingLocation(Vector2 tile);
    bool doesTileSinkDebris(int tileX, int tileY, Debris.DebrisType type);
    bool CanRefillWateringCanOnTile(int tileX, int tileY);
    bool isTileBuildingFishable(int tileX, int tileY);

    IObjectWrapper getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      IFarmerWrapper who,
      double baitPotency,
      Vector2 bobberTile,
      string location = null);

    IEnumerable<IFarmAnimalWrapper> getAllFarmAnimals();

    bool isTileOccupied(
      Vector2 tileLocation,
      string characterToIgnore = "",
      bool ignoreAllCharacters = false);

    void MakeMapModifications(bool force = false);
    Vector2 GetSpouseOutdoorAreaCorner();
    int GetSpouseOutdoorAreaSpritesheetIndex();
    void addSpouseOutdoorArea(string spouseName);
    void addGrandpaCandles();
    void pokeTileForConstruction(Vector2 tile);
    bool isTileOccupiedForPlacement(Vector2 tileLocation, IObjectWrapper toPlace = null);
    bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p);
    void draw(SpriteBatch b);
    Point GetMainMailboxPosition();
    Point GetGrandpaShrinePosition();
    Point GetMainFarmHouseEntry();
    void startEvent(IEventWrapper evt);
    void drawAboveAlwaysFrontLayer(SpriteBatch b);
    void ApplyHousePaint();
    void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false);
    bool isTileOpenBesidesTerrainFeatures(Vector2 tile);
    bool CanBeRemotedlyViewed();
    void UpdateWhenCurrentLocation(GameTime time);
    int getTotalCrops();
    int getTotalCropsReadyForHarvest();
    int getTotalUnwateredCrops();
    int getTotalGreenhouseCropsReadyForHarvest();
    int getTotalOpenHoeDirt();
    int getTotalForageItems();
    int getNumberOfMachinesReadyForHarvest();
    bool doesFarmCaveNeedHarvesting();
  }
}