/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using BetterPanning.Data;
using SObject = StardewValley.Object;
using System.Collections.Generic;

namespace BetterPanning.Config
{
    public static class TreasureGroupDefaultConfig
    {
        public static Dictionary<TREASURE_GROUP, TreasureGroup> CreateTreasureGroup(string file)
        {
            Dictionary<TREASURE_GROUP, TreasureGroup> groups = new Dictionary<TREASURE_GROUP, TreasureGroup>();

            groups.Add(TREASURE_GROUP.Rares, CreateGroup(TREASURE_GROUP.Rares, 0.005));
            groups.Add(TREASURE_GROUP.Geodes, CreateGroup(TREASURE_GROUP.Geodes, 0.075));
            groups.Add(TREASURE_GROUP.GeodeMinerals, CreateGroup(TREASURE_GROUP.GeodeMinerals, 0.02));            
            groups.Add(TREASURE_GROUP.Boots, CreateGroup(TREASURE_GROUP.Boots,  0.01));
            groups.Add(TREASURE_GROUP.Rings, CreateGroup(TREASURE_GROUP.Rings, 0.01));
            groups.Add(TREASURE_GROUP.Minerals, CreateGroup(TREASURE_GROUP.Minerals, 0.15));
            groups.Add(TREASURE_GROUP.Artifacts, CreateGroup(TREASURE_GROUP.Artifacts, 0.01));
            groups.Add(TREASURE_GROUP.Gems, CreateGroup(TREASURE_GROUP.Gems, 0.06));
            groups.Add(TREASURE_GROUP.Ores, CreateGroup(TREASURE_GROUP.Ores, 0.40));
            groups.Add(TREASURE_GROUP.Bars, CreateGroup(TREASURE_GROUP.Bars, 0.04));
            groups.Add(TREASURE_GROUP.Tackle, CreateGroup(TREASURE_GROUP.Tackle, 0.02));
            groups.Add(TREASURE_GROUP.Totems, CreateGroup(TREASURE_GROUP.Totems, 0.01));
            groups.Add(TREASURE_GROUP.SpringSeeds, CreateGroup(TREASURE_GROUP.SpringSeeds, 0.0125));            
            groups.Add(TREASURE_GROUP.SummerSeeds, CreateGroup(TREASURE_GROUP.SummerSeeds, 0.0125));            
            groups.Add(TREASURE_GROUP.FallSeeds, CreateGroup(TREASURE_GROUP.FallSeeds, 0.0125));            
            groups.Add(TREASURE_GROUP.WinterSeeds, CreateGroup(TREASURE_GROUP.WinterSeeds, 0.0125));            
            groups.Add(TREASURE_GROUP.Other, CreateGroup(TREASURE_GROUP.Other, 0.14));
            groups.Add(TREASURE_GROUP.Custom, CreateGroup(TREASURE_GROUP.Custom, 0.00, false));

            PanningMod.Instance.Helper.Data.WriteJsonFile(file, groups);

            return groups;
        }

        private static TreasureGroup CreateGroup(TREASURE_GROUP id, double chance, bool enabled = true, bool manual = false)
        {
           TreasureGroup newGroup = new TreasureGroup(id, chance, enabled, manual);
            newGroup.treasureList = GetTreasureList(id);
            return newGroup;
        }

