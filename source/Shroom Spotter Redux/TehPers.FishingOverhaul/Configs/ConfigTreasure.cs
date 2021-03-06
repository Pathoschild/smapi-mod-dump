/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using TehPers.Core.Api.Enums;
using TehPers.Core.Json.Serialization;

namespace TehPers.FishingOverhaul.Configs {

    [JsonDescribe]
    public class ConfigTreasure {

        [Description("All the treasure that can be found while fishing")]
        public List<TreasureData> PossibleLoot { get; set; } = new List<TreasureData> {
            new TreasureData(Objects.DressedSpinner, 0.025, 1, 1, 6, allowDuplicates: false),
            new TreasureData(Objects.Bait, 0.25, 2, 4),
            //new TreasureData(Objects.BAIT, 0.05, 10), // No need for this I guess

            // Archaeology
            new TreasureData(Objects.LostBook, 0.025, allowDuplicates: false),
            new TreasureData(585, 0.0625, idRange: 4), // Archaeology, part 1
            new TreasureData(96, 0.125, idRange: 32), // Archaeology, part 2
            //new TreasureData(Objects.STRANGE_DOLL1, 0.0025),
            //new TreasureData(Objects.STRANGE_DOLL2, 0.0025),

            new TreasureData(Objects.Geode, 0.2, 1, 3),
            new TreasureData(Objects.FrozenGeode, 0.125, 1, 3),
            new TreasureData(Objects.MagmaGeode, 0.125, 1, 3),
            new TreasureData(Objects.OmniGeode, 0.0625, 1, 3),

            new TreasureData(Objects.IridiumOre, 0.0075, 1, 3),
            new TreasureData(Objects.GoldOre, 0.15, 3, 10),
            new TreasureData(Objects.IronOre, 0.15, 3, 10),
            new TreasureData(Objects.CopperOre, 0.15, 3, 10),
            new TreasureData(Objects.Coal, 0.3, 3, 10),

            // Junk
            new TreasureData(Objects.Wood, 0.25, 10, 25),
            new TreasureData(Objects.Stone, 0.25, 10, 25),
            new TreasureData(Objects.MixedSeeds, 0.5, 3, 5, maxLevel: 1),

            new TreasureData(Objects.TreasureChest, 0.005, allowDuplicates: false),
            new TreasureData(Objects.PrismaticShard, 0.00025, allowDuplicates: false),

            // Gems
            new TreasureData(Objects.Diamond, 0.01, allowDuplicates: false),
            new TreasureData(Objects.FireQuartz, 0.025, 1, 3),
            new TreasureData(Objects.Emerald, 0.025, 1, 3),
            new TreasureData(Objects.Ruby, 0.025, 1, 3),
            new TreasureData(Objects.FrozenTear, 0.025, 1, 3),
            new TreasureData(Objects.Jade, 0.025, 1, 3),
            new TreasureData(Objects.Aquamarine, 0.025, 1, 3),
            new TreasureData(Objects.EarthCrystal, 0.025, 1, 3),
            new TreasureData(Objects.Amethyst, 0.025, 1, 3),
            new TreasureData(Objects.Topaz, 0.025, 1, 3),

            // Weapons
            new TreasureData(Objects.NeptuneGlaive, 0.001, meleeWeapon: true, allowDuplicates: false),
            new TreasureData(Objects.BrokenTrident, 0.001, meleeWeapon: true, allowDuplicates: false),

            // Equippables
            new TreasureData(Objects.IridiumBand, 0.0025, allowDuplicates: false),
            new TreasureData(504, 0.005, idRange: 10, allowDuplicates: false), // Boots
            new TreasureData(516, 0.005, idRange: 4, allowDuplicates: false), // Rings
            new TreasureData(529, 0.005, idRange: 6, allowDuplicates: false) // Rings, part 2
        };
    }
}
