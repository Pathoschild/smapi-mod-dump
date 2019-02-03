using Paritee.StardewValleyAPI.Buildings.AnimalShop;
using Paritee.StardewValleyAPI.Buildings.AnimalShop.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using Paritee.StardewValleyAPI.Players;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace BetterFarmAnimalVariety
{
    public class ModApi
    {
        private ModConfig Config;

        public ModApi(ModConfig config)
        {
            this.Config = config;
        }

        /// <returns>Returns Dictionary<string, string[]></returns>
        public Dictionary<string, string[]> GetFarmAnimalsByCategory()
        {
            return this.Config.FarmAnimals.ToDictionary(entry => entry.Key, entry => entry.Value.Types);
        }

        /// <param name="player">Paritee.StardewValleyAPI.Players</param>
        /// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.Blue</returns>
        public BlueVariation GetBlueFarmAnimals(Player player)
        {
            BlueConfig blueConfig = new BlueConfig(player.HasSeenEvent(BlueVariation.EVENT_ID));

            return new BlueVariation(blueConfig);
        }

        /// <param name="player">Paritee.StardewValleyAPI.Players</param>
        /// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.Void</returns>
        public VoidVariation GetVoidFarmAnimals(Player player)
        {
            VoidConfig voidConfig = new VoidConfig(this.Config.VoidFarmAnimalsInShop, player.HasCompletedQuest(VoidVariation.QUEST_ID));

            return new VoidVariation(voidConfig);
        }

        /// <param name="farm">StardewValley.Farm</param>
        /// <param name="blueFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.BlueVariation</param>
        /// <param name="voidFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.VoidVariation</param>
        /// <returns>Returns Paritee.StardewValleyAPI.Buidlings.AnimalShop</returns>
        public AnimalShop GetAnimalShop(Farm farm, BlueVariation blueFarmAnimals, VoidVariation voidFarmAnimals)
        {
            List<FarmAnimalForPurchase> farmAnimalsForPurchase = this.Config.GetFarmAnimalsForPurchase(farm);
            StockConfig stockConfig = new StockConfig(farmAnimalsForPurchase, blueFarmAnimals, voidFarmAnimals);
            Stock stock = new Stock(stockConfig);

            return new AnimalShop(stock);
        }
    }
}
