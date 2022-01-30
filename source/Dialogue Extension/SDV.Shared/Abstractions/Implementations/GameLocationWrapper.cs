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
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using xTile;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SDV.Shared.Abstractions
{
  public class GameLocationWrapper : IGameLocationWrapper
  {
    public GameLocationWrapper(GameLocation gameLocation) => GetBaseType = gameLocation;

    public Map Map { get; set; }
    public OverlaidDictionary Objects { get; }
    public IEnumerable<ITemporaryAnimatedSpriteWrapper> TemporarySprites { get; }
    public string Name { get; }
    public float LightLevel { get; set; }
    public bool IsFarm { get; set; }
    public bool IsOutdoors { get; set; }
    public bool IsGreenhouse { get; set; }
    public NetFields NetFields { get; }
    public NetRoot<IGameLocationWrapper> Root { get; }
    public string NameOrUniqueName { get; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
    public bool SeedsIgnoreSeasonsHere() => false;

    public bool CanPlantSeedsHere(int crop_index, int tile_x, int tile_y) => false;

    public bool CanPlantTreesHere(int sapling_index, int tile_x, int tile_y) => false;

    public void InvalidateCachedMultiplayerMap(IDictionary<string, ICachedMultiplayerMapWrapper> cached_data)
    {
    }

    public void MakeMapModifications(bool force = false)
    {
    }

    public bool ApplyCachedMultiplayerMap(IDictionary<string, ICachedMultiplayerMapWrapper> cached_data,
      string requested_map_path) => false;

    public void StoreCachedMultiplayerMap(IDictionary<string, ICachedMultiplayerMapWrapper> cached_data)
    {
    }

    public void TransferDataFromSavedLocation(IGameLocationWrapper l)
    {
    }

    public void OnTerrainFeatureAdded(ITerrainFeaturesWrapper feature, Vector2 location)
    {
    }

    public void OnTerrainFeatureRemoved(ITerrainFeaturesWrapper feature)
    {
    }

    public void UpdateTerrainFeatureUpdateSubscription(ITerrainFeaturesWrapper feature)
    {
    }

    public string GetSeasonForLocation() => null;

    public bool isTemp() => false;

    public void playSound(string audioName, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
    }

    public void playSoundPitched(string audioName, int pitch,
      NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
    }

    public void playSoundAt(string audioName, Vector2 position,
      NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default)
    {
    }

    public void localSound(string audioName)
    {
    }

    public void localSoundAt(string audioName, Vector2 position)
    {
    }

    public void ApplyMapOverride(Map override_map, string override_key, Rectangle? source_rect = null,
      Rectangle? dest_rect = null)
    {
    }

    public void ApplyMapOverride(string map_name, Rectangle? source_rect = null, Rectangle? destination_rect = null)
    {
    }

    public void ApplyMapOverride(string map_name, string override_key_name, Rectangle? source_rect = null,
      Rectangle? destination_rect = null)
    {
    }

    public bool RunLocationSpecificEventCommand(IEventWrapper current_event, string command_string, bool first_run,
      params string[] args) => false;

    public void UpdateMapSeats()
    {
    }

    public void loadMap(string mapPath, bool force_reload = false)
    {
    }

    public void reloadMap()
    {
    }

    public bool canSlimeMateHere() => false;

    public bool canSlimeHatchHere() => false;

    public void addCharacter(INPCWrapper character)
    {
    }

    public IEnumerable<INPCWrapper> getCharacters() => null;

    public IWarpWrapper isCollidingWithWarp(Rectangle position, ICharacterWrapper character) => null;

    public IWarpWrapper isCollidingWithWarpOrDoor(Rectangle position, ICharacterWrapper character = null) => null;

    public IWarpWrapper isCollidingWithDoors(Rectangle position, ICharacterWrapper character = null) => null;

    public IWarpWrapper getWarpFromDoor(Point door, ICharacterWrapper character = null) => null;

    public void addResourceClumpAndRemoveUnderlyingTerrain(int resourceClumpIndex, int width, int height, Vector2 tile)
    {
    }

    public bool canFishHere() => false;

    public bool CanRefillWateringCanOnTile(int tileX, int tileY) => false;

    public bool isTileBuildingFishable(int tileX, int tileY) => false;

    public bool isTileFishable(int tileX, int tileY) => false;

    public bool isFarmerCollidingWithAnyCharacter() => false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer) => false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, ICharacterWrapper character) =>
      false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer,
      int damagesFarmer, bool glider) => false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer,
      int damagesFarmer, bool glider,
      ICharacterWrapper character) =>
      false;

    public bool isCollidingPosition(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer,
      int damagesFarmer, bool glider,
      ICharacterWrapper character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false) =>
      false;

    public IFurnitureWrapper GetFurnitureAt(Vector2 tile_position) => null;

    public bool isTilePassable(Location tileLocation, xTile.Dimensions.Rectangle viewport) => false;

    public bool isTilePassable(Rectangle nextPosition, xTile.Dimensions.Rectangle viewport) => false;

    public bool isPointPassable(Location location, xTile.Dimensions.Rectangle viewport) => false;

    public bool isTileOnMap(Vector2 position) => false;

    public bool isTileOnMap(int x, int y) => false;

    public void busLeave()
    {
    }

    public int numberOfObjectsWithName(string name) => 0;

    public Point getWarpPointTo(string location, ICharacterWrapper character = null) => default;

    public Point getWarpPointTarget(Point warpPointLocation, ICharacterWrapper character = null) => default;

    public bool HasLocationOverrideDialogue(INPCWrapper character) => false;

    public string GetLocationOverrideDialogue(INPCWrapper character) => null;

    public void boardBus(Vector2 playerTileLocation)
    {
    }

    public INPCWrapper doesPositionCollideWithCharacter(float x, float y) => null;

    public INPCWrapper doesPositionCollideWithCharacter(Rectangle r, bool ignoreMonsters = false) => null;

    public void switchOutNightTiles()
    {
    }

    public void checkForMusic(GameTime time)
    {
    }

    public INPCWrapper isCollidingWithCharacter(Rectangle box) => null;

    public void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
    }

    public bool moveObject(int oldX, int oldY, int newX, int newY) => false;

    public void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
    {
    }

    public void updateMap()
    {
    }

    public ILargeTerrainFeatureWrapper getLargeTerrainFeatureAt(int tileX, int tileY) => null;

    public void UpdateWhenCurrentLocation(GameTime time)
    {
    }

    public void updateWater(GameTime time)
    {
    }

    public INPCWrapper getCharacterFromName(string name) => null;

    public void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
    }

    public IResponseWrapper[] createYesNoResponses()
    {
      return new IResponseWrapper[] { };
    }

    public void createQuestionDialogue(string question, IResponseWrapper[] answerChoices, string dialogKey)
    {
    }

    public void createQuestionDialogue(string question, IResponseWrapper[] answerChoices,
      afterQuestionBehavior afterDialogueBehavior,
      INPCWrapper speaker = null)
    {
    }

    public void createQuestionDialogue(string question, IResponseWrapper[] answerChoices, string dialogKey, IObjectWrapper actionObject)
    {
    }

    public void createQuestionDialogueWithCustomWidth(string question, IResponseWrapper[] answerChoices, string dialogKey)
    {
    }

    public Point GetMapPropertyPosition(string key, int default_x, int default_y) => default;

    public void monsterDrop(IMonsterWrapper monster, int x, int y, IFarmerWrapper who)
    {
    }

    public bool HasUnlockedAreaSecretNotes(IFarmerWrapper who) => false;

    public bool damageMonster(Rectangle areaOfEffect, int minDamage, int maxDamage, bool isBomb, IFarmerWrapper who) => false;

    public bool damageMonster(Rectangle areaOfEffect, int minDamage, int maxDamage, bool isBomb,
      float knockBackModifier,
      int addedPrecision, float critChance, float critMultiplier, bool triggerMonsterInvincibleTimer, IFarmerWrapper who) =>
      false;

    public bool BlocksDamageLOS(int x, int y) => false;

    public void moveCharacters(GameTime time)
    {
    }

    public void growWeedGrass(int iterations)
    {
    }

    public void removeDamageDebris(IMonsterWrapper monster)
    {
    }

    public void spawnWeeds(bool weedsOnly)
    {
    }

    public bool addCharacterAtRandomLocation(INPCWrapper n) => false;

    public void OnMiniJukeboxAdded()
    {
    }

    public void OnMiniJukeboxRemoved()
    {
    }

    public void UpdateMiniJukebox()
    {
    }

    public bool IsMiniJukeboxPlaying() => false;

    public void DayUpdate(int dayOfMonth)
    {
    }

    public void addLightGlows()
    {
    }

    public INPCWrapper isCharacterAtTile(Vector2 tileLocation) => null;

    public void ResetCharacterDialogues()
    {
    }

    public string getMapProperty(string propertyName) => null;

    public void tryToAddCritters(bool onlyIfOnScreen = false)
    {
    }

    public void addClouds(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void addOwl()
    {
    }

    public void setFireplace(bool on, int tileLocationX, int tileLocationY, bool playSound = true)
    {
    }

    public void addWoodpecker(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void addSquirrels(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void addBunnies(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void instantiateCrittersList()
    {
    }

    public void addCritter(ICritterWrapper c)
    {
    }

    public void addButterflies(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void addBirdies(double chance, bool onlyIfOnScreen = false)
    {
    }

    public void addJumperFrog(Vector2 tileLocation)
    {
    }

    public void addFrog()
    {
    }

    public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
    {
    }

    public bool isAreaClear(Rectangle area) => false;

    public void refurbishMapPortion(Rectangle areaToRefurbish, string refurbishedMapName, Point mapReaderStartPoint)
    {
    }

    public Vector2 getRandomTile() => default;

    public void setUpLocationSpecificFlair()
    {
    }

    public void hostSetup()
    {
    }

    public bool HasFarmerWatchingBroadcastEventReturningHere() => false;

    public void resetForPlayerEntry()
    {
    }

    public void SelectRandomMiniJukeboxTrack()
    {
    }

    public ILightSourceWrapper getLightSource(int identifier) => null;

    public bool hasLightSource(int identifier) => false;

    public void removeLightSource(int identifier)
    {
    }

    public void repositionLightSource(int identifier, Vector2 position)
    {
    }

    public bool isTileOccupiedForPlacement(Vector2 tileLocation, IObjectWrapper toPlace = null) => false;

    public IFarmerWrapper isTileOccupiedByFarmer(Vector2 tileLocation) => null;

    public bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "", bool ignoreAllCharacters = false) =>
      false;

    public bool isTileOccupiedIgnoreFloors(Vector2 tileLocation, string characterToIgnore = "") => false;

    public bool isTileHoeDirt(Vector2 tileLocation) => false;

    public void playTerrainSound(Vector2 tileLocation, ICharacterWrapper who = null, bool showTerrainDisturbAnimation = true)
    {
    }

    public bool checkTileIndexAction(int tileIndex) => false;

    public bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, IFarmerWrapper who) => false;

    public bool CanFreePlaceFurniture() => false;

    public bool LowPriorityLeftClick(int x, int y, IFarmerWrapper who) => false;

    public bool CanPlaceThisFurnitureHere(IFurnitureWrapper furniture) => false;

    public IEnumerable<Rectangle> getWalls() => null;

    public bool leftClick(int x, int y, IFarmerWrapper who) => false;

    public int getExtraMillisecondsPerInGameMinuteForThisLocation() => 0;

    public bool isTileLocationTotallyClearAndPlaceable(int x, int y) => false;

    public bool isTileLocationTotallyClearAndPlaceable(Vector2 v) => false;

    public bool isTileLocationTotallyClearAndPlaceableIgnoreFloors(Vector2 v) => false;

    public void ActivateKitchen(NetRef<IChestWrapper> fridge)
    {
    }

    public bool isTilePlaceable(Vector2 v, IItemWrapper item = null) => false;

    public bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p) => false;

    public void openDoor(Location tileLocation, bool playSound)
    {
    }

    public void doStarpoint(string which)
    {
    }

    public string FormatCompletionLine(Func<IFarmerWrapper, float> check) => null;

    public string FormatCompletionLine(Func<IFarmerWrapper, bool> check, string true_value, string false_value) => null;

    public void ShowQiCat()
    {
    }

    public bool performAction(string action, IFarmerWrapper who, Location tileLocation) => false;

    public void showPrairieKingMenu()
    {
    }

    public Vector2 findNearestObject(Vector2 startingPoint, int objectIndex, bool bigCraftable) => default;

    public void lockedDoorWarp(string[] actionParams)
    {
    }

    public void playElliottPiano(int key)
    {
    }

    public void readNote(int which)
    {
    }

    public void mailbox()
    {
    }

    public void farmerFile()
    {
    }

    public void openItemChest(Location location, int whichObject)
    {
    }

    public void getWallDecorItem(Location location)
    {
    }

    public bool openShopMenu(string which) => false;

    public bool isObjectAt(int x, int y) => false;

    public bool isObjectAtTile(int tileX, int tileY) => false;
    public IObjectWrapper getObjectAt(int x, int y) => null;

    public IObjectWrapper getObjectAtTile(int x, int y) => null;

    public bool saloon(Location tileLocation) => false;

    public bool carpenters(Location tileLocation) => false;

    public bool blacksmith(Location tileLocation) => false;

    public bool animalShop(Location tileLocation) => false;

    public void openChest(Location location, int debrisType, int numberOfChunks)
    {
    }

    public void openChest(Location location, int[] debrisType, int numberOfChunks)
    {
    }

    public string actionParamsToString(string[] actionparams) => null;

    public void removeTile(Location tileLocation, string layer)
    {
    }

    public void removeTile(int x, int y, string layer)
    {
    }

    public void characterTrampleTile(Vector2 tile)
    {
    }

    public bool characterDestroyObjectWithinRectangle(Rectangle rect, bool showDestroyedObject) => false;

    public IObjectWrapper removeObject(Vector2 location, bool showDestroyedObject) => null;

    public void removeTileProperty(int tileX, int tileY, string layer, string key)
    {
    }

    public void setTileProperty(int tileX, int tileY, string layer, string key, string value)
    {
    }

    public void removeBatch(IEnumerable<Vector2> locations)
    {
    }

    public void setObjectAt(float x, float y, IObjectWrapper o)
    {
    }

    public void cleanupBeforeSave()
    {
    }

    public void cleanupForVacancy()
    {
    }

    public void cleanupBeforePlayerExit()
    {
    }

    public bool answerDialogueAction(string questionAndAnswer, string[] questionParams) => false;

    public bool answerDialogue(IResponseWrapper answer) => false;

    public void setObject(Vector2 v, IObjectWrapper o)
    {
    }

    public bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

    public float getExtraTrashChanceForCrabPot(int x, int y) => 0;

    public void tryToBuyNewBackpack()
    {
    }

    public void checkForMapChanges()
    {
    }

    public void removeStumpOrBoulder(int tileX, int tileY, IObjectWrapper o)
    {
    }

    public void destroyObject(Vector2 tileLocation, IFarmerWrapper who)
    {
    }

    public void destroyObject(Vector2 tileLocation, bool hardDestroy, IFarmerWrapper who)
    {
    }

    public GameLocation.LocationContext GetLocationContext() => GameLocation.LocationContext.Default;

    public bool sinkDebris(IDebrisWrapper debris, Vector2 chunkTile, Vector2 chunkPosition) => false;

    public bool doesTileSinkDebris(int xTile, int yTile, Debris.DebrisType type) => false;

    public bool doesEitherTileOrTileIndexPropertyEqual(int xTile, int yTile, string propertyName, string layerName,
      string propertyValue) =>
      false;

    public string doesTileHaveProperty(int xTile, int yTile, string propertyName, string layerName) => null;

    public string doesTileHavePropertyNoNull(int xTile, int yTile, string propertyName, string layerName) => null;

    public bool isWaterTile(int xTile, int yTile) => false;

    public bool isOpenWater(int xTile, int yTile) => false;

    public bool isCropAtTile(int tileX, int tileY) => false;

    public bool dropObject(IObjectWrapper obj, Vector2 dropLocation, xTile.Dimensions.Rectangle viewport, bool initialPlacement,
      IFarmerWrapper who = null) => false;

    public bool dropObject(IObjectWrapper obj) => false;

    public void explode(Vector2 tileLocation, int radius, IFarmerWrapper who, bool damageFarmers = true, int damage_amount = -1)
    {
    }

    public void explosionAt(float x, float y)
    {
    }

    public void removeTemporarySpritesWithID(int id)
    {
    }

    public void removeTemporarySpritesWithID(float id)
    {
    }

    public void removeTemporarySpritesWithIDLocal(float id)
    {
    }

    public void makeHoeDirt(Vector2 tileLocation, bool ignoreChecks = false)
    {
    }

    public int numberOfObjectsOfType(int index, bool bigCraftable) => 0;

    public void passTimeForObjects(int timeElapsed)
    {
    }

    public void performTenMinuteUpdate(int timeOfDay)
    {
    }

    public void performOrePanTenMinuteUpdate(Random r)
    {
    }

    public int getFishingLocation(Vector2 tile) => 0;

    public bool IsUsingMagicBait(IFarmerWrapper who) => false;

    public IObjectWrapper getFish(float millisecondsAfterNibble, int bait, int waterDepth, IFarmerWrapper who,
      double baitPotency,
      Vector2 bobberTile, string locationName) => null;

    public bool isActionableTile(int xTile, int yTile, IFarmerWrapper who) => false;

    public void digUpArtifactSpot(int xLocation, int yLocation, IFarmerWrapper who)
    {
    }

    public string checkForBuriedItem(int xLocation, int yLocation, bool explosion, bool detectOnly, IFarmerWrapper who) => null;

    public void setAnimatedMapTile(int tileX, int tileY, int[] animationTileIndexes, long interval, string layer,
      string action,
      int whichTileSheet = 0)
    {
    }

    public bool AllowMapModificationsInResetState() => false;

    public void setMapTile(int tileX, int tileY, int index, string layer, string action, int whichTileSheet = 0)
    {
    }

    public void setMapTileIndex(int tileX, int tileY, int index, string layer, int whichTileSheet = 0)
    {
    }

    public void shiftObjects(int dx, int dy)
    {
    }

    public int getTileIndexAt(Point p, string layer) => 0;

    public int getTileIndexAt(int x, int y, string layer) => 0;

    public string getTileSheetIDAt(int x, int y, string layer) => null;

    public void OnStoneDestroyed(int indexOfStone, int x, int y, IFarmerWrapper who)
    {
    }

    public bool isBehindBush(Vector2 Tile) => false;

    public bool isBehindTree(Vector2 Tile) => false;

    public void spawnObjects()
    {
    }

    public bool isTileLocationOpen(Location location) => false;

    public bool isTileLocationOpenIgnoreFrontLayers(Location location) => false;

    public void spawnWeedsAndStones(int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
    {
    }

    public void removeEverythingExceptCharactersFromThisTile(int x, int y)
    {
    }

    public string getFootstepSoundReplacement(string footstep) => null;

    public void removeEverythingFromThisTile(int x, int y)
    {
    }

    public IDictionary<string, string> GetLocationEvents() => null;

    public void checkForEvents()
    {
    }

    public IEventWrapper findEventById(int id, IFarmerWrapper farmerActor = null) => null;

    public void startEvent(IEventWrapper evt)
    {
    }

    public void drawBackground(SpriteBatch b)
    {
    }

    public void drawWater(SpriteBatch b)
    {
    }

    public void drawWaterTile(SpriteBatch b, int x, int y)
    {
    }

    public void drawWaterTile(SpriteBatch b, int x, int y, Color color)
    {
    }

    public void drawFloorDecorations(SpriteBatch b)
    {
    }

    public ITemporaryAnimatedSpriteWrapper getTemporarySpriteByID(int id) => null;

    public bool shouldHideCharacters() => false;

    public void DrawFarmerUsernames(SpriteBatch b)
    {
    }

    public void draw(SpriteBatch b)
    {
    }

    public void drawAboveFrontLayer(SpriteBatch b)
    {
    }

    public void drawLightGlows(SpriteBatch b)
    {
    }

    public IObjectWrapper tryToCreateUnseenSecretNote(IFarmerWrapper who) => null;

    public bool performToolAction(IToolWrapper t, int tileX, int tileY) => false;

    public void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public void updateSeasonalTileSheets(Map map = null)
    {
    }

    public int checkEventPrecondition(string precondition) => 0;

    public void updateWarps()
    {
    }

    public void loadWeeds()
    {
    }

    public bool CanLoadPathObjectHere(Vector2 tile) => false;

    public void loadObjects()
    {
    }

    public void updateDoors()
    {
    }

    public bool isTerrainFeatureAt(int x, int y) => false;

    public void loadLights()
    {
    }

    public bool isFarmBuildingInterior() => false;

    public bool CanBeRemotedlyViewed() => false;

    public bool Equals(IGameLocationWrapper other) => false;

    public GameLocation GetBaseType { get; }

    public delegate void afterQuestionBehavior(Farmer who, string whichAnswer);
  }
}