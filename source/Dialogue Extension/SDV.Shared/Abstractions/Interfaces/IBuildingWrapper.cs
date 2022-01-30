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
using StardewValley.Buildings;

namespace SDV.Shared.Abstractions
{
  public interface IBuildingWrapper : IWrappedType<Building>
  {
    IModDataDictionaryWrapper modDataForSerialization { get; set; }
    bool isCabin { get; }
    string nameOfIndoors { get; }
    string nameOfIndoorsWithoutUnique { get; }
    bool isMoving { get; set; }
    NetFields NetFields { get; }
    bool CanBePainted();
    bool hasCarpenterPermissions();
    string textureName();
    void resetTexture();
    int getTileSheetIndexForStructurePlacementTile(int x, int y);
    void performTenMinuteAction(int timeElapsed);
    void resetLocalState();
    bool CanLeftClick(int x, int y);
    bool leftClicked();
    bool doAction(Vector2 tileLocation, IFarmerWrapper who);
    bool isActionableTile(int xTile, int yTile, IFarmerWrapper who);
    void performActionOnBuildingPlacement();
    void performActionOnConstruction(IGameLocationWrapper location);
    void performActionOnDemolition(IGameLocationWrapper location);
    IEnumerable<IItemWrapper> GetAdditionalItemsToCheckBeforeDemolish();
    void BeforeDemolish();
    void performActionOnUpgrade(IGameLocationWrapper location);

    string isThereAnythingtoPreventConstruction(
      IGameLocationWrapper location,
      Vector2 tile_location);

    bool performActiveObjectDropInAction(IFarmerWrapper who, bool probe);
    void performToolAction(IToolWrapper t, int tileX, int tileY);
    void updateWhenFarmNotCurrentLocation(GameTime time);
    void Update(GameTime time);
    void showUpgradeAnimation(IGameLocationWrapper location);
    Vector2 getUpgradeSignLocation();
    string getNameOfNextUpgrade();
    void showDestroyedAnimation(IGameLocationWrapper location);
    void dayUpdate(int dayOfMonth);
    void upgrade();
    Rectangle getSourceRect();
    Rectangle getSourceRectForMenu();
    void updateInteriorWarps(IGameLocationWrapper interior = null);
    Point getPointForHumanDoor();
    Rectangle getRectForHumanDoor();
    Rectangle getRectForAnimalDoor();
    void load();
    bool isUnderConstruction();
    bool occupiesTile(Vector2 tile);
    bool isTilePassable(Vector2 tile);
    bool isTileOccupiedForPlacement(Vector2 tile, IObjectWrapper to_place);
    bool isTileFishable(Vector2 tile);
    bool CanRefillWateringCan();
    bool intersects(Rectangle boundingBox);
    void drawInMenu(SpriteBatch b, int x, int y);
    void drawBackground(SpriteBatch b);
    void draw(SpriteBatch b);
    void drawShadow(SpriteBatch b, int localX = -1, int localY = -1);
    void OnStartMove();
    void OnEndMove();
    Point getPorchStandingSpot();

    bool doesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value);

    Point getMailboxPosition();
    int GetAdditionalTilePropertyRadius();
    void removeOverlappingBushes(IGameLocationWrapper location);
    void drawInConstruction(SpriteBatch b);
  }
}