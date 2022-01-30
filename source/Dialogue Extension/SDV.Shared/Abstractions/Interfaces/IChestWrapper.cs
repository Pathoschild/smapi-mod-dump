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
using StardewValley.Network;
using StardewValley.Objects;

namespace SDV.Shared.Abstractions
{
  public interface IChestWrapper : IObjectWrapper, INetObject<NetFields>
  {
    Color Tint { get; set; }
    Chest.SpecialChestTypes SpecialChestType { get; set; }
    void resetLidFrame();
    void fixLidFrame();
    int getLastLidFrame();
    void addContents(int coins, IItemWrapper item);

    bool MoveToSafePosition(
      IGameLocationWrapper location,
      Vector2 tile_position,
      int depth = 0,
      Vector2? prioritize_direction = null);
    
    void destroyAndDropContents(Vector2 pointToDropAt, IGameLocationWrapper location);
    void dumpContents(IGameLocationWrapper location);
    NetMutex GetMutex();
    void OpenMiniShippingMenu();
    void performOpenChest();
    void grabItemFromChest(IItemWrapper item, IFarmerWrapper who);
    IItemWrapper addItem(IItemWrapper item);
    int GetActualCapacity();
    void CheckAutoLoad(IFarmerWrapper who);
    void ShowMenu();
    void grabItemFromInventory(IItemWrapper item, IFarmerWrapper who);
    NetObjectList<IItemWrapper> GetItemsForPlayer(long id);
    bool isEmpty();
    void clearNulls();
    void UpdateFarmerNearby(IGameLocationWrapper location, bool animate = true);

    void SetBigCraftableSpriteIndex(
      int sprite_index,
      int starting_lid_frame = -1,
      int lid_frame_count = 3);

    void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool local = false);
  }
}