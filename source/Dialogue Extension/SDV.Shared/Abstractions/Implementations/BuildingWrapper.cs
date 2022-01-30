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
  public class BuildingWrapper : IBuildingWrapper
  {
    public BuildingWrapper(Building item) => GetBaseType = item;
    public Building GetBaseType { get; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
    public bool isCabin { get; }
    public string nameOfIndoors { get; }
    public string nameOfIndoorsWithoutUnique { get; }
    public bool isMoving { get; set; }
    public NetFields NetFields { get; }
    public bool CanBePainted() => false;

    public bool hasCarpenterPermissions() => false;

    public string textureName() => null;

    public void resetTexture()
    {
    }

    public int getTileSheetIndexForStructurePlacementTile(int x, int y) => 0;

    public void performTenMinuteAction(int timeElapsed)
    {
    }

    public void resetLocalState()
    {
    }

    public bool CanLeftClick(int x, int y) => false;

    public bool leftClicked() => false;

    public bool doAction(Vector2 tileLocation, IFarmerWrapper who) => false;

    public bool isActionableTile(int xTile, int yTile, IFarmerWrapper who) => false;

    public void performActionOnBuildingPlacement()
    {
    }

    public void performActionOnConstruction(IGameLocationWrapper location)
    {
    }

    public void performActionOnDemolition(IGameLocationWrapper location)
    {
    }

    public IEnumerable<IItemWrapper> GetAdditionalItemsToCheckBeforeDemolish()
    {
      yield break;
    }

    public void BeforeDemolish()
    {
    }

    public void performActionOnUpgrade(IGameLocationWrapper location)
    {
    }

    public string isThereAnythingtoPreventConstruction(IGameLocationWrapper location, Vector2 tile_location) => null;

    public bool performActiveObjectDropInAction(IFarmerWrapper who, bool probe) => false;

    public void performToolAction(IToolWrapper t, int tileX, int tileY)
    {
    }

    public void updateWhenFarmNotCurrentLocation(GameTime time)
    {
    }

    public void Update(GameTime time)
    {
    }

    public void showUpgradeAnimation(IGameLocationWrapper location)
    {
    }

    public Vector2 getUpgradeSignLocation() => default;

    public string getNameOfNextUpgrade() => null;

    public void showDestroyedAnimation(IGameLocationWrapper location)
    {
    }

    public void dayUpdate(int dayOfMonth)
    {
    }

    public void upgrade()
    {
    }

    public Rectangle getSourceRect() => default;

    public Rectangle getSourceRectForMenu() => default;

    public void updateInteriorWarps(IGameLocationWrapper interior = null)
    {
    }

    public Point getPointForHumanDoor() => default;

    public Rectangle getRectForHumanDoor() => default;

    public Rectangle getRectForAnimalDoor() => default;

    public void load()
    {
    }

    public bool isUnderConstruction() => false;

    public bool occupiesTile(Vector2 tile) => false;

    public bool isTilePassable(Vector2 tile) => false;

    public bool isTileOccupiedForPlacement(Vector2 tile, IObjectWrapper to_place) => false;

    public bool isTileFishable(Vector2 tile) => false;

    public bool CanRefillWateringCan() => false;

    public bool intersects(Rectangle boundingBox) => false;

    public void drawInMenu(SpriteBatch b, int x, int y)
    {
    }

    public void drawBackground(SpriteBatch b)
    {
    }

    public void draw(SpriteBatch b)
    {
    }

    public void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
    {
    }

    public void OnStartMove()
    {
    }

    public void OnEndMove()
    {
    }

    public Point getPorchStandingSpot() => default;

    public bool doesTileHaveProperty(int tile_x, int tile_y, string property_name, string layer_name, ref string property_value) => false;

    public Point getMailboxPosition() => default;

    public int GetAdditionalTilePropertyRadius() => 0;

    public void removeOverlappingBushes(IGameLocationWrapper location)
    {
    }

    public void drawInConstruction(SpriteBatch b)
    {
    }
  }
}
