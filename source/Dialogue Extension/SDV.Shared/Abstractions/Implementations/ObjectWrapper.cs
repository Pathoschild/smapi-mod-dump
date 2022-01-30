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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using Rectangle = xTile.Dimensions.Rectangle;

namespace SDV.Shared.Abstractions
{
  public class ObjectWrapper : IObjectWrapper
  {
    public ObjectWrapper(Object item) => GetBaseType = item;
    public Object GetBaseType { get; }
    public Vector2 TileLocation { get; set; }
    public string Type { get; set; }
    public bool CanBeSetDown { get; set; }
    public bool CanBeGrabbed { get; set; }
    public bool IsHoeDirt { get; }
    public bool IsSpawnedObject { get; set; }
    public bool IsOn { get; set; }
    public int Fragility { get; set; }
    public int Price { get; set; }
    public int Edibility { get; set; }
    public int Stack { get; set; }
    public int Quality { get; set; }
    public bool Flipped { get; set; }
    public bool HasBeenPickedUpByFarmer { get; set; }
    public bool IsRecipe { get; set; }
    public int MinutesUntilReady { get; set; }
    public Vector2 Scale { get; set; }
    public string DisplayName { get; set; }
    public bool destroyOvernight { get; set; }
    public ILightSourceWrapper lightSource { get; set; }
    public string name { get; set; }
    public string Name { get; set; }
    public int SpecialVariable { get; set; }
    public int Category { get; set; }
    public bool HasBeenInInventory { get; set; }
    public int ParentSheetIndex { get; set; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
    public NetFields NetFields { get; }
    public Vector2 getLocalPosition(Rectangle viewport) => default;

    public bool performToolAction(IToolWrapper t, IGameLocationWrapper location) => false;

    public bool isAnimalProduct() => false;

    public bool onExplosion(IFarmerWrapper who, IGameLocationWrapper location) => false;

    public bool canBeShipped() => false;

    public void ApplySprinkler(IGameLocationWrapper location, Vector2 tile)
    {
    }

    public void ApplySprinklerAnimation(IGameLocationWrapper location)
    {
    }

    public IEnumerable<Vector2> GetSprinklerTiles()
    {
      yield break;
    }

    public bool IsInSprinklerRangeBroadphase(Vector2 target) => false;

    public void DayUpdate(IGameLocationWrapper location)
    {
    }

    public void rot()
    {
    }

    public void actionWhenBeingHeld(IFarmerWrapper who)
    {
    }

    public void actionWhenStopBeingHeld(IFarmerWrapper who)
    {
    }

    public void ConsumeInventoryItem(IFarmerWrapper who, int parent_sheet_index, int amount)
    {
    }

    public void ConsumeInventoryItem(IFarmerWrapper who, IItemWrapper drop_in, int amount)
    {
    }

    public int GetTallyOfObject(IFarmerWrapper who, int index, bool big_craftable) => 0;

    public IObjectWrapper GetDeconstructorOutput(IItemWrapper item) => null;

    public bool performObjectDropInAction(IItemWrapper dropInItem, bool probe, IFarmerWrapper who) => false;

    public void updateWhenCurrentLocation(GameTime time, IGameLocationWrapper environment)
    {
    }

    public void actionOnPlayerEntry()
    {
    }

    public bool canBeTrashed() => false;

    public bool isForage(IGameLocationWrapper location) => false;

    public void initializeLightSource(Vector2 tileLocation, bool mineShaft = false)
    {
    }

    public void performRemoveAction(Vector2 tileLocation, IGameLocationWrapper environment)
    {
    }

    public void dropItem(IGameLocationWrapper location, Vector2 origin, Vector2 destination)
    {
    }

    public bool isPassable() => false;

    public void reloadSprite()
    {
    }

    public void consumeRecipe(IFarmerWrapper who)
    {
    }

    public Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation) => default;

    public bool canBeGivenAsGift() => false;

    public bool performDropDownAction(IFarmerWrapper who) => false;

