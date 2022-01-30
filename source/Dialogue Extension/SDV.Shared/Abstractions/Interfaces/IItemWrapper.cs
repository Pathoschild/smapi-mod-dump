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
  public interface IItemWrapper : IWrappedType<Item>, INetObject<IItemWrapper>, INetSerializable, ISalableWrapper
  {
    int SpecialVariable { get; set; }
    int Category { get; set; }
    bool HasBeenInInventory { get; set; }
    int ParentSheetIndex { get; set; }
    IModDataDictionaryWrapper modDataForSerialization { get; set; }
    void MarkContextTagsDirty();
    IEnumerable<string> GetContextTagList();
    IEnumerable<string> GetContextTags();
    bool HasContextTag(string tag);
    string SanitizeContextTag(string tag);
    bool ShouldSerializeparentSheetIndex();

    void drawTooltip(
      SpriteBatch spriteBatch,
      ref int x,
      ref int y,
      SpriteFont font,
      float alpha,
      StringBuilder overrideText);

    string[] ModifyItemBuffs(string[] buffs);

    Point getExtraSpaceNeededForTooltipSpecialIcons(
      SpriteFont font,
      int minWidth,
      int horizontalBuffer,
      int startingHeight,
      StringBuilder descriptionText,
      string boldTitleText,
      int moneyAmountToDisplayAtBottom);

    void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber);

    void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth);

    void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize);
    bool isPlaceable();
    bool canBeTrashed();
    bool canBePlacedInWater();
    bool canBeDropped();
    void actionWhenBeingHeld(IFarmerWrapper who);
    void actionWhenStopBeingHeld(IFarmerWrapper who);
    int getRemainingStackSpace();
    int healthRecoveredOnConsumption();
    int staminaRecoveredOnConsumption();
    string getHoverBoxText(IItemWrapper hoveredItem);
    bool canBeGivenAsGift();
    void drawAttachments(SpriteBatch b, int x, int y);
    bool canBePlacedHere(IGameLocationWrapper l, Vector2 tile);
    int attachmentSlots();
    string getCategoryName();
    Color getCategoryColor();
    string checkForSpecialItemHoldUpMeessage();
    IItemWrapper getOne();
    void _GetOneFrom(IItemWrapper source);
    int CompareTo(object obj);
    int getCategorySortValue();
    void resetState();
  }
}