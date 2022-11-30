/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike.Netcode
{
    public class NetChest : INetObject<NetFields>
    {
        public NetFields NetFields { get; } = new();

        public readonly NetVector2 TileLocation = new();

        public readonly NetObjectList<Item> Items = new();

        public readonly NetBool IndeterminateItemChest = new();

        public readonly NetBool IsTimedChest = new();

        public readonly NetLong MustOpenBy = new();

        public bool Spawned { get; private set; } = false;

        public NetChest()
        {
            InitNetFields();
        }

        public NetChest(Vector2 tileLocation, List<Item> items) : this()
        {
            TileLocation.Value = tileLocation;
            Items.CopyFrom(items);
        }

        public NetChest(Vector2 tileLocation) : this()
        {
            TileLocation.Value = tileLocation;
            IndeterminateItemChest.Value = true;
        }

        public NetChest(Vector2 tileLocation, long mustOpenBy) : this(tileLocation)
        {
            MustOpenBy.Value = mustOpenBy;
            IsTimedChest.Value = true;
        }

        public NetChest(Vector2 tileLocation, List<Item> items, long mustOpenBy) : this(tileLocation, items)
        {
            MustOpenBy.Value = mustOpenBy;
            IsTimedChest.Value = true;
        }

        protected void InitNetFields()
        {
            NetFields.AddFields(TileLocation, Items, IndeterminateItemChest, IsTimedChest, MustOpenBy);
        }

        // Is only called for Game1.player
        public void Spawn(MineShaft mine)
        {
            if (Spawned)
                return;

            List<Item> items;
            if (IndeterminateItemChest.Value && Items.Count == 0)
            {
                items = new();
                List<string> playerItems = new();

                foreach (Item playerItem in Game1.player.Items)
                {
                    if (playerItem is MeleeWeapon || playerItem is Ring || playerItem is Boots)
                        playerItems.Add(playerItem.DisplayName);
                }

                if (Game1.player.boots.Value is not null)
                    playerItems.Add(Game1.player.boots.Value.DisplayName);
                if (Game1.player.leftRing.Value is not null)
                    playerItems.Add(Game1.player.leftRing.Value.DisplayName);
                if (Game1.player.rightRing.Value is not null)
                    playerItems.Add(Game1.player.rightRing.Value.DisplayName);

                Item item = Merchant.GetNextMerchantFloor(mine).PickAnyRandomAvoiding(playerItems);
                items.Add(item);
            }
            else
                items = Items.ToList();

            Chest chest;
            if (IsTimedChest.Value)
            {
                chest = new TimedChest(MustOpenBy.Value, items, TileLocation.Value)
                {
                    Tint = Color.White
                };
            }
            else
            {
                chest = new(0, items, TileLocation.Value)
                {
                    Tint = Color.White
                };
            }

            mine.overlayObjects[TileLocation.Value] = chest;

            Spawned = true;
        }
    }
}