    public void MonsterMusk(IFarmerWrapper who)
    {
    }

    public string[] ModifyItemBuffs(string[] buffs)
    {
      return new string[] { };
    }

    public bool performUseAction(IGameLocationWrapper location) => false;

    public Color getCategoryColor() => default;

    public string getCategoryName() => null;

    public bool isActionable(IFarmerWrapper who) => false;

    public int getHealth() => 0;

    public void setHealth(int health)
    {
    }

    public int healthRecoveredOnConsumption() => 0;

    public int staminaRecoveredOnConsumption() => 0;

    public bool checkForAction(IFarmerWrapper who, bool justCheckingForActivity = false) => false;

    public void AttemptAutoLoad(IFarmerWrapper who)
    {
    }

    public void farmerAdjacentAction(IGameLocationWrapper location)
    {
    }

    public void addWorkingAnimation(IGameLocationWrapper environment)
    {
    }

    public void onReadyForHarvest(IGameLocationWrapper environment)
    {
    }

    public bool minutesElapsed(int minutes, IGameLocationWrapper environment) => false;

    public string checkForSpecialItemHoldUpMeessage() => null;

    public bool countsForShippedCollection() => false;

    public Vector2 getScale() => default;

    public void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, IFarmerWrapper f)
    {
    }

    public void drawPlacementBounds(SpriteBatch spriteBatch, IGameLocationWrapper location)
    {
    }

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
      StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
    }

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
      StackDrawType drawStackNumber)
    {
    }

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth)
    {
    }

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize)
    {
    }

    public void drawAsProp(SpriteBatch b)
    {
    }

    public void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
    {
    }

    public void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
    {
    }

    public int maximumStackSize() => 0;

    public int addToStack(IItemWrapper otherStack) => 0;

    public void hoverAction()
    {
    }

    public bool clicked(IFarmerWrapper who) => false;

    public IItemWrapper getOne() => null;

    public void _GetOneFrom(IItemWrapper source)
    {
    }

    public bool canBePlacedHere(IGameLocationWrapper l, Vector2 tile) => false;

    public bool isPlaceable() => false;

    public bool IsConsideredReadyMachineForComputer() => false;

    public bool isSapling() => false;

    public bool IsSprinkler() => false;

    public int GetModifiedRadiusForSprinkler() => 0;

    public int GetBaseRadiusForSprinkler() => 0;

    public bool placementAction(IGameLocationWrapper location, int x, int y, IFarmerWrapper who = null) => false;

    public bool actionWhenPurchased() => false;

    public bool canBePlacedInWater() => false;

    public bool needsToBeDonated() => false;

    public string getDescription() => null;

    public int sellToStorePrice(long specificPlayerID = -1) => 0;

    public int salePrice() => 0;

    public bool IsInfiniteStock() => false;

    public void MarkContextTagsDirty()
    {
    }

    public IEnumerable<string> GetContextTagList()
    {
      yield break;
    }

    public IEnumerable<string> GetContextTags()
    {
      yield break;
    }

    public bool HasContextTag(string tag) => false;

    public string SanitizeContextTag(string tag) => null;

    public bool ShouldSerializeparentSheetIndex() => false;

    public void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha,
      StringBuilder overrideText)
    {
    }

    public Point getExtraSpaceNeededForTooltipSpecialIcons(SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight,
      StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom) =>
      default;

    public bool ShouldDrawIcon() => false;

    public bool CanBuyItem(IFarmerWrapper who) => false;

    public bool canBeDropped() => false;

    public int getRemainingStackSpace() => 0;

    public string getHoverBoxText(IItemWrapper hoveredItem) => null;

    public void drawAttachments(SpriteBatch b, int x, int y)
    {
    }

    public int attachmentSlots() => 0;

    public bool canStackWith(ISalableWrapper other) => false;

    public ISalableWrapper GetSalableInstance() => null;

    public int CompareTo(object obj) => 0;

    public int getCategorySortValue() => 0;

    public void resetState()
    {
    }
  }
}
