/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using MarketTown;
using Netcode;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Object = StardewValley.Object;

namespace MarketTown
{
    public partial class ModEntry
    {
        private void UpdateOrders()
        {
            foreach (var c in Game1.player.currentLocation.characters)
            {
                if (c.IsVillager)
                {
                    CheckOrder(c, Game1.player.currentLocation);
                }
                else
                {
                    c.modData.Remove(orderKey);
                }
            }
        }

        private void CheckOrder(NPC npc, GameLocation location)
        {
            Random rand = new Random();
            if (npc.modData.TryGetValue(orderKey, out string orderData) && rand.NextDouble() < Config.OrderChance)
            {
                //npc.modData.Remove(orderKey);
                UpdateOrder(npc, JsonConvert.DeserializeObject<OrderData>(orderData));
                return;
            }
            if (!Game1.NPCGiftTastes.ContainsKey(npc.Name) || npcOrderNumbers.Value.TryGetValue(npc.Name, out int amount) && amount >= Config.MaxNPCOrdersPerNight)
                return;
            if (rand.NextDouble() < Config.OrderChance)
            {
                StartOrder(npc, location);
            }
        }

        private void UpdateOrder(NPC npc, OrderData orderData)
        {
            if (!npc.IsEmoting)
            {
                npc.doEmote(424242, false);
            }
        }

        private void StartOrder(NPC npc, GameLocation location)
        {
            List<int> loves = new();
            foreach (var str in Game1.NPCGiftTastes["Universal_Love"].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    loves.Add(int.Parse(str));
                }
            }
            foreach (var str in Game1.NPCGiftTastes[npc.Name].Split('/')[1].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    loves.Add(int.Parse(str));
                }
            }

            List<int> likes = new();
            foreach (var str in Game1.NPCGiftTastes["Universal_Like"].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    likes.Add(int.Parse(str));
                }
            }
            foreach (var str in Game1.NPCGiftTastes[npc.Name].Split('/')[3].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    likes.Add(int.Parse(str));
                }
            }

            List<int> neutral = new();
            foreach (var str in Game1.NPCGiftTastes["Universal_Neutral"].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    neutral.Add(int.Parse(str));
                }
            }
            foreach (var str in Game1.NPCGiftTastes[npc.Name].Split('/')[9].Split(' '))
            {
                if (Game1.objectData.ContainsKey(str) && CraftingRecipe.cookingRecipes.ContainsKey(Game1.objectData[str].Name))
                {
                    neutral.Add(int.Parse(str));
                }
            }
            Random rand = new Random();

            string loved = "neutral";
            int dish = 216;

            if (!loves.Any() && !likes.Any() && !neutral.Any())
                return;
            try
            {
                if (!likes.Any() && !neutral.Any() && loved.Any() || loved.Any() && rand.NextDouble() < Config.LovedDishChance)
                {
                    loved = "love";
                    dish = loves[rand.Next(loves.Count)];
                }
                else
                {
                    if (rand.NextDouble() < 0.0 && likes.Any())
                    {
                        loved = "like";
                        dish = likes[rand.Next(likes.Count)];
                    }
                    else if (neutral.Any())
                    {
                        loved = "neutral";
                        dish = neutral[rand.Next(neutral.Count)];
                    }
                }
            } catch { }
            var name = Game1.objectData[dish.ToString()].Name;
            int price = 0;
            price = (int)(Game1.objectData[dish.ToString()].Price);


            npc.modData[orderKey] = JsonConvert.SerializeObject(new OrderData(dish, name, price, loved));
        }
    }
}