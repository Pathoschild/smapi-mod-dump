using BetterPanning.Data;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace BetterPanning.GamePatch
{
   static  class PanOverrider
   {
        public static void postfix_getPanItems(ref List<Item> __result)
        {
            __result = PanOverrider.GetTreasure();            
        }

        internal static List<Item> GetTreasure()
        {
            List<Item> rewards = new List<Item>();

            //Treasure Groups
            List<TreasureGroup> possibleGroups = PanningMod.Instance.treasureGroups.Values
                .Where(group => group.Enabled == true)
                .OrderBy(group => group.GroupChance)
                .ToList();
            
            // Select rewards
            double chance = 1f;

            while (possibleGroups.Count > 0 && Game1.random.NextDouble() <= chance)
            {
                TreasureGroup group = possibleGroups.ChooseItem(Game1.random);
                
                // Possible treasure based on selected treasure group selected above.
                List<TreasureData> possibleLoot = new List<TreasureData>(group.treasureList)
                    .Where(loot => loot.Enabled)
                    .OrderBy(loot => loot.Chance)
                    .ThenBy(loot => loot.Id)
                    .ToList();

                if (possibleLoot.Count == 0)
                {
                    PanningMod.Instance.Monitor.Log($"   Group: {group.GroupID}, No Possible Loot Found... check the logic");                    
                    break;
                }

                TreasureData treasure = possibleLoot.ChooseItem(Game1.random);
                int id = treasure.Id;

                // Lost books have custom handling
                if (id == 102) // LostBook Item ID
                {
                    if (Game1.player.archaeologyFound == null || !Game1.player.archaeologyFound.ContainsKey(102) || Game1.player.archaeologyFound[102][0] >= 21)
                    {
                        possibleLoot.Remove(treasure);
                        continue;
                    }
                    Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                }

                // Create reward item
                Item reward;
                if (group.GroupID == TREASURE_GROUP.Rings)                    
                {
                    reward = new Ring(id);
                }
                else if (group.GroupID == TREASURE_GROUP.Boots)
                {
                    reward = new Boots(id);
                }
                else
                {
                    // Random quantity
                    int count = Game1.random.Next(treasure.MinAmount, treasure.MaxAmount);

                    reward = new StardewValley.Object(id, count);
                }

                // Add the reward
                rewards.Add(reward);

                // Check if this reward shouldn't be duplicated
                if (!treasure.AllowDuplicates)
                    possibleLoot.Remove(treasure);

                // Update chance
                chance *= PanningMod.Instance.config.additionalLootChance + Game1.player.DailyLuck; 
            }

            return rewards;
        }
    }
}
