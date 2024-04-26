/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jslattery26/stardew_mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
#nullable enable
namespace ChestPoolingV2
{
    public static class ChestExtensions
    {
        public static void Log(this string s)
        {
            ChestPoolingV2Mod.Log(s);
        }

        public static Chest? GetBestChest(this List<Chest> chests, Item item)
        {
            List<(int, Chest)> chestScores = chests.Select(chest => (chest.ChestScore(item), chest)).ToList();
            chestScores.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            chestScores.RemoveAll(score => score.Item1 == 0);
            List<Chest> bestChests = chestScores.Select(score => score.Item2).ToList();
            return bestChests.DefaultIfEmpty(null).First();
        }

        public static bool HasEmptySlotsThatWorks(this Chest chest)
        {
            return chest.Items.Count < Chest.capacity || chest.Items.Any(i => i == null);
        }

        public static bool ChestAlreadyHasItems(this Chest chest, Item item)
        {
            if (!chest.Items.HasAny())
            {
                return false;
            }

            return chest.Items.ContainsId(item.QualifiedItemId);
        }

        public static int ChestScore(this Chest chest, Item item)
        {
            bool chestEmpty = !chest.Items.HasAny();
            bool chestHasItem = chest.Items.ContainsId(item.QualifiedItemId);
            if (chestEmpty || !chestHasItem)
            {
                return 0;
            }
            if (!chest.HasEmptySlotsThatWorks())
            {
                Log("Chest is full");
                Log($"Chest has {chest.Items.Count} slots");
                Log($"Chest has {chest.Items.CountItemStacks()} slots with items");
                Game1.addHUDMessage(new HUDMessage("One of your chests is full.", HUDMessage.error_type) { timeLeft = 1000 });
                return 0;
            }
            int stacks = chest.Items.CountId(item.QualifiedItemId);
            Log($"Chest has {stacks} stacks of {item.Name}");
            return stacks;
        }

        public static void GrabItemFromInventoryFromOtherChest(this Chest chest, Item item, Farmer who)
        {
            if (item.Stack == 0)
            {
                item.Stack = 1;
            }

            Item item2 = chest.addItem(item);
            if (item2 == null)
            {
                who.removeItemFromInventory(item);
            }
            else
            {
                item2 = who.addItemToInventory(item2);
            }

            chest.clearNulls();
            int num = (Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1);
            // ! This is the same as Chest.grabItemFromInventory() but removed showMenu cause it shows the non-current chest
            if (Game1.activeClickableMenu is ItemGrabMenu menu)
            {
                menu.heldItem = item2;
            }
            if (num != -1)
            {
                Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(num);
                Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
        }
    }



}