/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using System;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace OrganizeInventory
{
    public class ModEntry : Mod
    {
        // - Index 0 is the left-most slot of the first row.
        // - Index 12 is the left-most slot of the second row.
        // - Index 24 is the left-most slot of the third row.
        private static readonly Dictionary<string, List<int>> ITEMS_TO_SLOTS = new Dictionary<string, List<int>>
        {
            { "Galaxy Sword", new List<int>(){ 0 } },
            { "Iridium Pickaxe", new List<int>(){ 1 } },
            { "Master Slingshot", new List<int>(){ 2 } },
            { "Warp Totem: Desert", new List<int>(){ 3 } },
            { "Cherry Bomb", new List<int>(){ 4 } },
            { "Bomb", new List<int>(){ 5 } },
            { "Mega Bomb", new List<int>(){ 6 } },

            { "Staircase", new List<int>(){ 8 } },
            { "Cheese", new List<int>(){ 9 } },
            { "Triple Shot Espresso", new List<int>(){ 10 } },
            { "Spicy Eel", new List<int>(){ 11 } },

            { "Cloth", new List<int>(){ 12 }},
            { "Crab Cakes", new List<int>(){ 13 }},
            { "Fire Quartz", new List<int>(){ 14 }},
            { "Prismatic Shard", new List<int>(){ 15 }},
            { "Topaz", new List<int>(){ 16 }},
            { "Amethyst", new List<int>(){ 17 }},
            { "Aquamarine", new List<int>(){ 18 }},
            { "Jade", new List<int>(){ 19 }},
            { "Emerald", new List<int>(){ 20 }},
            { "Ruby", new List<int>(){ 21 }},
            { "Diamond", new List<int>(){ 22 }},
            { "Warp Totem: Farm", new List<int>(){ 23 }},

            { "Lucky Ring", new List<int>(){ 24, 25 }},
            { "Void Essence", new List<int>(){ 26 } },
            { "Solar Essence", new List<int>(){ 27 } },
            { "Iridium Ore", new List<int>(){ 28 } },
            { "Gold Ore", new List<int>(){ 29 } },
            { "Iron Ore", new List<int>(){ 30 } },
            { "Copper Ore", new List<int>(){ 31 } },
            { "Coal", new List<int>(){ 32 } },
            { "Omni Geode", new List<int>(){ 33 } },
            { "Quartz", new List<int>(){ 34 } },
            { "Stone", new List<int>(){ 35 } },
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (e.Button.ToString() == "O")
            {
                OrganizeInventory();
            }
        }

        private void OrganizeInventory()
        {
            foreach (var (name, slots) in ITEMS_TO_SLOTS)
            {
                var existingSlots = GetSlotsOfItem(name);

                if (existingSlots.Count == 0)
                {
                    // The item is not in our inventory.
                    continue;
                }

                // Prune out the items that are already sorted.
                existingSlots.RemoveAll(existingSlot => slots.Contains(existingSlot));

                for (int i = 0; i < slots.Count; i++)
                {
                    int slot = slots[i];

                    if (existingSlots.Count >= i + 1)
                    {
                        int existingSlot = existingSlots[i];
                        SwapItem(existingSlot, slot);
                    }
                }
            }
        }

        private List<int> GetSlotsOfItem(string name)
        {
            var slots = new List<int>();

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                var item = Game1.player.Items[i];
                if (item == null)
                {
                    continue;
                }

                if (item.Name == name)
                {
                    slots.Add(i);
                }
            }

            return slots;
        }

        private void SwapItem(int slot1, int slot2)
        {
            var item1 = Game1.player.Items[slot1];
            var item2 = Game1.player.Items[slot2];

            string item1Name = item1?.Name ?? "n/a";
            string item2Name = item2?.Name ?? "n/a";

            Game1.player.Items[slot1] = item2;
            Game1.player.Items[slot2] = item1;
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
