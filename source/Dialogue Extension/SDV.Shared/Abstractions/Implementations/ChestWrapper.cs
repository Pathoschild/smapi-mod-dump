/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;

namespace SDV.Shared.Abstractions
{
  public class ChestWrapper : ObjectWrapper, IChestWrapper
  {
    public ChestWrapper(Object item) : base(item)
    {
    }

    public Color Tint { get; set; }
    public Chest.SpecialChestTypes SpecialChestType { get; set; }
    public void resetLidFrame()
    {
    }

    public void fixLidFrame()
    {
    }

    public int getLastLidFrame() => 0;
    
    public void addContents(int coins, IItemWrapper item)
    {
    }

    public bool MoveToSafePosition(IGameLocationWrapper location, Vector2 tile_position, int depth = 0,
      Vector2? prioritize_direction = null) =>
      false;

    public void destroyAndDropContents(Vector2 pointToDropAt, IGameLocationWrapper location)
    {
    }

    public void dumpContents(IGameLocationWrapper location)
    {
    }

    public NetMutex GetMutex() => null;

    public void OpenMiniShippingMenu()
    {
    }

    public void performOpenChest()
    {
    }

    public void grabItemFromChest(IItemWrapper item, IFarmerWrapper who)
    {
    }

    public IItemWrapper addItem(IItemWrapper item) => null;

    public int GetActualCapacity() => 0;

    public void CheckAutoLoad(IFarmerWrapper who)
    {
    }

    public void ShowMenu()
    {
    }

    public void grabItemFromInventory(IItemWrapper item, IFarmerWrapper who)
    {
    }

    public NetObjectList<IItemWrapper> GetItemsForPlayer(long id) => null;

    public bool isEmpty() => false;

    public void clearNulls()
    {
    }

    public void UpdateFarmerNearby(IGameLocationWrapper location, bool animate = true)
    {
    }

    public void SetBigCraftableSpriteIndex(int sprite_index, int starting_lid_frame = -1, int lid_frame_count = 3)
    {
    }

    public void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1, bool local = false)
    {
    }
  }
}
