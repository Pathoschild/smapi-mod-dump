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

namespace SDV.Shared.Abstractions
{
  public interface IObjectWrapper : IWrappedType<Object>
  {
    Vector2 TileLocation { get; set; }
    string Type { get; set; }
    bool CanBeSetDown { get; set; }
    bool CanBeGrabbed { get; set; }
    bool IsHoeDirt { get; }
    bool IsSpawnedObject { get; set; }
    bool IsOn { get; set; }
    int Fragility { get; set; }
    int Price { get; set; }
    int Edibility { get; set; }
    int Stack { get; set; }
    int Quality { get; set; }
    bool Flipped { get; set; }
    bool HasBeenPickedUpByFarmer { get; set; }
    bool IsRecipe { get; set; }
    int MinutesUntilReady { get; set; }
    Vector2 Scale { get; set; }
    string DisplayName { get; set; }
    bool destroyOvernight { get; set; }
    ILightSourceWrapper lightSource { get; set; }
    string name { get; set; }
    string Name { get; set; }
    int SpecialVariable { get; set; }
    int Category { get; set; }
    bool HasBeenInInventory { get; set; }
    int ParentSheetIndex { get; set; }
    IModDataDictionaryWrapper modDataForSerialization { get; set; }
    NetFields NetFields { get; }
    Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport);
    bool performToolAction(IToolWrapper t, IGameLocationWrapper location);
    bool isAnimalProduct();
    bool onExplosion(IFarmerWrapper who, IGameLocationWrapper location);
    bool canBeShipped();
    void ApplySprinkler(IGameLocationWrapper location, Vector2 tile);
    void ApplySprinklerAnimation(IGameLocationWrapper location);
    IEnumerable<Vector2> GetSprinklerTiles();
    bool IsInSprinklerRangeBroadphase(Vector2 target);
    void DayUpdate(IGameLocationWrapper location);
    void rot();
    void actionWhenBeingHeld(IFarmerWrapper who);
    void actionWhenStopBeingHeld(IFarmerWrapper who);
    void ConsumeInventoryItem(IFarmerWrapper who, int parent_sheet_index, int amount);
    void ConsumeInventoryItem(IFarmerWrapper who, IItemWrapper drop_in, int amount);
    int GetTallyOfObject(IFarmerWrapper who, int index, bool big_craftable);
    IObjectWrapper GetDeconstructorOutput(IItemWrapper item);
    bool performObjectDropInAction(IItemWrapper dropInItem, bool probe, IFarmerWrapper who);
    void updateWhenCurrentLocation(GameTime time, IGameLocationWrapper environment);
    void actionOnPlayerEntry();
    bool canBeTrashed();
    bool isForage(IGameLocationWrapper location);
    void initializeLightSource(Vector2 tileLocation, bool mineShaft = false);
    void performRemoveAction(Vector2 tileLocation, IGameLocationWrapper environment);
    void dropItem(IGameLocationWrapper location, Vector2 origin, Vector2 destination);
    bool isPassable();
    void reloadSprite();
    void consumeRecipe(IFarmerWrapper who);
    Rectangle getBoundingBox(Vector2 tileLocation);
    bool canBeGivenAsGift();
    bool performDropDownAction(IFarmerWrapper who);
    void MonsterMusk(IFarmerWrapper who);
    string[] ModifyItemBuffs(string[] buffs);
    bool performUseAction(IGameLocationWrapper location);
    Color getCategoryColor();
    string getCategoryName();
    bool isActionable(IFarmerWrapper who);
    int getHealth();
    void setHealth(int health);
    int healthRecoveredOnConsumption();
    int staminaRecoveredOnConsumption();
    bool checkForAction(IFarmerWrapper who, bool justCheckingForActivity = false);
    void AttemptAutoLoad(IFarmerWrapper who);
    void farmerAdjacentAction(IGameLocationWrapper location);
    void addWorkingAnimation(IGameLocationWrapper environment);
    void onReadyForHarvest(IGameLocationWrapper environment);
    bool minutesElapsed(int minutes, IGameLocationWrapper environment);
    string checkForSpecialItemHoldUpMeessage();
    bool countsForShippedCollection();
    Vector2 getScale();
    void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, IFarmerWrapper f);
    void drawPlacementBounds(SpriteBatch spriteBatch, IGameLocationWrapper location);

    void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow);

    void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber);
    void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth);
    void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize);
    void drawAsProp(SpriteBatch b);
    void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f);

    void draw(
      SpriteBatch spriteBatch,
      int xNonTile,
      int yNonTile,
      float layerDepth,
      float alpha = 1f);

    int maximumStackSize();
    int addToStack(IItemWrapper otherStack);
    void hoverAction();
    bool clicked(IFarmerWrapper who);
    IItemWrapper getOne();
    void _GetOneFrom(IItemWrapper source);
    bool canBePlacedHere(IGameLocationWrapper l, Vector2 tile);
    bool isPlaceable();
    bool IsConsideredReadyMachineForComputer();
    bool isSapling();
    bool IsSprinkler();
    int GetModifiedRadiusForSprinkler();
    int GetBaseRadiusForSprinkler();
    bool placementAction(IGameLocationWrapper location, int x, int y, IFarmerWrapper who = null);
    bool actionWhenPurchased();
    bool canBePlacedInWater();
    bool needsToBeDonated();
    string getDescription();
    int sellToStorePrice(long specificPlayerID = -1);
    int salePrice();
    bool IsInfiniteStock();
    void MarkContextTagsDirty();
    IEnumerable<string> GetContextTagList();
    IEnumerable<string> GetContextTags();
    bool HasContextTag(string tag);
    string SanitizeContextTag(string tag);
    bool ShouldSerializeparentSheetIndex();
    void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText);
    Point getExtraSpaceNeededForTooltipSpecialIcons(SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight, StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom);
    bool ShouldDrawIcon();
    bool CanBuyItem(IFarmerWrapper who);
    bool canBeDropped();
    int getRemainingStackSpace();
    string getHoverBoxText(IItemWrapper hoveredItem);
    void drawAttachments(SpriteBatch b, int x, int y);
    int attachmentSlots();
    bool canStackWith(ISalableWrapper other);
    ISalableWrapper GetSalableInstance();
    int CompareTo(object obj);
    int getCategorySortValue();
    void resetState();
  }
}