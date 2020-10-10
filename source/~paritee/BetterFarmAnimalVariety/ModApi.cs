/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using Paritee.StardewValleyAPI.Buildings.AnimalShop;
using Paritee.StardewValleyAPI.Buildings.AnimalShop.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using Paritee.StardewValleyAPI.Players;
using Paritee.StardewValleyAPI.Players.Actions;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterFarmAnimalVariety
{
    public class ModApi
    {
        private readonly ModConfig Config;
        private readonly ISemanticVersion ModVersion;

        public ModApi(ModConfig config, ISemanticVersion modVersion)
        {
            this.Config = config;
            this.ModVersion = modVersion;
        }

        /// <returns>Returns bool</returns>
        public bool IsEnabled(string version)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            return this.Config.IsEnabled;
        }

        /// <param name="version">string</param>
        /// <returns>Returns Dictionary<string, string[]></returns>
        public Dictionary<string, string[]> GetFarmAnimalsByCategory(string version)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            return this.Config.FarmAnimals.ToDictionary(entry => entry.Key, entry => entry.Value.Types);
        }

        /// <param name="version">string</param>
        /// <param name="player">Paritee.StardewValleyAPI.Players</param>
        /// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.Blue</returns>
        public BlueVariation GetBlueFarmAnimals(string version, Player player)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            BlueConfig blueConfig = new BlueConfig(player.HasSeenEvent(BlueVariation.EVENT_ID));

            return new BlueVariation(blueConfig);
        }

        /// <param name="version">string</param>
        /// <param name="player">Paritee.StardewValleyAPI.Players</param>
        /// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.Void</returns>
        public VoidVariation GetVoidFarmAnimals(string version, Player player)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            VoidConfig voidConfig = new VoidConfig(this.Config.VoidFarmAnimalsInShop, player.HasCompletedQuest(VoidVariation.QUEST_ID));

            return new VoidVariation(voidConfig);
        }

        /// <param name="version">string</param>
        /// <param name="farm">StardewValley.Farm</param>
        /// <param name="blueFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.BlueVariation</param>
        /// <param name="voidFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.VoidVariation</param>
        /// <returns>Returns Paritee.StardewValleyAPI.Buidlings.AnimalShop</returns>
        public AnimalShop GetAnimalShop(string version, Farm farm, BlueVariation blueFarmAnimals, VoidVariation voidFarmAnimals)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            List<FarmAnimalForPurchase> farmAnimalsForPurchase = this.Config.GetFarmAnimalsForPurchase(farm);
            StockConfig stockConfig = new StockConfig(farmAnimalsForPurchase, blueFarmAnimals, voidFarmAnimals);
            Stock stock = new Stock(stockConfig);

            return new AnimalShop(stock);
        }

        /// <param name="version">string</param>
        /// <param name="player">Paritee.StardewValleyAPI.Players</param>
        /// <param name="blueFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.BlueVariation</param>
        /// <returns>Returns Paritee.StardewValleyAPI.Players.Actions.BreedFarmAnimal</returns>
        public BreedFarmAnimal GetBreedFarmAnimal(string version, Player player, BlueVariation blueFarmAnimals)
        {
            if (!this.IsVersionSupported(version))
            {
                throw new NotSupportedException();
            }

            Dictionary<string, List<string>> farmAnimals = this.Config.GetFarmAnimalTypes();
            BreedFarmAnimalConfig breedFarmAnimalConfig = new BreedFarmAnimalConfig(farmAnimals, blueFarmAnimals, this.Config.RandomizeNewbornFromCategory, this.Config.RandomizeHatchlingFromCategory, this.Config.IgnoreParentProduceCheck);
            return new BreedFarmAnimal(player, breedFarmAnimalConfig);
        }

        private bool IsVersionSupported(string version)
        {
            // Must match the major version
            return version == this.ModVersion.MajorVersion.ToString();
        }
    }
}
