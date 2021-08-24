/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Linq;
using ExpandedStorage.Framework.Controllers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ExpandedStorage.Framework.Extensions
{
    internal static class ItemExtensions
    {
        private static AssetController Assets;

        public static void Init(AssetController assets)
        {
            Assets = assets;
        }

        public static Chest ToChest(this Item item, StorageController storage, Chest oldChest = null)
        {
            // Create Chest from Item
            var chest = new Chest(true, Vector2.Zero, item.ParentSheetIndex)
            {
                name = item.Name,
                SpecialChestType = Enum.TryParse(storage.SpecialChestType, out Chest.SpecialChestTypes specialChestType)
                    ? specialChestType
                    : Chest.SpecialChestTypes.None
            };
            chest.fridge.Value = storage.IsFridge;

            // Add held object chest
            if (storage.HeldStorage)
            {
                var heldChest = new Chest(true, Vector2.Zero, item.ParentSheetIndex);
                if (item is Object obj && obj.heldObject.Value is Chest oldHeldChest && oldHeldChest.items.Any())
                    heldChest.items.CopyFrom(oldHeldChest.items);
                chest.heldObject.Value = heldChest;
            }

            chest.lidFrameCount.Value = Math.Max(storage.Frames, 1);

            // Copy modData from original item
            foreach (var modData in item.modData)
                chest.modData.CopyFrom(modData);

            // Copy modData from config
            foreach (var modData in storage.ModData)
            {
                if (!chest.modData.ContainsKey(modData.Key))
                    chest.modData.Add(modData.Key, modData.Value);
            }

            // Add modData for Storage Type
            chest.modData["furyx639.ExpandedStorage/Storage"] = Assets.Storages.FirstOrDefault(s => s.Value.Equals(storage)).Key;

            oldChest ??= item is Chest oldItemChest ? oldItemChest : null;
            if (oldChest == null) return chest;

            chest.playerChoiceColor.Value = oldChest.playerChoiceColor.Value;
            if (oldChest.items.Any())
                chest.items.CopyFrom(oldChest.items);

            return chest;
        }
    }
}