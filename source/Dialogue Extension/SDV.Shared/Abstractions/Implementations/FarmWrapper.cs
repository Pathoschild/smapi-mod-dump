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
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SDV.Shared.Abstractions
{
  public class FarmWrapper : IFarmWrapper
  {
    public FarmWrapper(Farm item) => GetBaseType = item;
    public Farm GetBaseType { get; }
    public NetLongDictionary<IFarmAnimalWrapper, NetRef<IFarmAnimalWrapper>> Animals { get; }
    public void AddModularShippingBin()
    {
    }

    public Rectangle GetHouseRect() => default;

    public Vector2 GetStarterShippingBinLocation() => default;

    public Vector2 GetGreenhouseStartLocation() => default;

    public void ClearGreenhouseGrassTiles()
    {
    }

    public Point GetPetStartLocation() => default;

    public void DayUpdate(int dayOfMonth)
    {
    }

    public void doDailyMountainFarmUpdate()
    {
    }

    public bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

    public float getExtraTrashChanceForCrabPot(int x, int y) => 0;

    public void addCrows()
    {
    }

    public void performTenMinuteUpdate(int timeOfDay)
    {
    }

    public void spawnGroundMonsterOffScreen()
    {
    }

    public void spawnFlyingMonstersOffScreen()
    {
    }

    public bool performToolAction(IToolWrapper t, int tileX, int tileY) => false;

    public void timeUpdate(int timeElapsed)
    {
    }

    public bool placeAnimal(IBluePrintWrapper blueprint, Vector2 tileLocation, bool serverCommand, long ownerID) => false;

    public int tryToAddHay(int num) => 0;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider,
      ICharacterWrapper character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false) =>
      false;

    public bool CheckPetAnimal(Vector2 position, IFarmerWrapper who) => false;

    public bool CheckPetAnimal(Rectangle rect, IFarmerWrapper who) => false;

    public bool CheckInspectAnimal(Vector2 position, IFarmerWrapper who) => false;

    public bool CheckInspectAnimal(Rectangle rect, IFarmerWrapper who) => false;

    public void requestGrandpaReevaluation()
    {
    }

    public bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who) => false;

    public void grandpaStatueCallback(IItemWrapper item, IFarmerWrapper who)
    {
    }

    public void TransferDataFromSavedLocation(IGameLocationWrapper l)
    {
    }

    public NetCollection<IItemWrapper> getShippingBin(IFarmerWrapper who) => null;

    public void shipItem(IItemWrapper i, IFarmerWrapper who)
    {
    }

    public bool leftClick(int x, int y, IFarmerWrapper who) => false;

    public void showShipment(IObjectWrapper o, bool playThrowSound = true)
    {
    }

    public int getFishingLocation(Vector2 tile) => 0;

    public bool doesTileSinkDebris(int tileX, int tileY, Debris.DebrisType type) => false;

    public bool CanRefillWateringCanOnTile(int tileX, int tileY) => false;

    public bool isTileBuildingFishable(int tileX, int tileY) => false;

    public IObjectWrapper getFish(float millisecondsAfterNibble, int bait, int waterDepth, IFarmerWrapper who, double baitPotency,
      Vector2 bobberTile, string location = null) =>
      null;

    public IEnumerable<IFarmAnimalWrapper> getAllFarmAnimals()
    {
      yield break;
    }

    public bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "", bool ignoreAllCharacters = false) => false;

    public void MakeMapModifications(bool force = false)
    {
    }

    public Vector2 GetSpouseOutdoorAreaCorner() => default;

    public int GetSpouseOutdoorAreaSpritesheetIndex() => 0;

    public void addSpouseOutdoorArea(string spouseName)
    {
    }

    public void addGrandpaCandles()
    {
    }

    public void pokeTileForConstruction(Vector2 tile)
    {
    }

    public bool isTileOccupiedForPlacement(Vector2 tileLocation, IObjectWrapper toPlace = null) => false;

    public bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p) => false;

    public void draw(SpriteBatch b)
    {
    }

    public Point GetMainMailboxPosition() => default;

    public Point GetGrandpaShrinePosition() => default;

    public Point GetMainFarmHouseEntry() => default;

    public void startEvent(IEventWrapper evt)
    {
    }

    public void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
    }

    public void ApplyHousePaint()
    {
    }

    public void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
    }

    public bool isTileOpenBesidesTerrainFeatures(Vector2 tile) => false;

    public bool CanBeRemotedlyViewed() => false;

    public void UpdateWhenCurrentLocation(GameTime time)
    {
    }

    public int getTotalCrops() => 0;

    public int getTotalCropsReadyForHarvest() => 0;

    public int getTotalUnwateredCrops() => 0;

    public int getTotalGreenhouseCropsReadyForHarvest() => 0;

    public int getTotalOpenHoeDirt() => 0;

    public int getTotalForageItems() => 0;

    public int getNumberOfMachinesReadyForHarvest() => 0;

    public bool doesFarmCaveNeedHarvesting() => false;
  }
}