        private static List<TreasureData> GetTreasureList(TREASURE_GROUP id)
        {
            switch (id)
            {
                case TREASURE_GROUP.Rares:
                    return GetRareTreasureList();    
                    
                case TREASURE_GROUP.Geodes:
                    return GetGeodeTreasureList();      
                    
                case TREASURE_GROUP.GeodeMinerals:
                    return GetGeodeMineralsTreasureList(); 
                    
                case TREASURE_GROUP.Boots:
                    return GetBootsTreasureList();    
                    
                case TREASURE_GROUP.Rings:
                    return GetRingsTreasureList();   
                    
                case TREASURE_GROUP.Minerals:
                    return GetMinerialTreasureList();    
                    
                case TREASURE_GROUP.Artifacts:
                    return GetArtifactTreasureList();     
                    
                case TREASURE_GROUP.Gems:
                    return GetGemsTreasureList();

                case TREASURE_GROUP.Ores:
                    return GetOreTreasureList();

                case TREASURE_GROUP.Bars:
                    return GetBarTreasureList();

                case TREASURE_GROUP.Tackle:
                    return GetTackleTreasureList();

                case TREASURE_GROUP.Totems:
                    return GetTotemTreasureList();

                case TREASURE_GROUP.SpringSeeds:
                    return GetSpringSeedTreasureList();

                case TREASURE_GROUP.SummerSeeds:
                    return GetSummerSeedTreasureList();

                case TREASURE_GROUP.FallSeeds:
                    return GetFallSeedTreasureList();

                case TREASURE_GROUP.WinterSeeds:
                    return GetWinterSeedTreasureList();

                case TREASURE_GROUP.Other:
                    return GetOtherTreasureList();   
                    
                case TREASURE_GROUP.Custom:
                    return GetCustomTreasureList();  
                    
                default:
                    return new List<TreasureData>();                    
            }
        }

        private static List<TreasureData> GetRareTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(SObject.prismaticShardIndex, "Prismatic Shard", 0.10));
            treasures.Add(CreateTreasure(166, "Treasure Chest", 0.30));
            treasures.Add(CreateTreasure(347, "Rare Seed", 0.25, 1, 2, false));
            treasures.Add(CreateTreasure(787, "Battery Pack", 0.20, 1, 2, false));
            treasures.Add(CreateTreasure(797, "Pearl", 0.15, 1, 2, false));

