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
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ItemWrapper : IItemWrapper
  {
    public ItemWrapper(Item item) => GetBaseType = item;
    public Item GetBaseType { get; }
    public IItemWrapper NetFields { get; }
    public void MarkDirty()
    {
    }

    public void MarkClean()
    {
    }

    public bool Tick() => false;

    public void Read(BinaryReader reader, NetVersion version)
    {
    }

    public void Write(BinaryWriter writer)
    {
    }

    public void ReadFull(BinaryReader reader, NetVersion version)
    {
    }

    public void WriteFull(BinaryWriter writer)
    {
    }

    public uint DirtyTick { get; set; }
    public bool Dirty { get; }
    public bool NeedsTick { get; set; }
    public bool ChildNeedsTick { get; set; }
    public INetSerializable Parent { get; set; }
    public INetRoot Root { get; }
    public bool ShouldDrawIcon() => false;

    public void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
      StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
    }

    public string getDescription() => null;

    public int maximumStackSize() => 0;

    public int addToStack(Item stack) => 0;

    public int salePrice() => 0;

    public bool actionWhenPurchased() => false;

    public bool canStackWith(ISalable other) => false;

    public bool CanBuyItem(Farmer farmer) => false;

    public bool IsInfiniteStock() => false;

    public ISalableWrapper GetSalableInstance() => null;

    public int addToStack(IItemWrapper stack) => 0;

    public bool CanBuyItem(IFarmerWrapper farmer) => false;

    public bool canStackWith(ISalableWrapper other) => false;

    ISalable ISalable.GetSalableInstance() => GetSalableInstance();

    public string DisplayName { get; }
    public string Name { get; }
    public int Stack { get; set; }
    public int SpecialVariable { get; set; }
    public int Category { get; set; }
    public bool HasBeenInInventory { get; set; }
    public int ParentSheetIndex { get; set; }
    public IModDataDictionaryWrapper modDataForSerialization { get; set; }
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

    public string[] ModifyItemBuffs(string[] buffs)
    {
      return new string[] { };
    }

    public Point getExtraSpaceNeededForTooltipSpecialIcons(SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight,
      StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom) =>
      default;

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

    public bool isPlaceable() => false;

    public bool canBeTrashed() => false;

    public bool canBePlacedInWater() => false;

    public bool canBeDropped() => false;

    public void actionWhenBeingHeld(IFarmerWrapper who)
    {
    }

    public void actionWhenStopBeingHeld(IFarmerWrapper who)
    {
    }

    public int getRemainingStackSpace() => 0;

    public int healthRecoveredOnConsumption() => 0;

    public int staminaRecoveredOnConsumption() => 0;

    public string getHoverBoxText(IItemWrapper hoveredItem) => null;

    public bool canBeGivenAsGift() => false;

    public void drawAttachments(SpriteBatch b, int x, int y)
    {
    }

    public bool canBePlacedHere(IGameLocationWrapper l, Vector2 tile) => false;

    public int attachmentSlots() => 0;

    public string getCategoryName() => null;

    public Color getCategoryColor() => default;

    public string checkForSpecialItemHoldUpMeessage() => null;

    public IItemWrapper getOne() => null;

    public void _GetOneFrom(IItemWrapper source)
    {
    }

    public int CompareTo(object obj) => 0;

    public int getCategorySortValue() => 0;

    public void resetState()
    {
    }
  }
}
