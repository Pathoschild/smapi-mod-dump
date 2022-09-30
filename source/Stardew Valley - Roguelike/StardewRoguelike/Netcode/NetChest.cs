/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
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

        private bool wasSpawned = false;

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

        protected void InitNetFields()
        {
            NetFields.AddFields(TileLocation, Items, IndeterminateItemChest);
        }

        // Is only called for Game1.player
        public void Spawn(MineShaft mine)
        {
            if (wasSpawned)
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

            Chest chest = new(0, items, TileLocation.Value)
            {
                Tint = Color.White
            };
            mine.overlayObjects[TileLocation.Value] = chest;

            wasSpawned = true;
        }
    }
}
