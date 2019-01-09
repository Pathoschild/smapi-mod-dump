using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MTN2.Compatibility {
    public class forageSpawns : spawn<SObject> {
        Dictionary<string, int> itemList = new Dictionary<string, int>() {
            {"Wild Horseradish", 16 },
            {"WildHorseradish", 16 },
            {"Horseradish", 16 },
            {"Daffodil", 18 },
            {"Leek", 20 },
            {"Dandelion", 22 },
            {"Coconut", 88 },
            {"Cactus Fruit", 90 },
            {"CactusFruit", 90 },
            {"Morel", 257 },
            {"Fiddlehead Fern", 259 },
            {"FiddleheadFern", 259 },
            {"Fiddlehead", 259 },
            {"Chanterelle", 281 },
            {"Holly", 283 },
            {"Spice Berry", 396 },
            {"SpiceBerry", 396 },
            {"Grape", 398 },
            {"Spring Onion", 399 },
            {"SpringOnion", 399 },
            {"Sweet Pea", 402 },
            {"SweetPea", 402 },
            {"Common Mushroom", 404 },
            {"CommonMushroom", 404 },
            {"Wild Plum", 406 },
            {"WildPlum", 406 },
            {"Hazelnut", 408 },
            {"Blackberry", 410 },
            {"Winter Root", 412 },
            {"WinterRoot", 412 },
            {"Crystal Fruit", 414 },
            {"CrystalFruit", 414 },
            {"Snow Yam", 416 },
            {"SnowYam", 416 },
            {"Crocus", 418 },
            {"Red Mushroom", 420 },
            {"RedMushroom", 420 },
            {"Purple Mushroom", 422 },
            {"PurpleMushroom", 422 }
        };

        public void checkIntegrity() {
            int results;

            if (itemId > 0) return;
            if (itemId < 0) {
                itemId = 0;
                return;
            }
            if (itemList.TryGetValue(itemName, out results)) {
                itemId = results;
            } else {
                itemId = 0;
                return;
            }
            if (minimumAmount < 1) minimumAmount = 1;
            if (maximumAmount < 0) maximumAmount = 0;
            if (minCooldown < 1) minCooldown = 1;
            if (maxCooldown < 0) maxCooldown = 0;
            return;
        }
    }
}
