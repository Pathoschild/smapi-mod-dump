using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.Framework;
using static TehPers.Stardew.Framework.Helpers;

namespace TehPers.Stardew.FishingOverhaul.Configs {
    public class ConfigTreasure {
        public TreasureData[] PossibleLoot { get; set; } = {
            new TreasureData(Objects.DRESSED_SPINNER, 0.025, 1, 1, 4, 6, allowDuplicates: false),
            new TreasureData(Objects.BAIT, 0.25, 2, 4),
            //new TreasureData(Objects.BAIT, 0.05, 10), // No need for this I guess

            // Archaeology
            new TreasureData(Objects.LOST_BOOK, 0.025, allowDuplicates: false),
            new TreasureData(585, 0.0625, idRange: 4), // Archaeology, part 1
            new TreasureData(96, 0.125, idRange: 32), // Archaeology, part 2
            //new TreasureData(Objects.STRANGE_DOLL1, 0.0025),
            //new TreasureData(Objects.STRANGE_DOLL2, 0.0025),

            new TreasureData(Objects.GEODE, 0.2, 1, 3),
            new TreasureData(Objects.FROZEN_GEODE, 0.125, 1, 3),
            new TreasureData(Objects.MAGMA_GEODE, 0.125, 1, 3),
            new TreasureData(Objects.OMNI_GEODE, 0.0625, 1, 3),

            new TreasureData(Objects.IRIDIUM_ORE, 0.0075, 1, 3, 5),
            new TreasureData(Objects.GOLD_ORE, 0.15, 3, 10, 4),
            new TreasureData(Objects.IRON_ORE, 0.15, 3, 10, 3),
            new TreasureData(Objects.COPPER_ORE, 0.15, 3, 10),
            new TreasureData(Objects.COAL, 0.3, 3, 10),

            // Junk
            new TreasureData(Objects.WOOD, 0.25, 10, 25),
            new TreasureData(Objects.STONE, 0.25, 10, 25),
            new TreasureData(Objects.MIXED_SEEDS, 0.5, 3, 5, maxLevel: 1),

            new TreasureData(Objects.TREASURE_CHEST, 0.005, allowDuplicates: false),
            new TreasureData(Objects.PRISMATIC_SHARD, 0.00025, allowDuplicates: false),

            // Gems
            new TreasureData(Objects.DIAMOND, 0.01, allowDuplicates: false),
            new TreasureData(Objects.FIRE_QUARTZ, 0.025, 1, 3, 4),
            new TreasureData(Objects.EMERALD, 0.025, 1, 3, 4),
            new TreasureData(Objects.RUBY, 0.025, 1, 3, 4),
            new TreasureData(Objects.FROZEN_TEAR, 0.025, 1, 3, 3),
            new TreasureData(Objects.JADE, 0.025, 1, 3, 3),
            new TreasureData(Objects.AQUAMARINE, 0.025, 1, 3, 3),
            new TreasureData(Objects.EARTH_CRYSTAL, 0.025, 1, 3),
            new TreasureData(Objects.AMETHYST, 0.025, 1, 3),
            new TreasureData(Objects.TOPAZ, 0.025, 1, 3),

            // Weapons
            new TreasureData(Objects.NEPTUNE_GLAIVE, 0.001, minDistance: 5, meleeWeapon: true, allowDuplicates: false),
            new TreasureData(Objects.BROKEN_TRIDENT, 0.001, minDistance: 5, meleeWeapon: true, allowDuplicates: false),

            // Equippables
            new TreasureData(Objects.IRIDIUM_BAND, 0.0025, allowDuplicates: false),
            new TreasureData(504, 0.005, idRange: 10, allowDuplicates: false), // Boots
            new TreasureData(516, 0.005, idRange: 4, allowDuplicates: false), // Rings
            new TreasureData(529, 0.005, idRange: 6, allowDuplicates: false) // Rings, part 2
        };

        public class TreasureData : IWeighted {
            public int id { get; set; }
            public double chance { get; set; }
            public int minAmount { get; set; }
            public int maxAmount { get; set; }
            public int minCastDistance { get; set; }
            public int minLevel { get; set; }
            /**<remarks>This is ignored when level >= 10</remarks>**/
            public int maxLevel { get; set; }
            public int idRange { get; set; }
            public bool meleeWeapon { get; set; }
            public bool allowDuplicates { get; set; }

            public TreasureData(int id, double chance, int minAmount = 1, int maxAmount = 1, int minDistance = 0, int minLevel = 0, int maxLevel = 10, int idRange = 1, bool meleeWeapon = false, bool allowDuplicates = true) {
                this.id = id;
                this.chance = chance;
                this.minAmount = Math.Max(1, minAmount);
                this.maxAmount = Math.Max(this.minAmount, maxAmount);
                this.minCastDistance = Math.Min(5, Math.Max(0, minDistance));
                this.minLevel = minLevel;
                this.maxLevel = Math.Max(minLevel, maxLevel);
                this.idRange = Math.Max(1, idRange);
                this.meleeWeapon = meleeWeapon;
                this.allowDuplicates = allowDuplicates;
            }

            public bool IsValid(int level, int distance) => level >= this.minLevel && (this.maxLevel >= 10 || level <= this.maxLevel) && distance >= this.minCastDistance;

            public double GetWeight() => this.chance;
        }
    }
}