            return treasures;
        }

        private static List<TreasureData> GetGeodeTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(535, "Geode", 0.50, 1, 5, true));
            treasures.Add(CreateTreasure(536, "Frozen Geode", 0.225, 1, 5, true));
            treasures.Add(CreateTreasure(537, "Magma Geode", 0.225, 1, 5, true));
            treasures.Add(CreateTreasure(749, "Omni Geode", 0.05, 1, 5, true));

            return treasures;
        }

        private static List<TreasureData> GetGeodeMineralsTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(538, "Alamite",  0.016721));
            treasures.Add(CreateTreasure(539, "Bixite",  0.008361));
            treasures.Add(CreateTreasure(540, "Baryte",  0.050164));
            treasures.Add(CreateTreasure(541, "Aerinite",  0.020066));
            treasures.Add(CreateTreasure(542, "Calcite",  0.033443));
            treasures.Add(CreateTreasure(543, "Dolomite",  0.008361));
            treasures.Add(CreateTreasure(544, "Esperite",  0.025082));
            treasures.Add(CreateTreasure(545, "Fluorapatite",  0.012541));
            treasures.Add(CreateTreasure(546, "Geminite",  0.016721));
            treasures.Add(CreateTreasure(547, "Helvite",  0.005574));
            treasures.Add(CreateTreasure(548, "Jamborite",  0.016721));
            treasures.Add(CreateTreasure(549, "Jagoite",  0.02181));
            treasures.Add(CreateTreasure(550, "Kyanite",  0.010033));
            treasures.Add(CreateTreasure(551, "Lunarite",  0.012541));
            treasures.Add(CreateTreasure(552, "Malachite",  0.025082));
            treasures.Add(CreateTreasure(553, "Neptunite",  0.00627));
            treasures.Add(CreateTreasure(554, "Lemon Stone",  0.012541));
            treasures.Add(CreateTreasure(555, "Nekoite",  0.031352));
            treasures.Add(CreateTreasure(556, "Orpiment",  0.031352));
            treasures.Add(CreateTreasure(557, "Petrified Slime",  0.020902));
            treasures.Add(CreateTreasure(558, "Thunder Egg",  0.025082));
            treasures.Add(CreateTreasure(559, "Pyrite",  0.020902));
            treasures.Add(CreateTreasure(560, "Ocean Stone",  0.011401));
            treasures.Add(CreateTreasure(561, "Ghost Crystal",  0.012541));
            treasures.Add(CreateTreasure(562, "Tigerseye",  0.009121));
            treasures.Add(CreateTreasure(563, "Jasper",  0.016721));
            treasures.Add(CreateTreasure(564, "Opal",  0.016721));
            treasures.Add(CreateTreasure(565, "Fire Opal",  0.007166));
            treasures.Add(CreateTreasure(566, "Celestine",  0.020066));
            treasures.Add(CreateTreasure(567, "Marble",  0.022802));
            treasures.Add(CreateTreasure(568, "Sandstone",  0.041803));
            treasures.Add(CreateTreasure(569, "Granite",  0.033443));
            treasures.Add(CreateTreasure(570, "Basalt",  0.014333));
            treasures.Add(CreateTreasure(571, "Limestone",  0.167213));
            treasures.Add(CreateTreasure(572, "Soapstone",  0.020902));
            treasures.Add(CreateTreasure(573, "Hematite",  0.016721));
            treasures.Add(CreateTreasure(574, "Mudstone",  0.100328));
            treasures.Add(CreateTreasure(575, "Obsidian",  0.012541));
            treasures.Add(CreateTreasure(576, "Slate",  0.029508));
            treasures.Add(CreateTreasure(577, "Fairy Stone",  0.010033));
            treasures.Add(CreateTreasure(578, "Star Shards",  0.005016));

            return treasures;
        }

        private static List<TreasureData> GetBootsTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(504, "Sneakers",  0.20));
            treasures.Add(CreateTreasure(505, "Rubber Boots",  0.20));
            treasures.Add(CreateTreasure(506, "Leather Boots",  0.20));
            treasures.Add(CreateTreasure(507, "Work Boots",  0.20));
            treasures.Add(CreateTreasure(508, "Combat Boots",  0.05));
            treasures.Add(CreateTreasure(509, "Tundra Boots",  0.05));
            treasures.Add(CreateTreasure(510, "Thermal Boots",  0.05));
            treasures.Add(CreateTreasure(511, "Dark Boots",  0.01));
            treasures.Add(CreateTreasure(512, "Firewalker Boots",  0.01));
            treasures.Add(CreateTreasure(513, "Genie Shoes",  0.01));
            treasures.Add(CreateTreasure(514, "Space Boots",  0.01));
            treasures.Add(CreateTreasure(515, "Cowboy Boots",  0.01));

            return treasures;
        }

        private static List<TreasureData> GetRingsTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(516, "Small Glow Ring", 0.35));
            treasures.Add(CreateTreasure(517, "Glow Ring", 0.1025));
            treasures.Add(CreateTreasure(518, "Small Magnet Ring", 0.35));
            treasures.Add(CreateTreasure(519, "Magnet Ring", 0.1025));
            treasures.Add(CreateTreasure(520, "Slime Charmer Ring", 0.0025));
            treasures.Add(CreateTreasure(521, "Warrior Ring", 0.005));
            treasures.Add(CreateTreasure(522, "Vampire Ring",  0.005));
            treasures.Add(CreateTreasure(523, "Savage Ring",  0.005));
            treasures.Add(CreateTreasure(524, "Ring of Yoba",  0.005));
            treasures.Add(CreateTreasure(525, "Sturdy Ring",  0.005));
            treasures.Add(CreateTreasure(526, "Burglar's Ring",  0.005));
            treasures.Add(CreateTreasure(527, "Iridium Band",  0.0025));
            treasures.Add(CreateTreasure(529, "Amethyst Ring",  0.01));
            treasures.Add(CreateTreasure(530, "Topaz Ring",  0.01));
            treasures.Add(CreateTreasure(531, "Aquamarine Ring",  0.01));
            treasures.Add(CreateTreasure(532, "Jade Ring",  0.01));
            treasures.Add(CreateTreasure(533, "Emerald Ring",  0.01));
            treasures.Add(CreateTreasure(534, "Ruby Ring",  0.01));
            treasures.Add(CreateTreasure(859, "Lucky Ring", 0.002));
            return treasures;
        }

        private static List<TreasureData> GetMinerialTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(SObject.quartzIndex, "Quartz", 0.48, 1, 3, true));
            treasures.Add(CreateTreasure(82, "Fire Quartz", 0.24, 1, 3, true));
            treasures.Add(CreateTreasure(84, "Frozen Tear", 0.16, 1, 3, true));
            treasures.Add(CreateTreasure(86, "Earth Crystal", 0.12, 1, 3, true));

            return treasures;
        }

        private static List<TreasureData> GetArtifactTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(96, "Dwarf Scroll I", 0.01));
            treasures.Add(CreateTreasure(97, "Dwarf Scroll II", 0.0075));
            treasures.Add(CreateTreasure(98, "Dwarf Scroll III",  0.005));
            treasures.Add(CreateTreasure(99, "Dwarf Scroll IV",  0.005));
            treasures.Add(CreateTreasure(100, "Chipped Amphora",  0.01));
            treasures.Add(CreateTreasure(101, "Arrowhead",  0.01));
            treasures.Add(CreateTreasure(102, "Lost Book",  0.005));
            treasures.Add(CreateTreasure(103, "Ancient Doll", 0.01));
            treasures.Add(CreateTreasure(104, "Elvish Jewelry", 0.01));
            treasures.Add(CreateTreasure(105, "Chewing Stick", 0.075));
            treasures.Add(CreateTreasure(106, "Ornamental Fan", 0.075));
            treasures.Add(CreateTreasure(107, "Dinosaur Egg", 0.01));
            treasures.Add(CreateTreasure(108, "Rare Disc", 0.01));
            treasures.Add(CreateTreasure(109, "Ancient Sword",  0.01));
            treasures.Add(CreateTreasure(110, "Rusty Spoon",  0.12));
            treasures.Add(CreateTreasure(111, "Rusty Spur",  0.11));
            treasures.Add(CreateTreasure(112, "Rusty Cog",  0.11));
            treasures.Add(CreateTreasure(113, "Chicken Statue",  0.0975));
            treasures.Add(CreateTreasure(114, "Ancient Seed",  0.05));
            treasures.Add(CreateTreasure(115, "Prehistoric Tool",  0.005));
            treasures.Add(CreateTreasure(116, "Dried Starfish",  0.01));
            treasures.Add(CreateTreasure(117, "Anchor",  0.05));
            treasures.Add(CreateTreasure(118, "Glass Shards",  0.075));
            treasures.Add(CreateTreasure(119, "Bone Flute",  0.01));
            treasures.Add(CreateTreasure(120, "Prehistoric Handaxe",  0.01));
            treasures.Add(CreateTreasure(121, "Dwarvish Helm",  0.005));
            treasures.Add(CreateTreasure(122, "Dwarf Gadget",  0.005));
            treasures.Add(CreateTreasure(123, "Ancient Drum",  0.01));
            treasures.Add(CreateTreasure(124, "Golden Mask",  0.005));
            treasures.Add(CreateTreasure(125, "Golden Relic",  0.005));
            treasures.Add(CreateTreasure(126, "Strange Doll",  0.0075));
            treasures.Add(CreateTreasure(127, "Strange Doll",  0.0075));
            treasures.Add(CreateTreasure(579, "Prehistoric Scapula",  0.005));
            treasures.Add(CreateTreasure(580, "Prehistoric Tibia",  0.005));
            treasures.Add(CreateTreasure(581, "Prehistoric Skull",  0.005));
            treasures.Add(CreateTreasure(582, "Skeletal Hand",  0.005));
            treasures.Add(CreateTreasure(583, "Prehistoric Rib",  0.005));
            treasures.Add(CreateTreasure(584, "Prehistoric Vertebra",  0.005));
            treasures.Add(CreateTreasure(585, "Skeletal Tail",  0.005));
            treasures.Add(CreateTreasure(586, "Nautilus Fossi",  0.005));
            treasures.Add(CreateTreasure(587, "Amphibian Fossil",  0.005));
            treasures.Add(CreateTreasure(588, "Palm Fossil",  0.005));
            treasures.Add(CreateTreasure(589, "Trilobite",  0.005));

            return treasures;
        }

        private static List<TreasureData> GetGemsTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(SObject.emeraldIndex, "Emerald", 0.094364, 1, 2, true));
            treasures.Add(CreateTreasure(SObject.aquamarineIndex, "Aquamarine", 0.131062, 1, 3, true));
            treasures.Add(CreateTreasure(SObject.rubyIndex, "Ruby", 0.094364, 1, 2, true));
            treasures.Add(CreateTreasure(SObject.amethystClusterIndex, "Amethyst", 0.235911, 1, 4, true));
            treasures.Add(CreateTreasure(SObject.topazIndex, "Topaz", 0.294889, 1, 5, true));
            treasures.Add(CreateTreasure(SObject.sapphireIndex, "Jade", 0.117955, 1, 2, true));
            treasures.Add(CreateTreasure(SObject.diamondIndex, "Diamond", 0.031455));

            return treasures;
        }

        private static List<TreasureData> GetOreTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(SObject.copper, "Copper Ore", 0.49, 5, 15, true));
            treasures.Add(CreateTreasure(SObject.iron, "Iron Ore", 0.34, 3, 13, true));
            treasures.Add(CreateTreasure(SObject.gold, "Gold Ore", 0.14, 2, 10, true));
            treasures.Add(CreateTreasure(SObject.iridium, "Iriduim Ore", 0.03, 1, 6, true));

            return treasures;
        }

        private static List<TreasureData> GetBarTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(SObject.copperBar, "Copper Bar", 0.49));
            treasures.Add(CreateTreasure(SObject.ironBar, "Iron Bar", 0.34));
            treasures.Add(CreateTreasure(SObject.goldBar, "Gold Bar", 0.14));
            treasures.Add(CreateTreasure(SObject.iridiumBar, "Iridium Bar", 0.03));
            return treasures;
        }

        private static List<TreasureData> GetTackleTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(686, "Spinner", 0.136));
            treasures.Add(CreateTreasure(687, "Dressed Spinner", 0.068));
            treasures.Add(CreateTreasure(691, "Barbed Hook", 0.068));
            treasures.Add(CreateTreasure(692, "Lead Bobber", 0.341));
            treasures.Add(CreateTreasure(693, "Treasure Hunter", 0.091));
            treasures.Add(CreateTreasure(694, "Trap Bobber", 0.136));
            treasures.Add(CreateTreasure(695, "Cork Bobber", 0.091));
            treasures.Add(CreateTreasure(703, "Magnet", 0.068));

            return treasures;
        }

        private static List<TreasureData> GetTotemTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(681, "Rain Totem", 0.25));
            treasures.Add(CreateTreasure(688, "Warp Totem: Farm", 0.25));
            treasures.Add(CreateTreasure(689, "Warp Totem: Mountains", 0.25));
            treasures.Add(CreateTreasure(690, "Warp Totem: Beach", 0.25));
            
            return treasures;
        }

        private static List<TreasureData> GetSpringSeedTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(427, "Tulip Bulb", 0.209, 15, 30, true));
            treasures.Add(CreateTreasure(429, "Jazz Seeds", 0.139, 15, 30, true));
            treasures.Add(CreateTreasure(472, "Parsnip Seeds", 0.209, 15, 30, true));
            treasures.Add(CreateTreasure(474, "Cauliflower Seeds", 0.052, 5, 20, true));
            treasures.Add(CreateTreasure(475, "Potato Seeds", 0.084, 10, 25, true));
            treasures.Add(CreateTreasure(476, "Garlic Seeds", 0.104, 10, 25, true));
            treasures.Add(CreateTreasure(477, "Kale Seeds", 0.06, 5, 20, true));
            treasures.Add(CreateTreasure(478, "Rhubarb Seeds", 0.042, 1, 15, true));
            treasures.Add(CreateTreasure(495, "Spring Seeds", 0.06, 5, 20, true));
            treasures.Add(CreateTreasure(745, "Strawberry Seeds", 0.042, 5, 15, true));

            return treasures;
        }

        private static List<TreasureData> GetSummerSeedTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(431, "Sunflower Seeds", 0.019, 5, 10, true));
            treasures.Add(CreateTreasure(453, "Poppy Seeds", 0.039, 5, 15, true));
            treasures.Add(CreateTreasure(455, "Spangle Seeds", 0.077, 10, 25, true));
            treasures.Add(CreateTreasure(479, "Melon Seeds", 0.048, 5, 20, true));
            treasures.Add(CreateTreasure(480, "Tomato Seeds", 0.077, 10, 25, true));
            treasures.Add(CreateTreasure(481, "Blueberry Seeds", 0.048, 5, 20, true));
            treasures.Add(CreateTreasure(482, "Pepper Seeds", 0.097, 10, 25, true));
            treasures.Add(CreateTreasure(483, "Wheat Seeds", 0.387, 15, 30, true));
            treasures.Add(CreateTreasure(484, "Radish Seeds", 0.097, 10, 25, true));
            treasures.Add(CreateTreasure(485, "Red Cabbage Seeds", 0.039, 5, 15, true));
            treasures.Add(CreateTreasure(486, "Starfruit Seeds", 0.01, 1, 7, true));
            treasures.Add(CreateTreasure(487, "Corn Seeds", 0.026, 5, 10, true));
            treasures.Add(CreateTreasure(496, "Summer Seeds", 0.035, 5, 20, true));
            
            return treasures;
        }

        private static List<TreasureData> GetFallSeedTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(299, "Amaranth Seeds", 0.067, 5, 20, true));
            treasures.Add(CreateTreasure(425, "Fairy Seeds", 0.023, 5, 10, true));
            treasures.Add(CreateTreasure(488, "Eggplant Seeds", 0.233, 15, 30, true));
            treasures.Add(CreateTreasure(489, "Artichoke Seeds", 0.155, 15, 30, true));
            treasures.Add(CreateTreasure(490, "Pumpkin Seeds", 0.047, 5, 15, true));
            treasures.Add(CreateTreasure(492, "Yam Seeds", 0.078, 10, 25, true));
            treasures.Add(CreateTreasure(491, "Bok Choy Seeds", 0.093, 10, 25, true));
            treasures.Add(CreateTreasure(493, "Cranberry Seeds", 0.019, 5, 10, true));
            treasures.Add(CreateTreasure(494, "Beet Seeds", 0.233, 1, 20, true));           
            treasures.Add(CreateTreasure(497, "Fall Seeds", 0.052, 5, 20, true));
            
            return treasures;
        }

        private static List<TreasureData> GetWinterSeedTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            treasures.Add(CreateTreasure(297, "Grass Starter", 0.10, 1, 5, true));
            treasures.Add(CreateTreasure(498, "Winter Seeds", 0.85, 10, 25, true));
            treasures.Add(CreateTreasure(802, "Cactus Seeds", 0.05, 1, 3, true));            
            return treasures;
        }

        private static List<TreasureData> GetOtherTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();            
            treasures.Add(CreateTreasure(167, "Joja Cola", 0.005));
            treasures.Add(CreateTreasure(168, "Trash", 0.005));
            treasures.Add(CreateTreasure(169, "Driftwood", 0.005));
            treasures.Add(CreateTreasure(170, "Broken Glasses", 0.005));
            treasures.Add(CreateTreasure(171, "Broken CD", 0.005));
            treasures.Add(CreateTreasure(172, "Soggy Newspaper", 0.005));            
            treasures.Add(CreateTreasure(SObject.coal, "Coal", 0.955, 3, 15, true));
            treasures.Add(CreateTreasure(423, "Rice", 0.015, 1, 3, true));
                        
            return treasures;
        }

        private static List<TreasureData> GetCustomTreasureList()
        {
            List<TreasureData> treasures = new List<TreasureData>();
            return treasures;
        }

        private static TreasureData CreateTreasure(int id, string name, double chance, int minAmount, int maxAmount, bool allowDuplicates)
        {
            return new TreasureData(id, name, chance, minAmount, maxAmount, allowDuplicates, true);
        }

        private static TreasureData CreateTreasure(int id, string name, double chance)
        {
            return new TreasureData(id, name, chance, 1, 1, false, true);
        }
    }
}
