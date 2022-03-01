/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WizardsLizards/CauldronOfChance
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CauldronOfChance
{
    /// <summary>
    /// Holds all items/recipes/combinations
    /// </summary>
    public class Cauldron : IDisposable
    {
        public const double butterfliesConst = 0.1;
        public const double boomConst = 0.1;
        public const double cauldronLuckConst = 0.1;
        public const double durationConst = 0.5;

        public List<string> fishes { get; } = new List<string>()
        {
            "Pufferfish",
            "Anchovy",
            "Tuna",
            "Sardine",
            "Bream",
            "Largemouth Bass",
            "Smallmouth Bass",
            "Rainbow Trout",
            "Salmon",
            "Walleye",
            "Perch",
            "Carp",
            "Catfish",
            "Pike",
            "Sunfish",
            "Red Mullet",
            "Herring",
            "Eel",
            "Octopus",
            "Red Snapper",
            "Squid",
            "Sea Cucumber",
            "Super Cucumber",
            "Ghostfish",
            "Stonefish",
            "Ice Pip",
            "Lava Eel",
            "Sandfish",
            "Scorpion Carp",
            "Flounder",
            "Midnight Carp",
            "Sturgeon",
            "Tiger Trout",
            "Bullhead",
            "Tilapia",
            "Chub",
            "Dorado",
            "Albacore",
            "Shad",
            "Lingcod",
            "Halibut",
            "Woodskip",
            "Void Salmon",
            "Slimejack",
            "Stingray",
            "Lionfish",
            "Blue Discus",
            "Midnight Squid",
            "Spook Fish",
            "Blobfish",
            "Crimsonfish",
            "Angler",
            "Legend",
            "Glacierfish",
            "Mutant Carp",
            "Son of Crimsonfish",
            "Ms. Angler",
            "Legend II",
            "Glacierfish Jr.",
            "Radioactive Carp",
        };
        public List<string> shells { get; } = new List<string>()
        {
            "Lobster",
            "Clam",
            "Crayfish",
            "Crab",
            "Cockle",
            "Mussel",
            "Shrimp",
            "Snail",
            "Periwinkle",
            "Oyster",
            "Rainbow Shell",
        };
        public List<string> corals { get; } = new List<string>()
        {
            "Nautilus Shell",
            "Coral",
            "Sea Urchin",
        };
        public List<string> bait { get; } = new List<string>()
        {
            "Bait",
            "Magnet",
            "Wild Bait",
            "Magic Bait",
        };
        public List<string> tackles { get; } = new List<string>()
        {
            "Spinner",
            "Dressed Spinner",
            "Trap Bobber",
            "Cork Bobber",
            "Lead Bobber",
            "Treasure Hunter",
            "Barbed Hook",
            "Curiosity Lure",
            "Quality Bobber",
        };
        public List<string> algae { get; } = new List<string>()
        {
            "Seaweed",
            "Green Algae",
            "Whie Algae",
            "Seaweed",
        };
        public List<string> seeds { get; } = new List<string>()
        {
            "Jazz Seeds",
            "Cauliflower Seeds",
            "Coffee Bean",
            "Garlic Seeds",
            "Bean Starter",
            "Kale Seeds",
            "Parsnip Seeds",
            "Potato Seeds",
            "Rhubarb Seeds",
            "Strawberry Seeds",
            "Tulip Bulb",
            "Rice Shoot",
            "Blueberry Seeds",
            "Corn Seeds",
            "Hops Starter",
            "Pepper Seeds",
            "Melon Seeds",
            "Poppy Seeds",
            "Radish Seeds",
            "Red Cabbage Seeds",
            "Starfruit Seeds",
            "Spangle Seeds",
            "Sunflower Seeds",
            "Tomato Seeds",
            "Wheat Seeds",
            "Amaranth Seeds",
            "Artichoke Seeds",
            "Beet Seeds",
            "Bok Choy Seeds",
            "Cranberry Seeds",
            "Eggplant Seeds",
            "Fairy Seeds",
            "Grape Starter",
            "Pumpkin Seeds",
            "Yam Seeds",
            "Ancient Seeds",
            "Cactus Seeds",
            "Mixed Seeds",
            "Pineapple Seeds",
            "Taro Tuber",
            "Rare Seed",
            "Tea Sapling",
            "Spring Seeds Seeds",
            "Summer Seeds Seeds",
            "Fall Seeds Seeds",
            "Winter Seeds Seeds",
        };
        public List<string> fertilizer { get; } = new List<string>()
        {
            "Basic Fertilizer",
            "Quality Fertilizer",
            "Tree Fertilizer",
            "Basic Retaining Soil",
            "Quality Retaining Soil",
            "Speed-Gro",
            "Deluxe Speed-Gro",
            "Deluxe Fertilizer",
            "Deluxe Retaining Soil",
            "Hyper Speed-Gro",
        };
        public List<string> minerals { get; } = new List<string>()
        {
            "Quartz",
            "Earth Crystal",
            "Frozen Tear",
            "Fire Quartz",
            "Tigerseye",
            "Opal",
            "Fire Opal",
            "Alamite",
            "Bixite",
            "Baryte",
            "Aerinite",
            "Calcite",
            "Dolomite",
            "Esperite",
            "Fluorapatite",
            "Geminite",
            "Helvite",
            "Jamborite",
            "Jagoite",
            "Kyanite",
            "Lunarite",
            "Malachite",
            "Neptunite",
            "Lemon Stone",
            "Nekoite",
            "Orpiment",
            "Petrified Slime",
            "Thunder Egg",
            "Pyrite",
            "Ocean Stone",
            "Ghost Crystal",
            "Jasper",
            "Celestine",
            "Marble",
            "Sandstone",
            "Granite",
            "Basalt",
            "Limestone",
            "Soapstone",
            "Hematite",
            "Mudstone",
            "Obsidian",
            "Slate",
            "Fairy Stone",
            "Star Shards",
        };
        public List<string> gems { get; } = new List<string>()
        {
            "Emerald",
            "Aquamarine",
            "Ruby",
            "Amethyst",
            "Topaz",
            "Jade",
            "Diamond",
            "Prismatic Shard",
        };
        public List<string> monsterloot { get; } = new List<string>()
        {
            "Slime",
            "Bug Meat",
            "Bat Wing",
            "Solar Essence",
            "Void Essence",
            "Bone Fragment",
        };
        public List<string> metalbars { get; } = new List<string>()
        {
            "Copper Bar",
            "Iron Bar",
            "Gold Bar",
            "Iridium Bar",
            "Radioactive Bar",
        };
        public List<string> ores { get; } = new List<string>()
        {
            "Copper Ore",
            "Iron Ore",
            "Gold Ore",
            "Iridium Ore",
            "Radioactive Ore",
        };
        public List<string> food { get; } = new List<string>()
        {
            "Fried Egg",
            "Omelet",
            "Salad",
            "Cheese Cauliflower",
            "Baked Fish",
            "Parsnip Soup",
            "Vegetable Medley",
            "Complete Breakfast",
            "Fried Calamari",
            "Strange Bun",
            "Lucky Lunch",
            "Fried Mushroom",
            "Pizza",
            "Bean Hotpot",
            "Glazed Yams",
            "Carp Surprise",
            "Hashbrowns",
            "Pancakes",
            "Salmon Dinner",
            "Fish Taco",
            "Crispy Bass",
            "Pepper Poppers",
            "Bread",
            "Tom Kha Soup",
            "Trout Soup",
            "Chocolate Cake",
            "Pink Cake",
            "Rhubarb Pie",
            "Cookie",
            "Spaghetti",
            "Fried Eel",
            "Spicy Eel",
            "Sashimi",
            "Maki Roll",
            "Tortilla",
            "Red Plate",
            "Eggplant Parmesan",
            "Rice Pudding",
            "Ice Cream",
            "Blueberry Tart",
            "Autumn's Bounty",
            "Pumpkin Soup",
            "Super Meal",
            "Cranberry Sauce",
            "Stuffing",
            "Farmer's Lunch",
            "Survival Burger",
            "Dish O' The Sea",
            "Miner's Treat",
            "Roots Platter",
            "Triple Shot Espresso",
            "Seafoam Pudding",
            "Algae Soup",
            "Pale Broth",
            "Plum Pudding",
            "Artichoke Dip",
            "Stir Fry",
            "Roasted Hazelnuts",
            "Pumpkin Pie",
            "Radish Salad",
            "Fruit Salat",
            "Blackberry Cobbler",
            "Cranberry Candy",
            "Bruschetta",
            "Coleslaw",
            "Fiddlehead Risotto",
            "Poppyseed Muffin",
            "Chowder",
            "Fish Stew",
            "Escargot",
            "Lobster Bisque",
            "Maple Bar",
            "Crab Cakes",
            "Shrimp Cocktail",
            "Ginger Ale",
            "Banana Pudding",
            "Mango Sticky Rice",
            "Poi",
            "Tropical Curry",
            "Squid Ink Ravioli",
        };
        public List<string> bones { get; } = new List<string>()
        {
            "Bone Fragment",
            "Prehistoric Rib",
            "Prehistoric Scapula",
            "Prehistoric Skull",
            "Prehistoric Tibia",
            "Prehistoric Vertebra",
            "Skeletal Hand",
            "Skeletal Tail",
        };
        public List<string> fossils { get; } = new List<string>()
        {
            "Amphibian Fossil",
            "Fossilized Leg",
            "Fossilized Ribs",
            "Fossilized Skull",
            "Fossilized Spine",
            "Fossilized Tail",
            "Mummified Bat",
            "Mummified Frog",
            "Nautilus Fossil",
            "Palm Fossil",
            "Snake Skull",
            "Snake Vertebrae",
            "Trilobite",
        };
        public List<string> foragables { get; } = new List<string>()
        {
            "Sap",
            "Wild Horseradish",
            "Daffodil",
            "Leek",
            "Dandelion",
            "Spring Onion",
            "Morel",
            "Common Mushroom",
            "Salmonberry",
            "Grape",
            "Spice Berry",
            "Sweet Pea",
            "Red Mushroom",
            "Fiddlehead Fern",
            "Wild Plum",
            "Hazelnut",
            "Blackberry",
            "Chanterelle",
            "Purple Mushroom",
            "Winter Root",
            "Crystal Fruit",
            "Snow Yam",
            "Crocus",
            "Holly",
            "Nautilus Shell",
            "Coral",
            "Sea Urchin",
            "Rainbow Shell",
            "Clam",
            "Cockle",
            "Mussel",
            "Oyster",
            "Seaweed",
            "Cave Carrot",
            "Cactus Fruit",
            "Coconut",
            "Ginger",
            "Magma Cap",
        };
        public List<string> bombs { get; } = new List<string>()
        {
            "Explosive Ammo",
            "Cherry Bomb",
            "Bomb",
            "Mega Bomb",
        };
        public List<string> trash { get; } = new List<string>()
        {
            "Trash",
            "Driftwood",
            "Soggy Newspaper",
            "Broken CD",
            "Broken Glasses",
            "Joja Cola",
            "Rotten Plant",
        };
        public List<string> artifacts { get; } = new List<string>()
        {
            "Dwarf Scroll I",
            "Dwarf Scroll II",
            "Dwarf Scroll III",
            "Dwarf Scroll IV",
            "Chipped Amphora",
            "Arrowhead",
            "Ancient Doll",
            "Elvish Jewelry",
            "Chewing Stick",
            "Ornamental Fan",
            "Dinosaur Egg",
            "Rare Disc",
            "Ancient Sword",
            "Rusty Spoon",
            "Rusty Spur",
            "Rusty Cog",
            "Chicken Statue",
            "Ancient Seed",
            "Prehistoric Tool",
            "Dried Starfish",
            "Anchor",
            "Glass Shards",
            "Bone Flute",
            "Prehistoric Handaxe",
            "Dwarvish Helm",
            "Dwarf Gadget",
            "Ancient Drum",
            "Golden Mask",
            "Golden Relic",
            "Strange Doll",
            "Strange Doll",
        };
        public List<string> crops { get; } = new List<string>()
        {
            "Jazz",
            "Cauliflower",
            "Coffee Bean",
            "Garlic",
            "Green Bean",
            "Kale",
            "Parsnip",
            "Potato",
            "Rhubarb",
            "Strawberry",
            "Tulip",
            "Unmilled Rice",
            "Blueberry",
            "Corn",
            "Hops",
            "Hot Pepper",
            "Melon",
            "Poppy",
            "Radish",
            "Red Cabbage",
            "Starfruit",
            "Summer Spangle",
            "Sunflower",
            "Tomato",
            "Wheat",
            "Amaranth",
            "Artichoke",
            "Beet",
            "Bok Choy",
            "Cranberries",
            "Eggplant",
            "Fairy Rose",
            "Grape",
            "Pumpkin",
            "Yam",
            "Ancient Fruit",
            "Cactus Fruit",
            "Pineapple",
            "Taro Root",
            "Sweet Gem Berry",
            "Tea Leaves",
        };
        public List<string> artisanGoods { get; } = new List<string>()
        {
            "Honey",
            "Wine",
            "Pale Ale",
            "Beer",
            "Mead",
            "Cheese",
            "Goat Cheese",
            "Coffee",
            "Green Tea",
            "Juice",
            "Cloth",
            "Mayonnaise",
            "Duck Mayonnaise",
            "Void Mayonnaise",
            "Dinosaur Mayonnaise",
            "Truffle Oil",
            "Oil",
            "Pickles",
            "Jelly",
            "Caviar",
            "Aged Roe",
        };

        public enum buffs
        {
            garlicOil1,
            garlicOil2,
            garlicOil3,

            debuffImmunity1,
            debuffImmunity2,
            debuffImmunity3,

            farmingBuff1,
            farmingBuff2,
            farmingBuff3,

            miningBuff1,
            miningBuff2,
            miningBuff3,

            fishingBuff1,
            fishingBuff2,
            fishingBuff3,

            foragingBuff1,
            foragingBuff2,
            foragingBuff3,

            attackBuff1,
            attackBuff2,
            attackBuff3,

            defenseBuff1,
            defenseBuff2,
            defenseBuff3,

            maxEnergyBuff1,
            maxEnergyBuff2,
            maxEnergyBuff3,

            luckBuff1,
            luckBuff2,
            luckBuff3,

            magneticRadiusBuff1,
            magneticRadiusBuff2,
            magneticRadiusBuff3,

            speedBuff1,
            speedBuff2,
            speedBuff3
        }
        public enum debuffs
        {
            monsterMusk1,
            monsterMusk2,
            monsterMusk3,

            farmingDebuff1,
            farmingDebuff2,
            farmingDebuff3,

            miningDebuff1,
            miningDebuff2,
            miningDebuff3,

            fishingDebuff1,
            fishingDebuff2,
            fishingDebuff3,

            foragingDebuff1,
            foragingDebuff2,
            foragingDebuff3,

            attackDebuff1,
            attackDebuff2,
            attackDebuff3,

            defenseDebuff1,
            defenseDebuff2,
            defenseDebuff3,

            maxEnergyDebuff1,
            maxEnergyDebuff2,
            maxEnergyDebuff3,

            luckDebuff1,
            luckDebuff2,
            luckDebuff3,

            speedDebuff1,
            speedDebuff2,
            speedDebuff3
        }

        //List 1: Holds all combinations. Tuple: Type - Buff to add on to, Match2 - Value for 2 matches, Match3 - Value for 3 matches, Items - List of possible match items (Distinct)
        public List<(string Type, int Match2, int Match3, List<string> Items)> buffCombinations { get; set; }

        //List 1: Holds all the combinations. Tuple: Result - Resulting item, Items - 3 Items needed to create this recipe (at full chance)(Not Distinct)
        public List<(string Result, List<string> Items)> recipes { get; set; }

        //List 1: Holds all item combinatins. Tuple: Result - Resulting item(s), Items - List of possible match items (Distinct)
        public List<(string Result, List<string> Items)> itemCombinations { get; set; }
        public List<(List<string> Result, List<string> Items)> multipleItemCombinations { get; set; }

        //List 1: Holds all cooking recipes. Tuple: Result - Resulting item, Items - List of possible match items, Categories - List of possible match categories
        public List<(string Result, List<int> Items, List<int> Categories)> cookingRecipes { get; set; }

        object Caller { get; set; }

        public Cauldron(object caller)
        {
            this.Caller = caller;

            initializeBuffCombinations();
            initializeCookingRecipes();
            initializeItemCombinations();
            initializeRecipes();
        }

        public void addToCauldron(string name, int value)
        {
            if (value == 0)
            {
                return;
            }

            if (name.Equals("garlicOil") || name.Equals("monsterMusk"))
            {
                addToCauldron("garlicOil", "monsterMusk", value);
            }
            else if (name.Equals("debuffImmunity"))
            {
                addToCauldron(name, "", value);
            }
            else if (name.Equals("magneticRadius"))
            {
                addToCauldron(name + "Buff", "", value);
            }
            else
            {
                addToCauldron(name + "Buff", name + "Debuff", value);
            }
        }

        public void addToCauldron(string buff, string debuff, int value)
        {
            int buffIndex1 = -1;
            int buffIndex2 = -1;
            int buffIndex3 = -1;
            int debuffIndex1 = -1;
            int debuffIndex2 = -1;
            int debuffIndex3 = -1;

            if (buff != null && buff.Equals("") == false)
            {
                buffIndex1 = (int)Enum.Parse(typeof(buffs), buff + 1);
                buffIndex2 = (int)Enum.Parse(typeof(buffs), buff + 2);
                buffIndex3 = (int)Enum.Parse(typeof(buffs), buff + 3);
            }
            if (debuff != null && debuff.Equals("") == false)
            {
                debuffIndex1 = (int)Enum.Parse(typeof(debuffs), debuff + 1);
                debuffIndex2 = (int)Enum.Parse(typeof(debuffs), debuff + 2);
                debuffIndex3 = (int)Enum.Parse(typeof(debuffs), debuff + 3);
            }

            List<int> BuffList = new List<int>();
            List<int> DebuffList = new List<int>();

            if (Caller is CauldronMagic)
            {
                CauldronMagic cauldronMagic = Caller as CauldronMagic;
                BuffList = cauldronMagic.buffList;
                DebuffList = cauldronMagic.debuffList;
            }
            else if (Caller is Ingredient)
            {
                Ingredient ingredient = Caller as Ingredient;
                BuffList = ingredient.buffList;
                DebuffList = ingredient.debuffList;
            }

            if (value >= 3)
            {
                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex3] += 3;
                    BuffList[buffIndex2] += 2;
                    BuffList[buffIndex1] += 1;
                }

                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex2] += 1;
                    DebuffList[debuffIndex1] += 1;
                }
            }
            else if (value == 2)
            {
                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex2] += 3;
                    BuffList[buffIndex1] += 2;
                }

                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex1] += 1;
                }
            }
            else if (value == 1)
            {
                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex1] += 3;
                }

                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex1] += 1;
                }
            }
            else if (value == -1)
            {
                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex1] += 3;
                }

                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex1] += 1;
                }
            }
            else if (value == -2)
            {
                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex2] += 3;
                    DebuffList[debuffIndex1] += 2;
                }

                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex1] += 1;
                }
            }
            else if (value <= -3)
            {
                if (debuff != null && debuff.Equals("") == false)
                {
                    DebuffList[debuffIndex3] += 3;
                    DebuffList[debuffIndex2] += 2;
                    DebuffList[debuffIndex1] += 1;
                }

                if (buff != null && buff.Equals("") == false)
                {
                    BuffList[buffIndex2] += 1;
                    BuffList[buffIndex1] += 1;
                }
            }
        }

        public void initializeCookingRecipes()
        {
            cookingRecipes = new List<(string Result, List<int> Items, List<int> Categories)>();

            Dictionary<string, string> cookingRecipeDict = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");

            foreach (KeyValuePair<string, string> recipe in cookingRecipeDict)
            {
                try
                {
                    Item csItem = Utility.fuzzyItemSearch(recipe.Key);

                    if(csItem is StardewValley.Object)
                    {
                        StardewValley.Object csObject = csItem as StardewValley.Object;

                        string[] objectDescription = Game1.objectInformation[csObject.ParentSheetIndex].Split('/');

                        string[] whatToBuff = (string[])((objectDescription.Length > 7) ? ((object)objectDescription[7].Split(' ')) : ((object)new string[12]
                        {
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                        "0", "0"
                        }));

                        csObject.ModifyItemBuffs(whatToBuff);

                        if(whatToBuff.Select(x => Int32.Parse(x)).Sum() > 0)
                        {
                            List<int> Items = new List<int>();
                            List<int> Categories = new List<int>();

                            List<string> ingredients = recipe.Value.Split('/')[0].Split(' ').ToList();

                            foreach (string ingredient in ingredients)
                            {
                                int id;

                                if (Int32.TryParse(ingredient, out id))
                                {
                                    if (id >= 0)
                                    {
                                        Items.Add(id);
                                    }
                                    else
                                    {
                                        Categories.Add(id);
                                    }
                                }
                            }

                            cookingRecipes.Add((recipe.Key, Items, Categories));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ObjectPatches.IMonitor.Log($"Could not add the recipe for {recipe.Key}:\n{ex}", LogLevel.Error);
                }
            }
        }

        public void initializeBuffCombinations()
        {
            buffCombinations = new List<(string Type, int Match2, int Match3, List<string> Items)>();

            //buffCombinations.Add(("", , , new List<string>() { }));

            #region category combinations
            List<string> fishing1 = new List<string>() { "Driftwood", };
            fishing1.AddRange(fishes);
            fishing1.AddRange(shells);
            fishing1.AddRange(corals);
            fishing1.AddRange(algae);
            fishing1.AddRange(bait);
            fishing1.AddRange(tackles);
            buffCombinations.Add(("fishing", 1, 2, fishing1));

            List<string> farming1 = new List<string>() { };
            farming1.AddRange(seeds);
            farming1.AddRange(fertilizer);
            buffCombinations.Add(("farming", 1, 2, farming1));

            List<string> attack1 = new List<string>() { };
            attack1.AddRange(minerals);
            attack1.AddRange(gems);
            buffCombinations.Add(("attack", 1, 2, attack1));

            buffCombinations.Add(("monsterMusk", 1, 2, monsterloot));
            buffCombinations.Add(("defense", 1, 2, metalbars));

            List<string> mining1 = new List<string>() { "Coal" };
            mining1.AddRange(ores);
            buffCombinations.Add(("mining", 1, 2, mining1));

            buffCombinations.Add(("maxEnergy", 1, 2, food));

            List<string> defense1 = new List<string>() { };
            defense1.AddRange(bones);
            defense1.AddRange(fossils);
            buffCombinations.Add(("defense", 1, 2, defense1));

            buffCombinations.Add(("foraging", 1, 2, foragables));
            buffCombinations.Add(("boom", 1, 2, bombs));
            buffCombinations.Add(("attack", 1, 2, bombs));

            List<string> boom1 = new List<string>() { "Sap", "Weeds", "Fiber" };
            boom1.AddRange(trash);
            buffCombinations.Add(("boom", 1, 2, boom1));
            buffCombinations.Add(("cauldronLuck", -1, -2, boom1));

            buffCombinations.Add(("luck", 1, 2, artifacts));
            buffCombinations.Add(("farming", 1, 2, crops));
            buffCombinations.Add(("speed", 1, 2, artisanGoods));
            #endregion category combinations

            #region fluff combinations
            buffCombinations.Add(("butterflies", 1, 2, new List<string>() { "Daffodil", "Dandelion", "Sweet Pea", "Crocus", "Tulip", "Blue Jazz", "Summer Spangle", "Poppy", "Sunflower", "Fairy Rose" }));
            buffCombinations.Add(("butterflies", 2, 3, new List<string>() { "Purple Mushroom", "Solar Essence", "Super Cucumber", "Void Essence", "Quartz" }));
            buffCombinations.Add(("butterflies", 2, 3, new List<string>() { "Fire Quartz", "Frozen Tear", "Jade", "Purple Mushroom" }));
            buffCombinations.Add(("cauldronLuck", 2, 3, new List<string>() { "Fire Quartz", "Frozen Tear", "Jade", "Purple Mushroom" }));
            buffCombinations.Add(("luck", 2, 3, new List<string>() { "Fire Quartz", "Frozen Tear", "Jade", "Purple Mushroom" }));
            buffCombinations.Add(("butterflies", 2, 3, new List<string>() { "Golden Pumpkin", "Magic Rock Candy", "Pearl", "Prismatic Shard", "Rabbit's Foot" }));
            #endregion fluff combinations

            #region bundle combinations
            //TODO: Add effects. For easy bundles: 0,1 values. for hard bundles: 2,3. for superhard: 3,3? (e.g. ancient fruit + gem berry)
            buffCombinations.Add(("foraging", 1, 2, new List<string>() { "Wild Horseradish", "Daffodil", "Leek", "Dandelion", "Spring Onion", "Spring Seeds", }));
            buffCombinations.Add(("foraging", 1, 2, new List<string>() { "Grape", "Spice Berry", "Sweet Pea", "Summer Seeds", }));
            buffCombinations.Add(("foraging", 1, 2, new List<string>() { "Common Mushroom", "Wild Plum", "Hazelnut", "Blackberry", "Fall Seeds", }));
            buffCombinations.Add(("foraging", 1, 2, new List<string>() { "Winter Root", "Crystal Fruit", "Snow Yam", "Crocus", "Winter Seeds", }));
            buffCombinations.Add(("foraging", 0, 1, new List<string>() { "Wood", "Stone", "Hardwood", "Sap", }));
            buffCombinations.Add(("foraging", 2, 3, new List<string>() { "Coconut", "Cactus Fruit", "Cave Carrot", "Red Mushroom", "Purple Mushroom", "Maple Syrup", "Oak Resin", "Pine Tar", "Morel", "Autumn's Bounty", }));
            buffCombinations.Add(("foraging", 2, 3, new List<string>() { "Purple Mushroom", "Fiddlehead Fern", "White Algae", "Hops", "Cookout Kit", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Parsnip", "Green Bean", "Cauliflower", "Potato", "Speed-Gro", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Tomato", "Hot Pepper", "Blueberry", "Melon", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Corn", "Eggplant", "Pumpkin", "Yam", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Parsnip", "Melon", "Pumpkin", "Corn", }));
            buffCombinations.Add(("cauldronLuck", 3, 3, new List<string>() { "Ancient Fruit", "Sweet Gem Berry", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Large Milk", "Large Egg", "Large Goat Milk", "Wool", "Duck Egg", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Roe", "Aged Roe", "Squid Ink", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Tulip", "Blue Jazz", "Summer Spangle", "Sunflower", "Fairy Rose", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Truffle Oil", "Cloth", "Goat Cheese", "Cheese", "Honey", "Jelly", "Apple", "Apricot", "Orange", "Peach", "Pomegranate", "Cherry", }));
            buffCombinations.Add(("luck", 1, 2, new List<string>() { "Mead", "Pale Ale", "Wine", "Juice", "Green Tea", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Sunfish", "Catfish", "Shad", "Tiger Trout", "Bait", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Largemouth Bass", "Carp", "Bullhead", "Sturgeon", "Dressed Spinner", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Sardine", "Tuna", "Red Snapper", "Tilapia", "Warp Totem: Beach ", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Walleye", "Bream", "Eel", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Lobster", "Crayfish", "Crab", "Cockle", "Mussel", "Shrimp", "Snail", "Periwinkle", "Oyster", "Clam", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Pufferfish", "Ghostfish", "Sandfish", "Woodskip", "Dish O' The Sea", }));
            buffCombinations.Add(("fishing", 1, 2, new List<string>() { "Largemouth Bass", "Shad", "Tuna", "Walleye", "Dish O' The Sea", }));
            buffCombinations.Add(("fishing", 2, 3, new List<string>() { "Lava Eel", "Scorpion Carp", "Octopus", "Blobfish", "Dish O' The Sea", }));
            buffCombinations.Add(("attack", 1, 2, new List<string>() { "Copper Bar", "Iron Bar", "Gold Bar", }));
            buffCombinations.Add(("cauldronLuck", 1, 2, new List<string>() { "Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz", "Omni Geode", }));
            buffCombinations.Add(("attack", 1, 2, new List<string>() { "Slime", "Bat Wing", "Solar Essence", "Void Essence", "Bone Fragment", }));
            buffCombinations.Add(("cauldronLuck", 1, 2, new List<string>() { "Amethyst", "Aquamarine", "Diamond", "Emerald", "Ruby", "Topaz", "Lucky Lunch", }));
            buffCombinations.Add(("speed", 2, 3, new List<string>() { "Iridium Ore", "Battery Pack", "Refined Quartz", }));
            buffCombinations.Add(("maxEnergy", 2, 3, new List<string>() { "Maple Syrup", "Fiddlehead Fern", "Truffle", "Poppy", "Maki Roll", "Fried Egg", "Pink Cake", }));
            buffCombinations.Add(("speed", 1, 2, new List<string>() { "Red Mushroom", "Sea Urchin", "Sunflower", "Duck Feather", "Aquamarine", "Red Cabbage", "Beet", "Amaranth", "Starfruit", "Cactus Fruit", "Blueberry", "Iridium Bar", }));
            buffCombinations.Add(("butterflies", 1, 2, new List<string>() { "Purple Mushroom", "Nautilus Shell", "Chub", "Frozen Geode", }));
            buffCombinations.Add(("magneticRadius", 1, 2, new List<string>() { "Salmonberry", "Cookie", "Ancient Doll", "Ice Cream", "Battery Pack", }));
            buffCombinations.Add(("foraging", 1, 2, new List<string>() { "Salmonberry", "Blackberry", "Wild Plum", }));
            buffCombinations.Add(("maxEnergy", 1, 2, new List<string>() { "Egg", "Milk", "Wheat Flour", "Complete Breakfast", }));
            buffCombinations.Add(("farming", 1, 2, new List<string>() { "Wheat", "Hay", "Apple", }));
            buffCombinations.Add(("luck", 1, 2, new List<string>() { "Oak Resin", "Wine", "Rabbit's Foot", "Pomegranate", "Gold Bar", }));
            buffCombinations.Add(("luck", 2, 3, new List<string>() { "Wine", "Dinosaur Mayonnaise", "Prismatic Shard", "Ancient Fruit", "Void Salmon", "Caviar", }));
            #endregion bundle combinations
        }

        public void initializeRecipes()
        {
            recipes = new List<(string Result, List<string> Items)>();

            //recipes.Add(("", new List<string>() { }));

            recipes.Add(("Golden Pumpkin", new List<string>() { "Gold Bar", "Gold Bar", "Pumpkin" }));
            recipes.Add(("Golden Mask", new List<string>() { "Gold Bar", "Gold Bar", "Amethyst" }));
            recipes.Add(("Dwarf Scroll I", new List<string>() { "Dwarf Scroll II", "Dwarf Scroll III", "Dwarf Scroll IV" }));
            recipes.Add(("Dwarf Scroll II", new List<string>() { "Dwarf Scroll I", "Dwarf Scroll III", "Dwarf Scroll IV" }));
            recipes.Add(("Dwarf Scroll III", new List<string>() { "Dwarf Scroll I", "Dwarf Scroll I#II", "Dwarf Scroll IV" }));
            recipes.Add(("Dwarf Scroll IV", new List<string>() { "Dwarf Scroll I", "Dwarf Scroll II", "Dwarf Scroll III" }));
        }

        public void initializeItemCombinations()
        {
            itemCombinations = new List<(string Result, List<string> Items)>();
            multipleItemCombinations = new List<(List<string> Result, List<string> Items)>();

            //itemCombinations.Add(("", new List<string>() { }));

            List<string> omniGeode = new List<string>() { };
            omniGeode.AddRange(minerals);
            omniGeode.AddRange(gems);
            omniGeode.Add("Geode");
            omniGeode.Add("Frozen Geode");
            omniGeode.Add("Magma Geode");
            itemCombinations.Add(("Omni Geode", omniGeode));

            //multipleItemCombinations.Add((new List<string>() { }, new List<string>() { }));

            List<string> boneCombination = new List<string>() { };
            boneCombination.AddRange(bones);
            boneCombination.AddRange(fossils);
            multipleItemCombinations.Add((bones, boneCombination));
        }

        public Ingredient getIngredient(Item item)
        {
            Ingredient Item;
            
            switch (item.Name)
            {
                case "Weeds":
                    Item = new Ingredient(item, farming: -1);
                    break;
                case "Stone":
                    Item = new Ingredient(item, defense: 1, speed: -1);
                    break;
                case "Wild Horseradish":
                    Item = new Ingredient(item, foraging: 1);
                    break;
                case "Daffodil":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Leek":
                    Item = new Ingredient(item, foraging: 1, maxEnergy: 1);
                    break;
                case "Dandelion":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Parsnip":
                    Item = new Ingredient(item, farming: 1, defense: 1);
                    break;
                case "Lumber":
                    Item = new Ingredient(item);
                    break;
                case "Emerald":
                    Item = new Ingredient(item, foraging: 2);
                    break;
                case "Aquamarine":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Ruby":
                    Item = new Ingredient(item, attack: 2);
                    break;
                case "Amethyst":
                    Item = new Ingredient(item, debuffImmunity: 2);
                    break;
                case "Topaz":
                    Item = new Ingredient(item, farming: 2);
                    break;
                case "Jade":
                    Item = new Ingredient(item, defense: 2, cauldronLuck: 3);
                    break;
                case "Diamond":
                    Item = new Ingredient(item, luck: 2);
                    break;
                case "Prismatic Shard":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 3);
                    break;
                case "Cave Carrot":
                    Item = new Ingredient(item, mining: 1, maxEnergy: 2);
                    break;
                case "Quartz":
                    Item = new Ingredient(item, mining: 1, butterfliesChance: 3);
                    break;
                case "Fire Quartz":
                    Item = new Ingredient(item, attack: 1, cauldronLuck: 3);
                    break;
                case "Frozen Tear":
                    Item = new Ingredient(item, defense: 1, cauldronLuck: 3);
                    break;
                case "Earth Crystal":
                    Item = new Ingredient(item, mining: 1);
                    break;
                case "Coconut":
                    Item = new Ingredient(item, defense: 1);
                    break;
                case "Cactus Fruit":
                    Item = new Ingredient(item, foraging: 2, speed: 1);
                    break;
                case "Sap":
                    Item = new Ingredient(item, boomChance: 1, maxEnergy: -1);
                    break;
                case "Torch":
                    Item = new Ingredient(item, boomChance: 1, speed: 1);
                    break;
                case "Spirit Torch":
                    Item = new Ingredient(item);
                    break;
                case "Dwarf Scroll I":
                    Item = new Ingredient(item, mining: 1);
                    break;
                case "Dwarf Scroll II":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Dwarf Scroll III":
                    Item = new Ingredient(item, mining: 3);
                    break;
                case "Dwarf Scroll IV":
                    Item = new Ingredient(item, mining: 3, cauldronLuck: 1);
                    break;
                case "Chipped Amphora":
                    Item = new Ingredient(item, defense: -1);
                    break;
                case "Arrowhead":
                    Item = new Ingredient(item, attack: 2);
                    break;
                case "Ancient Doll":
                    Item = new Ingredient(item, boomChance: 3, defense: -1);
                    break;
                case "Elvish Jewelry":
                    Item = new Ingredient(item, cauldronLuck: 2, speed: 2);
                    break;
                case "Chewing Stick":
                    Item = new Ingredient(item, attack: -1);
                    break;
                case "Ornamental Fan":
                    Item = new Ingredient(item, speed: 3);
                    break;
                case "Dinosaur Egg":
                    Item = new Ingredient(item, duration: 2, defense: 1);
                    break;
                case "Rare Disc":
                    Item = new Ingredient(item, boomChance: 3, cauldronLuck: 3);
                    break;
                case "Ancient Sword":
                    Item = new Ingredient(item, attack: 3);
                    break;
                case "Rusty Spoon":
                    Item = new Ingredient(item, defense: -2);
                    break;
                case "Rusty Spur":
                    Item = new Ingredient(item, defense: -2);
                    break;
                case "Rusty Cog":
                    Item = new Ingredient(item, defense: -2);
                    break;
                case "Chicken Statue":
                    Item = new Ingredient(item, farming: 3, speed: 2);
                    break;
                case "Ancient Seed":
                    Item = new Ingredient(item, farming: 3, cauldronLuck: 1);
                    break;
                case "Prehistoric Tool":
                    Item = new Ingredient(item, farming: 1, mining: 1);
                    break;
                case "Dried Starfish":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Anchor":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Glass Shards":
                    Item = new Ingredient(item, defense: -3);
                    break;
                case "Bone Flute":
                    Item = new Ingredient(item, monsterMusk: 2, speed: 2);
                    break;
                case "Prehistoric Handaxe":
                    Item = new Ingredient(item, attack: 2);
                    break;
                case "Dwarvish Helm":
                    Item = new Ingredient(item, defense: 3);
                    break;
                case "Dwarf Gadget":
                    Item = new Ingredient(item, boomChance: 1, magneticRadius: 3, speed: 2);
                    break;
                case "Ancient Drum":
                    Item = new Ingredient(item, monsterMusk: 2, attack: 2);
                    break;
                case "Golden Mask":
                    Item = new Ingredient(item, cauldronLuck: 2, defense: 2, magneticRadius: 1);
                    break;
                case "Golden Relic":
                    Item = new Ingredient(item, cauldronLuck: 1, luck: 2, farming: 1);
                    break;
                case "Strange Doll":
                    Item = new Ingredient(item, boomChance: 3, butterfliesChance: 3);
                    break;
                case "Pufferfish":
                    Item = new Ingredient(item, defense: 3, boomChance: 3, speed: -1);
                    break;
                case "Anchovy":
                    Item = new Ingredient(item);
                    break;
                case "Tuna":
                    Item = new Ingredient(item);
                    break;
                case "Sardine":
                    Item = new Ingredient(item);
                    break;
                case "Bream":
                    Item = new Ingredient(item);
                    break;
                case "Largemouth Bass":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Smallmouth Bass":
                    Item = new Ingredient(item);
                    break;
                case "Rainbow Trout":
                    Item = new Ingredient(item);
                    break;
                case "Salmon":
                    Item = new Ingredient(item);
                    break;
                case "Walleye":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Perch":
                    Item = new Ingredient(item);
                    break;
                case "Carp":
                    Item = new Ingredient(item);
                    break;
                case "Catfish":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Pike":
                    Item = new Ingredient(item);
                    break;
                case "Sunfish":
                    Item = new Ingredient(item);
                    break;
                case "Red Mullet":
                    Item = new Ingredient(item);
                    break;
                case "Herring":
                    Item = new Ingredient(item);
                    break;
                case "Eel":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Octopus":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Red Snapper":
                    Item = new Ingredient(item);
                    break;
                case "Squid":
                    Item = new Ingredient(item);
                    break;
                case "Seaweed":
                    Item = new Ingredient(item);
                    break;
                case "Green Algae":
                    Item = new Ingredient(item);
                    break;
                case "Sea Cucumber":
                    Item = new Ingredient(item);
                    break;
                case "Super Cucumber":
                    Item = new Ingredient(item, fishing: 2, cauldronLuck: 1, luck: 2, butterfliesChance: 3);
                    break;
                case "Ghostfish":
                    Item = new Ingredient(item);
                    break;
                case "White Algae":
                    Item = new Ingredient(item);
                    break;
                case "Stonefish":
                    Item = new Ingredient(item, fishing: 3, defense: 3, speed: -1);
                    break;
                case "Crimsonfish":
                    Item = new Ingredient(item, attack: 3, cauldronLuck: 3);
                    break;
                case "Son of Crimsonfish":
                    Item = new Ingredient(item, attack: 3, cauldronLuck: 3);
                    break;
                case "Angler":
                    Item = new Ingredient(item, fishing: 3, cauldronLuck: 3);
                    break;
                case "Ms. Angler":
                    Item = new Ingredient(item, fishing: 3, cauldronLuck: 3);
                    break;
                case "Ice Pip":
                    Item = new Ingredient(item, fishing: 3, speed: -1, defense: 1);
                    break;
                case "Lava Eel":
                    Item = new Ingredient(item, fishing: 3, attack: 2);
                    break;
                case "Legend":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 3);
                    break;
                case "Legend II":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 3);
                    break;
                case "Sandfish":
                    Item = new Ingredient(item, fishing: 2, mining: 1, defense: 2);
                    break;
                case "Scorpion Carp":
                    Item = new Ingredient(item, fishing: 2, attack: 2);
                    break;
                case "Treasure Chest":
                    Item = new Ingredient(item, fishing: 2, cauldronLuck: 3, luck: 3);
                    break;
                case "Joja Cola":
                    Item = new Ingredient(item, speed: 1, farming: -1);
                    break;
                case "Trash":
                    Item = new Ingredient(item);
                    break;
                case "Driftwood":
                    Item = new Ingredient(item);
                    break;
                case "Broken Glasses":
                    Item = new Ingredient(item);
                    break;
                case "Broken CD":
                    Item = new Ingredient(item);
                    break;
                case "Soggy Newspaper":
                    Item = new Ingredient(item);
                    break;
                case "Large Egg":
                    Item = new Ingredient(item, maxEnergy: 2);
                    break;
                case "Egg":
                    Item = new Ingredient(item, maxEnergy: 1);
                    break;
                case "Hay":
                    Item = new Ingredient(item, boomChance: 1, farming: 1);
                    break;
                case "Milk":
                    Item = new Ingredient(item, maxEnergy: 1);
                    break;
                case "Large Milk":
                    Item = new Ingredient(item, maxEnergy: 2);
                    break;
                case "Green Bean":
                    Item = new Ingredient(item);
                    break;
                case "Cauliflower":
                    Item = new Ingredient(item);
                    break;
                case "Potato":
                    Item = new Ingredient(item);
                    break;
                case "Fried Egg":
                    Item = new Ingredient(item);
                    break;
                case "Omelet":
                    Item = new Ingredient(item);
                    break;
                case "Salad":
                    Item = new Ingredient(item);
                    break;
                case "Cheese Cauliflower":
                    Item = new Ingredient(item);
                    break;
                case "Baked Fish":
                    Item = new Ingredient(item);
                    break;
                case "Parsnip Soup":
                    Item = new Ingredient(item);
                    break;
                case "Vegetable Medley":
                    Item = new Ingredient(item);
                    break;
                case "Complete Breakfast":
                    Item = new Ingredient(item);
                    break;
                case "Fried Calamari":
                    Item = new Ingredient(item);
                    break;
                case "Strange Bun":
                    Item = new Ingredient(item, cauldronLuck: 2, boomChance: 2);
                    break;
                case "Lucky Lunch":
                    Item = new Ingredient(item);
                    break;
                case "Fried Mushroom":
                    Item = new Ingredient(item);
                    break;
                case "Pizza":
                    Item = new Ingredient(item);
                    break;
                case "Bean Hotpot":
                    Item = new Ingredient(item);
                    break;
                case "Glazed Yams":
                    Item = new Ingredient(item);
                    break;
                case "Carp Surprise":
                    Item = new Ingredient(item, butterfliesChance: 3, boomChance: 3, garlicOil: 3, monsterMusk: 3, debuffImmunity: 3, farming: 3, mining: 3, fishing: 3, foraging: 3, attack: 3, defense: 3, maxEnergy: 3, luck: 3, magneticRadius: 3, speed: 3);
                    break;
                case "Hashbrowns":
                    Item = new Ingredient(item);
                    break;
                case "Pancakes":
                    Item = new Ingredient(item);
                    break;
                case "Salmon Dinner":
                    Item = new Ingredient(item);
                    break;
                case "Fish Taco":
                    Item = new Ingredient(item);
                    break;
                case "Crispy Bass":
                    Item = new Ingredient(item);
                    break;
                case "Pepper Poppers":
                    Item = new Ingredient(item);
                    break;
                case "Bread":
                    Item = new Ingredient(item);
                    break;
                case "Tom Kha Soup":
                    Item = new Ingredient(item);
                    break;
                case "Trout Soup":
                    Item = new Ingredient(item);
                    break;
                case "Chocolate Cake":
                    Item = new Ingredient(item, speed: -1, cauldronLuck: 3);
                    break;
                case "Pink Cake":
                    Item = new Ingredient(item, speed: -1, cauldronLuck: 3);
                    break;
                case "Rhubarb Pie":
                    Item = new Ingredient(item);
                    break;
                case "Cookie":
                    Item = new Ingredient(item);
                    break;
                case "Spaghetti":
                    Item = new Ingredient(item);
                    break;
                case "Fried Eel":
                    Item = new Ingredient(item);
                    break;
                case "Spicy Eel":
                    Item = new Ingredient(item);
                    break;
                case "Sashimi":
                    Item = new Ingredient(item);
                    break;
                case "Maki Roll":
                    Item = new Ingredient(item);
                    break;
                case "Tortilla":
                    Item = new Ingredient(item);
                    break;
                case "Red Plate":
                    Item = new Ingredient(item);
                    break;
                case "Eggplant Parmesan":
                    Item = new Ingredient(item);
                    break;
                case "Rice Pudding":
                    Item = new Ingredient(item);
                    break;
                case "Ice Cream":
                    Item = new Ingredient(item);
                    break;
                case "Blueberry Tart":
                    Item = new Ingredient(item);
                    break;
                case "Autumn's Bounty":
                    Item = new Ingredient(item);
                    break;
                case "Pumpkin Soup":
                    Item = new Ingredient(item);
                    break;
                case "Super Meal":
                    Item = new Ingredient(item);
                    break;
                case "Cranberry Sauce":
                    Item = new Ingredient(item);
                    break;
                case "Stuffing":
                    Item = new Ingredient(item);
                    break;
                case "Farmer's Lunch":
                    Item = new Ingredient(item);
                    break;
                case "Survival Burger":
                    Item = new Ingredient(item, defense: 3, debuffImmunity: 2);
                    break;
                case "Dish O' The Sea":
                    Item = new Ingredient(item);
                    break;
                case "Miner's Treat":
                    Item = new Ingredient(item);
                    break;
                case "Roots Platter":
                    Item = new Ingredient(item);
                    break;
                case "Sugar":
                    Item = new Ingredient(item, speed: 2, butterfliesChance: 2, boomChance: 2, cauldronLuck: -1);
                    break;
                case "Wheat Flour":
                    Item = new Ingredient(item);
                    break;
                case "Oil":
                    Item = new Ingredient(item);
                    break;
                case "Garlic":
                    Item = new Ingredient(item, garlicOil: 2);
                    break;
                case "Kale":
                    Item = new Ingredient(item);
                    break;
                case "Tea Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Rhubarb":
                    Item = new Ingredient(item);
                    break;
                case "Triple Shot Espresso":
                    Item = new Ingredient(item, duration: 1);
                    break;
                case "Melon":
                    Item = new Ingredient(item);
                    break;
                case "Tomato":
                    Item = new Ingredient(item);
                    break;
                case "Morel":
                    Item = new Ingredient(item);
                    break;
                case "Blueberry":
                    Item = new Ingredient(item);
                    break;
                case "Fiddlehead Fern":
                    Item = new Ingredient(item);
                    break;
                case "Hot Pepper":
                    Item = new Ingredient(item);
                    break;
                case "Warp Totem: Desert":
                    Item = new Ingredient(item, attack: 2);
                    break;
                case "Wheat":
                    Item = new Ingredient(item);
                    break;
                case "Radish":
                    Item = new Ingredient(item);
                    break;
                case "Seafoam Pudding":
                    Item = new Ingredient(item);
                    break;
                case "Red Cabbage":
                    Item = new Ingredient(item);
                    break;
                case "Flounder":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Starfruit":
                    Item = new Ingredient(item);
                    break;
                case "Midnight Carp":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Corn":
                    Item = new Ingredient(item);
                    break;
                case "Unmilled Rice":
                    Item = new Ingredient(item);
                    break;
                case "Eggplant":
                    Item = new Ingredient(item);
                    break;
                case "Rice Shoot":
                    Item = new Ingredient(item);
                    break;
                case "Artichoke":
                    Item = new Ingredient(item);
                    break;
                case "Artifact Trove":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 1);
                    break;
                case "Pumpkin":
                    Item = new Ingredient(item);
                    break;
                case "Wilted Bouquet":
                    Item = new Ingredient(item);
                    break;
                case "Bok Choy":
                    Item = new Ingredient(item);
                    break;
                case "Magic Rock Candy":
                    Item = new Ingredient(item);
                    break;
                case "Yam":
                    Item = new Ingredient(item);
                    break;
                case "Chanterelle":
                    Item = new Ingredient(item);
                    break;
                case "Cranberries":
                    Item = new Ingredient(item);
                    break;
                case "Holly":
                    Item = new Ingredient(item);
                    break;
                case "Beet":
                    Item = new Ingredient(item);
                    break;
                case "Cherry Bomb":
                    Item = new Ingredient(item);
                    break;
                case "Bomb":
                    Item = new Ingredient(item);
                    break;
                case "Mega Bomb":
                    Item = new Ingredient(item);
                    break;
                case "Brick Floor":
                    Item = new Ingredient(item);
                    break;
                case "Twig":
                    Item = new Ingredient(item);
                    break;
                case "Salmonberry":
                    Item = new Ingredient(item);
                    break;
                case "Grass Starter":
                    Item = new Ingredient(item);
                    break;
                case "Hardwood Fence":
                    Item = new Ingredient(item);
                    break;
                case "Amaranth Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Amaranth":
                    Item = new Ingredient(item);
                    break;
                case "Grape Starter":
                    Item = new Ingredient(item);
                    break;
                case "Hops Starter":
                    Item = new Ingredient(item);
                    break;
                case "Pale Ale":
                    Item = new Ingredient(item);
                    break;
                case "Hops":
                    Item = new Ingredient(item);
                    break;
                case "Void Egg":
                    Item = new Ingredient(item);
                    break;
                case "Mayonnaise":
                    Item = new Ingredient(item);
                    break;
                case "Duck Mayonnaise":
                    Item = new Ingredient(item);
                    break;
                case "Void Mayonnaise":
                    Item = new Ingredient(item);
                    break;
                case "Acorn":
                    Item = new Ingredient(item);
                    break;
                case "Maple Seed":
                    Item = new Ingredient(item);
                    break;
                case "Pine Cone":
                    Item = new Ingredient(item);
                    break;
                case "Wood Fence":
                    Item = new Ingredient(item);
                    break;
                case "Stone Fence":
                    Item = new Ingredient(item);
                    break;
                case "Iron Fence":
                    Item = new Ingredient(item);
                    break;
                case "Gate":
                    Item = new Ingredient(item);
                    break;
                case "Wood Floor":
                    Item = new Ingredient(item);
                    break;
                case "Stone Floor":
                    Item = new Ingredient(item);
                    break;
                case "Clay":
                    Item = new Ingredient(item);
                    break;
                case "Weathered Floor":
                    Item = new Ingredient(item);
                    break;
                case "Crystal Floor":
                    Item = new Ingredient(item);
                    break;
                case "Copper Bar":
                    Item = new Ingredient(item, defense: 1, duration: 1);
                    break;
                case "Iron Bar":
                    Item = new Ingredient(item, defense: 1, duration: 2);
                    break;
                case "Gold Bar":
                    Item = new Ingredient(item, defense: 2, duration: 2);
                    break;
                case "Iridium Bar":
                    Item = new Ingredient(item, defense: 3, duration: 3);
                    break;
                case "Refined Quartz":
                    Item = new Ingredient(item, luck: 1);
                    break;
                case "Honey":
                    Item = new Ingredient(item);
                    break;
                case "Tea Set":
                    Item = new Ingredient(item);
                    break;
                case "Pickles":
                    Item = new Ingredient(item);
                    break;
                case "Jelly":
                    Item = new Ingredient(item);
                    break;
                case "Beer":
                    Item = new Ingredient(item);
                    break;
                case "Rare Seed":
                    Item = new Ingredient(item, cauldronLuck: 3);
                    break;
                case "Wine":
                    Item = new Ingredient(item);
                    break;
                case "Energy Tonic":
                    Item = new Ingredient(item);
                    break;
                case "Juice":
                    Item = new Ingredient(item);
                    break;
                case "Muscle Remedy":
                    Item = new Ingredient(item, maxEnergy: 3);
                    break;
                case "Basic Fertilizer":
                    Item = new Ingredient(item);
                    break;
                case "Quality Fertilizer":
                    Item = new Ingredient(item);
                    break;
                case "Basic Retaining Soil":
                    Item = new Ingredient(item);
                    break;
                case "Quality Retaining Soil":
                    Item = new Ingredient(item);
                    break;
                case "Clam":
                    Item = new Ingredient(item);
                    break;
                case "Golden Pumpkin":
                    Item = new Ingredient(item, cauldronLuck: 3);
                    break;
                case "Poppy":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Copper Ore":
                    Item = new Ingredient(item);
                    break;
                case "Iron Ore":
                    Item = new Ingredient(item);
                    break;
                case "Coal":
                    Item = new Ingredient(item);
                    break;
                case "Gold Ore":
                    Item = new Ingredient(item);
                    break;
                case "Iridium Ore":
                    Item = new Ingredient(item);
                    break;
                case "Nautilus Shell":
                    Item = new Ingredient(item);
                    break;
                case "Coral":
                    Item = new Ingredient(item);
                    break;
                case "Rainbow Shell":
                    Item = new Ingredient(item);
                    break;
                case "Coffee":
                    Item = new Ingredient(item);
                    break;
                case "Spice Berry":
                    Item = new Ingredient(item);
                    break;
                case "Sea Urchin":
                    Item = new Ingredient(item);
                    break;
                case "Grape":
                    Item = new Ingredient(item);
                    break;
                case "Spring Onion":
                    Item = new Ingredient(item);
                    break;
                case "Strawberry":
                    Item = new Ingredient(item);
                    break;
                case "Straw Floor":
                    Item = new Ingredient(item);
                    break;
                case "Sweet Pea":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Field Snack":
                    Item = new Ingredient(item);
                    break;
                case "Common Mushroom":
                    Item = new Ingredient(item);
                    break;
                case "Wood Path":
                    Item = new Ingredient(item);
                    break;
                case "Wild Plum":
                    Item = new Ingredient(item);
                    break;
                case "Gravel Path":
                    Item = new Ingredient(item);
                    break;
                case "Hazelnut":
                    Item = new Ingredient(item);
                    break;
                case "Crystal Path":
                    Item = new Ingredient(item);
                    break;
                case "Blackberry":
                    Item = new Ingredient(item);
                    break;
                case "Cobblestone Path":
                    Item = new Ingredient(item);
                    break;
                case "Winter Root":
                    Item = new Ingredient(item);
                    break;
                case "Blue Slime Egg":
                    Item = new Ingredient(item);
                    break;
                case "Crystal Fruit":
                    Item = new Ingredient(item);
                    break;
                case "Stepping Stone Path":
                    Item = new Ingredient(item);
                    break;
                case "Snow Yam":
                    Item = new Ingredient(item);
                    break;
                case "Sweet Gem Berry":
                    Item = new Ingredient(item, speed: 3, cauldronLuck: 3);
                    break;
                case "Crocus":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Vinegar":
                    Item = new Ingredient(item);
                    break;
                case "Red Mushroom":
                    Item = new Ingredient(item);
                    break;
                case "Sunflower":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Purple Mushroom":
                    Item = new Ingredient(item, cauldronLuck: 3, butterfliesChance: 3);
                    break;
                case "Rice":
                    Item = new Ingredient(item);
                    break;
                case "Cheese":
                    Item = new Ingredient(item);
                    break;
                case "Fairy Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Goat Cheese":
                    Item = new Ingredient(item);
                    break;
                case "Tulip Bulb":
                    Item = new Ingredient(item);
                    break;
                case "Cloth":
                    Item = new Ingredient(item);
                    break;
                case "Jazz Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Truffle":
                    Item = new Ingredient(item);
                    break;
                case "Sunflower Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Truffle Oil":
                    Item = new Ingredient(item);
                    break;
                case "Coffee Bean":
                    Item = new Ingredient(item);
                    break;
                case "Stardrop":
                    Item = new Ingredient(item);
                    break;
                case "Goat Milk":
                    Item = new Ingredient(item);
                    break;
                case "Red Slime Egg":
                    Item = new Ingredient(item);
                    break;
                case "L. Goat Milk":
                    Item = new Ingredient(item);
                    break;
                case "Purple Slime Egg":
                    Item = new Ingredient(item);
                    break;
                case "Wool":
                    Item = new Ingredient(item);
                    break;
                case "Explosive Ammo":
                    Item = new Ingredient(item);
                    break;
                case "Duck Egg":
                    Item = new Ingredient(item);
                    break;
                case "Duck Feather":
                    Item = new Ingredient(item);
                    break;
                case "Caviar":
                    Item = new Ingredient(item);
                    break;
                case "Rabbit's Foot":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 3);
                    break;
                case "Aged Roe":
                    Item = new Ingredient(item, duration: 3);
                    break;
                case "Stone Base":
                    Item = new Ingredient(item);
                    break;
                case "Poppy Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Ancient Fruit":
                    Item = new Ingredient(item, cauldronLuck: 3, luck: 3);
                    break;
                case "Spangle Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Algae Soup":
                    Item = new Ingredient(item);
                    break;
                case "Pale Broth":
                    Item = new Ingredient(item);
                    break;
                case "Bouquet":
                    Item = new Ingredient(item);
                    break;
                case "Mead":
                    Item = new Ingredient(item);
                    break;
                case "Decorative Pot":
                    Item = new Ingredient(item);
                    break;
                case "Drum Block":
                    Item = new Ingredient(item);
                    break;
                case "Flute Block":
                    Item = new Ingredient(item);
                    break;
                case "Speed-Gro":
                    Item = new Ingredient(item);
                    break;
                case "Deluxe Speed-Gro":
                    Item = new Ingredient(item);
                    break;
                case "Parsnip Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Bean Starter":
                    Item = new Ingredient(item);
                    break;
                case "Cauliflower Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Potato Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Garlic Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Kale Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Rhubarb Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Melon Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Tomato Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Blueberry Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Pepper Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Wheat Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Radish Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Red Cabbage Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Starfruit Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Corn Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Eggplant Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Artichoke Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Pumpkin Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Bok Choy Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Yam Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Cranberry Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Beet Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Spring Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Summer Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Fall Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Winter Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Ancient Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Geode":
                    Item = new Ingredient(item);
                    break;
                case "Frozen Geode":
                    Item = new Ingredient(item);
                    break;
                case "Magma Geode":
                    Item = new Ingredient(item);
                    break;
                case "Alamite":
                    Item = new Ingredient(item);
                    break;
                case "Bixite":
                    Item = new Ingredient(item);
                    break;
                case "Baryte":
                    Item = new Ingredient(item);
                    break;
                case "Aerinite":
                    Item = new Ingredient(item);
                    break;
                case "Calcite":
                    Item = new Ingredient(item);
                    break;
                case "Dolomite":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Esperite":
                    Item = new Ingredient(item);
                    break;
                case "Fluorapatite":
                    Item = new Ingredient(item);
                    break;
                case "Geminite":
                    Item = new Ingredient(item);
                    break;
                case "Helvite":
                    Item = new Ingredient(item);
                    break;
                case "Jamborite":
                    Item = new Ingredient(item);
                    break;
                case "Jagoite":
                    Item = new Ingredient(item);
                    break;
                case "Kyanite":
                    Item = new Ingredient(item);
                    break;
                case "Lunarite":
                    Item = new Ingredient(item);
                    break;
                case "Malachite":
                    Item = new Ingredient(item);
                    break;
                case "Neptunite":
                    Item = new Ingredient(item);
                    break;
                case "Lemon Stone":
                    Item = new Ingredient(item);
                    break;
                case "Nekoite":
                    Item = new Ingredient(item);
                    break;
                case "Orpiment":
                    Item = new Ingredient(item);
                    break;
                case "Petrified Slime":
                    Item = new Ingredient(item, monsterMusk: 2);
                    break;
                case "Thunder Egg":
                    Item = new Ingredient(item, attack: 2);
                    break;
                case "Pyrite":
                    Item = new Ingredient(item);
                    break;
                case "Ocean Stone":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Ghost Crystal":
                    Item = new Ingredient(item);
                    break;
                case "Tigerseye":
                    Item = new Ingredient(item);
                    break;
                case "Jasper":
                    Item = new Ingredient(item);
                    break;
                case "Opal":
                    Item = new Ingredient(item);
                    break;
                case "Fire Opal":
                    Item = new Ingredient(item);
                    break;
                case "Celestine":
                    Item = new Ingredient(item);
                    break;
                case "Marble":
                    Item = new Ingredient(item);
                    break;
                case "Sandstone":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Granite":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Basalt":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Limestone":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Soapstone":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Hematite":
                    Item = new Ingredient(item);
                    break;
                case "Mudstone":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Obsidian":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Slate":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Fairy Stone":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 1);
                    break;
                case "Star Shards":
                    Item = new Ingredient(item);
                    break;
                case "Prehistoric Scapula":
                    Item = new Ingredient(item);
                    break;
                case "Prehistoric Tibia":
                    Item = new Ingredient(item);
                    break;
                case "Prehistoric Skull":
                    Item = new Ingredient(item);
                    break;
                case "Skeletal Hand":
                    Item = new Ingredient(item);
                    break;
                case "Prehistoric Rib":
                    Item = new Ingredient(item);
                    break;
                case "Prehistoric Vertebra":
                    Item = new Ingredient(item);
                    break;
                case "Skeletal Tail":
                    Item = new Ingredient(item);
                    break;
                case "Nautilus Fossil":
                    Item = new Ingredient(item);
                    break;
                case "Amphibian Fossil":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Palm Fossil":
                    Item = new Ingredient(item);
                    break;
                case "Trilobite":
                    Item = new Ingredient(item);
                    break;
                case "Artifact Spot":
                    Item = new Ingredient(item);
                    break;
                case "Tulip":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Summer Spangle":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Fairy Rose":
                    Item = new Ingredient(item, butterfliesChance: 1, luck: 2, cauldronLuck: 1);
                    break;
                case "Blue Jazz":
                    Item = new Ingredient(item, butterfliesChance: 1);
                    break;
                case "Sprinkler":
                    Item = new Ingredient(item);
                    break;
                case "Plum Pudding":
                    Item = new Ingredient(item);
                    break;
                case "Artichoke Dip":
                    Item = new Ingredient(item);
                    break;
                case "Stir Fry":
                    Item = new Ingredient(item);
                    break;
                case "Roasted Hazelnuts":
                    Item = new Ingredient(item);
                    break;
                case "Pumpkin Pie":
                    Item = new Ingredient(item);
                    break;
                case "Radish Salad":
                    Item = new Ingredient(item);
                    break;
                case "Fruit Salad":
                    Item = new Ingredient(item);
                    break;
                case "Blackberry Cobbler":
                    Item = new Ingredient(item);
                    break;
                case "Cranberry Candy":
                    Item = new Ingredient(item);
                    break;
                case "Apple":
                    Item = new Ingredient(item);
                    break;
                case "Green Tea":
                    Item = new Ingredient(item);
                    break;
                case "Bruschetta":
                    Item = new Ingredient(item);
                    break;
                case "Quality Sprinkler":
                    Item = new Ingredient(item);
                    break;
                case "Cherry Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Apricot Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Orange Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Peach Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Pomegranate Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Apple Sapling":
                    Item = new Ingredient(item);
                    break;
                case "Apricot":
                    Item = new Ingredient(item);
                    break;
                case "Orange":
                    Item = new Ingredient(item);
                    break;
                case "Peach":
                    Item = new Ingredient(item);
                    break;
                case "Pomegranate":
                    Item = new Ingredient(item);
                    break;
                case "Cherry":
                    Item = new Ingredient(item);
                    break;
                case "Iridium Sprinkler":
                    Item = new Ingredient(item);
                    break;
                case "Coleslaw":
                    Item = new Ingredient(item);
                    break;
                case "Fiddlehead Risotto":
                    Item = new Ingredient(item);
                    break;
                case "Poppyseed Muffin":
                    Item = new Ingredient(item);
                    break;
                case "Green Slime Egg":
                    Item = new Ingredient(item);
                    break;
                case "Rain Totem":
                    Item = new Ingredient(item, fishing: 3);
                    break;
                case "Mutant Carp":
                    Item = new Ingredient(item, boomChance: 3, cauldronLuck: 3, magneticRadius: 3);
                    break;
                case "Radioactive Carp":
                    Item = new Ingredient(item, boomChance: 3, cauldronLuck: 3, magneticRadius: 3);
                    break;
                case "Bug Meat":
                    Item = new Ingredient(item);
                    break;
                case "Bait":
                    Item = new Ingredient(item);
                    break;
                case "Spinner":
                    Item = new Ingredient(item);
                    break;
                case "Dressed Spinner":
                    Item = new Ingredient(item);
                    break;
                case "Warp Totem: Farm":
                    Item = new Ingredient(item, farming: 2);
                    break;
                case "Warp Totem: Mountains":
                    Item = new Ingredient(item, mining: 2);
                    break;
                case "Warp Totem: Beach":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Barbed Hook":
                    Item = new Ingredient(item);
                    break;
                case "Lead Bobber":
                    Item = new Ingredient(item);
                    break;
                case "Treasure Hunter":
                    Item = new Ingredient(item);
                    break;
                case "Trap Bobber":
                    Item = new Ingredient(item);
                    break;
                case "Cork Bobber":
                    Item = new Ingredient(item);
                    break;
                case "Sturgeon":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Tiger Trout":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Bullhead":
                    Item = new Ingredient(item);
                    break;
                case "Tilapia":
                    Item = new Ingredient(item);
                    break;
                case "Chub":
                    Item = new Ingredient(item);
                    break;
                case "Magnet":
                    Item = new Ingredient(item);
                    break;
                case "Dorado":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Albacore":
                    Item = new Ingredient(item);
                    break;
                case "Shad":
                    Item = new Ingredient(item);
                    break;
                case "Lingcod":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Halibut":
                    Item = new Ingredient(item);
                    break;
                case "Hardwood":
                    Item = new Ingredient(item);
                    break;
                case "Crab Pot":
                    Item = new Ingredient(item);
                    break;
                case "Lobster":
                    Item = new Ingredient(item);
                    break;
                case "Crayfish":
                    Item = new Ingredient(item);
                    break;
                case "Crab":
                    Item = new Ingredient(item);
                    break;
                case "Cockle":
                    Item = new Ingredient(item);
                    break;
                case "Mussel":
                    Item = new Ingredient(item);
                    break;
                case "Shrimp":
                    Item = new Ingredient(item);
                    break;
                case "Snail":
                    Item = new Ingredient(item);
                    break;
                case "Periwinkle":
                    Item = new Ingredient(item);
                    break;
                case "Oyster":
                    Item = new Ingredient(item);
                    break;
                case "Maple Syrup":
                    Item = new Ingredient(item);
                    break;
                case "Oak Resin":
                    Item = new Ingredient(item);
                    break;
                case "Pine Tar":
                    Item = new Ingredient(item);
                    break;
                case "Chowder":
                    Item = new Ingredient(item);
                    break;
                case "Fish Stew":
                    Item = new Ingredient(item);
                    break;
                case "Escargot":
                    Item = new Ingredient(item);
                    break;
                case "Lobster Bisque":
                    Item = new Ingredient(item);
                    break;
                case "Maple Bar":
                    Item = new Ingredient(item);
                    break;
                case "Crab Cakes":
                    Item = new Ingredient(item);
                    break;
                case "Shrimp Cocktail":
                    Item = new Ingredient(item);
                    break;
                case "Woodskip":
                    Item = new Ingredient(item);
                    break;
                case "Strawberry Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Jack-O-Lantern":
                    Item = new Ingredient(item);
                    break;
                case "Rotten Plant":
                    Item = new Ingredient(item);
                    break;
                case "Omni Geode":
                    Item = new Ingredient(item);
                    break;
                case "Slime":
                    Item = new Ingredient(item);
                    break;
                case "Bat Wing":
                    Item = new Ingredient(item);
                    break;
                case "Solar Essence":
                    Item = new Ingredient(item, butterfliesChance: 3, cauldronLuck: 3);
                    break;
                case "Void Essence":
                    Item = new Ingredient(item, butterfliesChance: 3, cauldronLuck: 3);
                    break;
                case "Mixed Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Fiber":
                    Item = new Ingredient(item, farming: -1);
                    break;
                case "Oil of Garlic":
                    Item = new Ingredient(item);
                    break;
                case "Life Elixir":
                    Item = new Ingredient(item);
                    break;
                case "Wild Bait":
                    Item = new Ingredient(item);
                    break;
                case "Glacierfish":
                    Item = new Ingredient(item, debuffImmunity: 3, cauldronLuck: 3);
                    break;
                case "Glacierfish Jr.":
                    Item = new Ingredient(item, debuffImmunity: 3, cauldronLuck: 3);
                    break;
                case "Battery Pack":
                    Item = new Ingredient(item);
                    break;
                case "Void Salmon":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Slimejack":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Pearl":
                    Item = new Ingredient(item);
                    break;
                case "Midnight Squid":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Spook Fish":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Blobfish":
                    Item = new Ingredient(item, fishing: 3);
                    break;
                case "Cactus Seeds":
                    Item = new Ingredient(item);
                    break;
                case "Iridium Milk":
                    Item = new Ingredient(item);
                    break;
                case "Tree Fertilizer":
                    Item = new Ingredient(item);
                    break;
                case "Dinosaur Mayonnaise":
                    Item = new Ingredient(item);
                    break;
                case "Void Ghost Pendant":
                    Item = new Ingredient(item);
                    break;
                case "Movie Ticket":
                    Item = new Ingredient(item, luck: 3, cauldronLuck: 3, butterfliesChance: 3);
                    break;
                case "Roe":
                    Item = new Ingredient(item);
                    break;
                case "Squid Ink":
                    Item = new Ingredient(item);
                    break;
                case "Tea Leaves":
                    Item = new Ingredient(item);
                    break;
                case "Monster Musk":
                    Item = new Ingredient(item, monsterMusk: 3);
                    break;
                case "Garlic Oil":
                    Item = new Ingredient(item, garlicOil: 3);
                    break;
                case "Blue Discus":
                    Item = new Ingredient(item, fishing: 2);
                    break;
                case "Stingray":
                    Item = new Ingredient(item, fishing: 2);
                    break;

                default:
                    Item = new Ingredient(item);
                    break;
            }
            return Item;
        }

        #region disposeable support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cauldron()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion disposeable support
    }
}
