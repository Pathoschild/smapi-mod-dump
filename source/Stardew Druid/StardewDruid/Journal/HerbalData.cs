/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Cast.Mists;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Characters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static StardewDruid.Journal.HerbalData;

namespace StardewDruid.Journal
{
    public class HerbalData
    {

        public enum herbals
        {

            none,
            ligna,
            melius_ligna,
            satius_ligna,
            magnus_ligna,
            optimus_ligna,
            impes,
            melius_impes,
            satius_impes,
            magnus_impes,
            optimus_impes,
            celeri,
            melius_celeri,
            satius_celeri,
            magnus_celeri,
            optimus_celeri,
            faeth,
            aether,
            ambrosia,

        }

        public Dictionary<string, Herbal> herbalism = new();

        public Dictionary<herbals, HerbalBuff> applied = new();

        public Dictionary<herbals, List<string>> titles = new()
        {

            [herbals.ligna] = new() { "Ligna", "Boosts rite damage and success-rate", },
            [herbals.impes] = new() { "Vigores", "Boosts charge-ups and rite critical hit chance", },
            [herbals.celeri] = new() { "Celeri", "Boosts movement speed, lowers rite cooldowns", },
            [herbals.faeth] = new() { "Essence", "Magical resources used for advanced alchemy", },
        };

        public Dictionary<herbals,List<herbals>> lines = new()
        {
            [herbals.ligna] = new() {
                herbals.ligna,
                herbals.melius_ligna,
                herbals.satius_ligna,
                herbals.magnus_ligna,
                herbals.optimus_ligna, 
            },
            [herbals.impes] = new() {
                herbals.impes,
                herbals.melius_impes,
                herbals.satius_impes,
                herbals.magnus_impes,
                herbals.optimus_impes,
            },
            [herbals.celeri] = new() {
                herbals.celeri,
                herbals.melius_celeri,
                herbals.satius_celeri,
                herbals.magnus_celeri,
                herbals.optimus_celeri,
            },
        };

        public Dictionary<int, IconData.relics> requirements = new()
        {
            [0] = IconData.relics.herbalism_mortar,
            [1] = IconData.relics.herbalism_pan,
            [2] = IconData.relics.herbalism_still,
            [3] = IconData.relics.herbalism_crucible,

        };

        public double consumeBuffer;

        public HerbalData()
        {

            herbalism = HerbalList();

        }

        public int MaxHerbal()
        {

            int max = -1;

            foreach (KeyValuePair<int, IconData.relics> requirement in requirements)
            {

                if (!Mod.instance.save.reliquary.ContainsKey(requirement.Value.ToString()))
                {

                    break;

                }

                max++;

            }

            return max;

        }

        public List<List<string>> OrganiseHerbals()
        {

            List<List<string>> source = new()
            {
                
                new List<string>()

            };

            int max = MaxHerbal();

            foreach (KeyValuePair<herbals, List<herbals>> line in lines)
            {

                foreach (herbals herbal in line.Value)
                {

                    string key = herbal.ToString();

                    if (herbalism[key].level > max)
                    {

                        source.Last().Add("blank");

                    }
                    else
                    {

                        Mod.instance.herbalData.CheckHerbal(key);

                        source.Last().Add(key);

                    }

                    if (herbal == lines[line.Key].Last())
                    {

                        source.Last().Add("configure");

                    }

                }

            }

            if(max >= 3)
            {

                source.Add(new());

                source.Last().Add(herbals.faeth.ToString());

            }

            if (max >= 4)
            {

                source.Last().Add(herbals.aether.ToString());

                source.Last().Add(herbals.ambrosia.ToString());

            }


            return source;

        }

        public static Dictionary<string,Herbal> HerbalList()
        {

            Dictionary<string, Herbal> potions = new();

            // ====================================================================
            // Ligna line

            potions[herbals.ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask1,

                level = 0,

                duration = 0,

                title = "Ligna",

                description = "Nature, liquified and sticky.",
                
                ingredients = new(){ ["(O)92"] = "Sap", ["(O)766"] = "Slime", ["Moss"] = "Moss", },

                bases = new() { },

                health = 10,

                stamina = 15,

                details = new()
                {
                    "Restores: 10 Health, 15 Stamina",
                    "Requires: 1x Organic Substance"
                }

            };


            potions[herbals.melius_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.melius_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask2,

                level = 1,

                duration = 60,

                title = "Melius Ligna",

                description = "Like bark-root tea, with more bark, and root to boot.",

                ingredients = new() { ["(O)311"] = "Acorn", ["(O)310"] = "MapleSeed", ["(O)309"] = "Pinecorn", ["(O)292"] = "MahoganySeed", },

                bases = new() { herbals.ligna, },

                health = 15,

                stamina = 30,

                details = new()
                {
                    "Restores: 15 Health, 30 Stamina",
                    "Effect: Alignment Level 1",
                    "Requires: Base, 1x Tree Seed"
                }

            };


            potions[herbals.satius_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.satius_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask3,

                level = 2,

                duration = 90,

                title = "Satius Ligna",

                description = "Infused with the leaves and petals of weeds. Toxic to dogs.",

                ingredients = new() { ["(O)418"] = "Crocus", ["(O)18"] = "Daffodil", ["(O)22"] = "Dandelion", ["(O)402"] = "Sweet Pea", ["(O)273"] = "Rice Shoot", },

                bases = new() { herbals.melius_ligna, },

                health = 20,

                stamina = 80,

                details = new()
                {
                    "Restores: 20 Health, 80 Stamina",
                    "Effect: Alignment Level 2",
                    "Requires: Base, 1x Wild Flower"
                }

            };


            potions[herbals.magnus_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.magnus_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask4,

                level = 3,

                duration = 120,

                title = "Magnus Ligna",

                description = "Potent oils enhance the rich, nutty flavour profile of the base mixture.",

                ingredients = new() { ["(O)247"] = "Oil", ["(O)431"] = "Sunflower Seeds", ["(O)270"] = "Corn", ["(O)271"] = "Unmilled Rice", ["(O)421"] = "Sunflower", }, // },

                bases = new() { herbals.satius_ligna, },

                health = 50,

                stamina = 200,

                details = new()
                {
                    "Restores: 50 Health, 200 Stamina",
                    "Effect: Alignment Level 3",
                    "Ingredients: Base, 1x Vegetable Oil",
                }

            };

            potions[herbals.optimus_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.optimus_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask5,

                level = 4,

                duration = 150,

                title = "Optimus Ligna",

                description = "Brims with the vibrant colours of creation",

                ingredients = new() { },

                bases = new() { herbals.magnus_ligna, herbals.aether },

                health = 100,

                stamina = 400,

                details = new()
                {
                    "Restores: 100 Health, 400 Stamina",
                    "Effect: Vigorous Level 4",
                    "Requires: Base, Ether"
                }

            };

            // ====================================================================
            // Impes series

            potions[herbals.impes.ToString()] = new()
            {

                line = herbals.impes,

                herbal = herbals.impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle1,

                title = "Vigores",

                description = "The flavours of the earth in a bottle.",

                ingredients = new() { ["(O)399"] = "Spring Onion", ["(O)78"] = "Cave Carrot", ["(O)24"] = "Parsnip", ["(O)831"] = "Taro Tubers", ["(O)16"] = "Wild Horseradish", ["(O)412"] = "Winter Root",},

                bases = new() { },

                health = 15,

                stamina = 40,

                details = new()
                {
                    "Restores: 15 Health, 40 Stamina",
                    "Requires: 1x Wild Tuber"
                }

            };


            potions[herbals.melius_impes.ToString()] = new()
            {

                line = herbals.impes,

                herbal = herbals.melius_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle2,

                level = 1,

                duration = 60,

                title = "Melius Vigores",

                description = "Contains the essence of cave.",

                ingredients = new() {  ["(O)420"] = "Red Mushrooms", ["(O)404"] = "Common Mushrooms", ["(O)257"] = "Morel", ["(O)767"] = "Batwings", },

                bases = new() { herbals.impes, },

                health = 30,

                stamina = 80,

                details = new()
                {
                    "Restores: 30 Health, 80 Stamina",
                    "Effect: Vigorous Level 1",
                    "Requires: Base, 1x Cave Forage"
                }

            };


            potions[herbals.satius_impes.ToString()] = new()
            {

                line = herbals.impes,

                herbal = herbals.satius_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle3,

                level = 2,

                duration = 90,

                title = "Satius Vigores",

                description = "The spicy tang that enflames the regions.",

                ingredients = new() { ["(O)419"] = "Vinegar", ["(O)260"] = "Hot Pepper", ["(O)829"] = "Ginger", },

                bases = new() { herbals.melius_impes, },

                health = 45,

                stamina = 160,

                details = new()
                {
                    "Restores: 45 Health, 160 Stamina",
                    "Effect: Vigorous Level 2",
                    "Requires: Base, 1x Spicy Ingredient"
                }

            };


            potions[herbals.magnus_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.magnus_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle4,

                level = 3,

                duration = 120,

                title = "Magnus Vigores",

                description = "Gently raise to boiling temperature then leave to simmer.",

                ingredients = new() { ["(O)93"] = "Torch", ["(O)82"] = "Fire Quartz", ["(O)382"] = "Coal", },

                bases = new() { herbals.satius_impes, },

                health = 70,

                stamina = 320,

                details = new()
                {
                    "Restores: 70 Health, 320 Stamina",
                    "Effect: Vigorous Level 3",
                    "Requires: Base, 1x Combustible Material"
                }

            };

            potions[herbals.optimus_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.optimus_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle5,

                level = 4,

                duration = 150,

                title = "Optimus Vigores",

                description = "It burns, but it stays down.",

                ingredients = new() { },

                bases = new() { herbals.magnus_impes, herbals.aether, },

                health = 180,

                stamina = 560,

                details = new()
                {
                    "Restores: 180 Health, 560 Stamina",
                    "Effect: Vigorous Level 4",
                    "Requires: Base, Ether"
                }

            };

            // ====================================================================
            // Celeri series

            potions[herbals.celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.celeri,

                scheme = IconData.schemes.blueberry,

                container = IconData.relics.vial,

                content = IconData.relics.vial1,

                title = "Celeri",

                description = "Good for your joints and memory retention.",

                ingredients = new() {  ["(O)129"] = "Sardine", ["(O)131"] = "Anchovy", ["(O)137"] = "Smallmouth Bass", ["(O)145"] = "Sunfish", ["(O)132"] = "Bream", ["(O)147"] = "Herring", ["(O)142"] = "Carp", },

                bases = new() { },

                health = 15,

                stamina = 25,

                details = new()
                {
                    "Restores: 10 Health, 30 Stamina",
                    "Requires: 1x Common Fish"
                }

            };

            potions[herbals.melius_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.melius_celeri,

                scheme = IconData.schemes.blueberry,

                container = IconData.relics.vial,

                content = IconData.relics.vial2,

                level = 1,

                duration = 60,

                title = "Melius Celeri",

                description = "Now with extra fiber for good gut health.",

                ingredients = new() { ["(O)152"] = "Algae", ["(O)153"] = "Seaweed", ["(O)157"] = "White Algae", ["(O)815"] = "Tea Leaves", ["(O)433"] = "Coffee Bean", ["(O)167"] = "Joja Cola", },

                health = 20,

                stamina = 60,

                bases = new() { herbals.celeri, },

                details = new()
                {
                    "Restores: 20 Health, 60 Stamina",
                    "Effect: Celerity Level 1",
                    "Requires: Base, 1x Energy Supplement"
                }

            };


            potions[herbals.satius_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.satius_celeri,

                scheme = IconData.schemes.blueberry,

                container = IconData.relics.vial,

                content = IconData.relics.vial3,

                level = 2,

                duration = 90,

                title = "Satius Celeri",

                description = "Now with mineral extracts for revitalised skin and circulation.",

                ingredients = new() { ["(O)80"] = "Quartz", ["(O)86"] = "Earth Crystal", ["(O)168"] = "Trash", ["(O)169"] = "Driftwood", ["(O)170"] = "Broken Glasses", ["(O)171"] = "Broken CD", ["(O)172"] = "Soggy Newspaper", },

                health = 30,

                stamina = 120,

                bases = new() { herbals.melius_celeri, },

                details = new()
                {
                    "Restores: 30 Health, 120 Stamina",
                    "Effect: Celerity Level 2",
                    "Requires: Base, 1x Mineral Substance"
                }

            };


            potions[herbals.magnus_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.magnus_celeri,

                scheme = IconData.schemes.blueberry,

                container = IconData.relics.vial,

                content = IconData.relics.vial4,

                level = 3,

                duration = 120,

                title = "Magnus Celeri",

                description = "Now with organically sourced protein for heightened muscle recovery.",

                ingredients = new() { ["(O)718"] = "Cockle", ["(O)719"] = "Mussel", ["(O)720"] = "Shrimp", ["(O)721"] = "Snail", ["(O)722"] = "Periwinkle", ["(O)723"] = "Oyster", },

                health = 60,

                stamina = 240,

                bases = new() { herbals.satius_celeri, },

                details = new()
                {
                    "Restores: 60 Health, 240 Stamina",
                    "Effect: Celerity Level 3",
                    "Requires: Base, 1x Common Shellfish"
                }

            };

            potions[herbals.optimus_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.optimus_celeri,

                scheme = IconData.schemes.blueberry,

                container = IconData.relics.vial,

                content = IconData.relics.vial5,

                level = 4,

                duration = 150,

                title = "Optimus Celeri",

                description = "Now with fluff and sprinkles for added bliss.",

                ingredients = new() {},

                health = 120,

                stamina = 480,

                bases = new() { herbals.magnus_celeri, herbals.aether, },

                details = new()
                {
                    "Restores: 120 Health, 480 Stamina",
                    "Effect: Celerity Level 4",
                    "Requires: Base, Ether"
                }

            };

            // ====================================================================
            // Faeth

            potions[herbals.faeth.ToString()] = new()
            {

                line = HerbalData.herbals.faeth,

                herbal = HerbalData.herbals.faeth,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle5,

                level = 3,

                duration = 150,

                title = "Faeth",

                description = "The currency of the Fates.",

                ingredients = new() { ["(O)577"] = "Fairy Stone", ["(O)595"] = "Fairy Rose", ["(O)768"] = "Solar Essence", ["(O)769"] = "Void Essence", },

                bases = new() {},

                details = new()
                {
                    "Enchants machines",
                }

            };

            // ====================================================================
            // Ether

            potions[herbals.aether.ToString()] = new()
            {

                line = HerbalData.herbals.faeth,

                herbal = HerbalData.herbals.aether,

                scheme = IconData.schemes.ether,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle5,

                level = 4,

                duration = 150,

                title = "Ether",

                description = "The essence of the Ancient Ones.",

                ingredients = new() { ["(O)60"] = "Emerald", ["(O)64"] = "Ruby", ["(O)72"] = "Diamond", },

                bases = new() {},

                details = new()
                {
                    "Enhances potions",
                }

            };

            // ====================================================================
            // Dusth

            potions[herbals.ambrosia.ToString()] = new()
            {

                line = HerbalData.herbals.faeth,

                herbal = HerbalData.herbals.ambrosia,

                scheme = IconData.schemes.ether,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle5,

                level = 4,

                duration = 150,

                title = "Ambrosia",

                description = "A taste of divinity",

                ingredients = new() {},

                bases = new() { herbals.faeth, herbals.aether },

                details = new()
                {
                    "Restores full health and stamina.",
                    "Effect: Godlike 1",
                    "Enhances enchantments",
                }

            };
            return potions;

        }

        public void PotionBehaviour(int index)
        {

            HerbalData.herbals potion = herbals.ligna;

            switch (index)
            {

                case 11:

                    potion = herbals.impes;
                    break;

                case 17:

                    potion = herbals.celeri;
                    break;

            }

            if (!Mod.instance.save.potions.ContainsKey(potion))
            {

                Mod.instance.save.potions.Add(potion, 1);

            }

            switch (Mod.instance.save.potions[potion])
            {

                case 0:

                    Mod.instance.save.potions[potion] = 1;
                    break;
                case 1:
                    Mod.instance.save.potions[potion] = 2;
                    break;
                case 2:
                    Mod.instance.save.potions[potion] = 0;
                    break;

            }

        }

        public void CheckHerbal(string id)
        {

            herbalism[id].status = CheckInventory(id);

        }

        public int CheckInventory(string id)
        {

            Herbal herbal = herbalism[id];

            herbalism[id].amounts.Clear();

            bool craftable = false;

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {

                Item checkSlot = Game1.player.Items[i];

                if (checkSlot == null)
                {

                    continue;

                }

                Item checkItem = checkSlot.getOne();

                if (herbal.ingredients.ContainsKey(@checkItem.QualifiedItemId))
                {

                    if (!herbalism[id].amounts.ContainsKey(@checkItem.QualifiedItemId))
                    {

                        herbalism[id].amounts[@checkItem.QualifiedItemId] = Game1.player.Items[i].Stack;

                    }
                    else
                    {

                        herbalism[id].amounts[@checkItem.QualifiedItemId] += Game1.player.Items[i].Stack;

                    }

                    craftable = true;

                }

            }

            if (Mod.instance.save.herbalism.ContainsKey(herbal.herbal))
            {

                if (Mod.instance.save.herbalism[herbal.herbal] == 999)
                {

                    return 3;

                }

            }

            if (herbal.bases.Count > 0)
            {

                foreach (herbals required in herbal.bases)
                {

                    if (!Mod.instance.save.herbalism.ContainsKey(required))
                    {

                        return 2;

                    }

                    if (Mod.instance.save.herbalism[required] == 0)
                    {

                        return 2;

                    }

                }

            }

            if (craftable)
            {

                return 1;

            }

            return 0;

        }

        public void MassBrew()
        {

            for(int i = 1; i <= 15; i++)
            {

                BrewHerbal(((herbals)i).ToString(), 50);

            }

        }

        public void BrewHerbal(string id, int draught)
        {

            Herbal herbal = herbalism[id];

            int brewed = 0;

            if (Mod.instance.save.herbalism.ContainsKey(herbal.herbal))
            {

                draught = Math.Min(999 - Mod.instance.save.herbalism[herbal.herbal], draught);

            }

            if (draught == 0)
            {

                return;

            }

            if (herbal.bases.Count > 0)
            {

                foreach (herbals required in herbal.bases)
                {

                    if (Mod.instance.save.herbalism.ContainsKey(required))
                    {

                        draught = Math.Min(Mod.instance.save.herbalism[required], draught);

                    }
                    else
                    {

                        return;

                    }

                }

            }

            if (draught == 0)
            {

                return;

            }

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {

                Item checkSlot = Game1.player.Items[i];

                if (checkSlot == null)
                {

                    continue;

                }

                Item checkItem = checkSlot.getOne();

                if (herbal.ingredients.ContainsKey(@checkItem.QualifiedItemId))
                {

                    int brew = Math.Min(Game1.player.Items[i].Stack, (draught - brewed));

                    if (herbal.bases.Count > 0)
                    {

                        foreach (herbals required in herbal.bases)
                        {

                            Mod.instance.save.herbalism[required] -= brew;

                        }

                    }

                    Game1.player.Items[i].Stack -= brew;

                    if (Game1.player.Items[i].Stack <= 0)
                    {
                        Game1.player.Items[i] = null;

                    }

                    brewed += brew;

                }

                if(brewed >= draught)
                {

                    break;

                }

            }

            CheckHerbal(id);

            /*if(herbal.level < 4)
            {

                CheckHerbal(((herbals)(herbal.herbal + 1)).ToString());

            }*/

            Game1.player.currentLocation.playSound("bubbles");

            if (!Mod.instance.save.herbalism.ContainsKey(herbal.herbal))
            {

                Mod.instance.save.herbalism[herbal.herbal] = brewed;

                return;

            }

            Mod.instance.save.herbalism[herbal.herbal] += brewed;


        }

        public void ConsumeHerbal(string id)
        {

            Herbal herbal = herbalism[id];

            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + herbal.stamina);

            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + herbal.health);

            Microsoft.Xna.Framework.Rectangle healthBox = Game1.player.GetBoundingBox();

            if(Game1.currentGameTime.TotalGameTime.TotalSeconds > consumeBuffer)
            {

                consumeBuffer = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

                ConsumePotion hudmessage = new("Consumed " + herbal.title, herbal);

                Game1.addHUDMessage(hudmessage);

            }

            if(herbal.level > 0)
            {

                if (applied.ContainsKey(herbal.line))
                {

                    if (applied[herbal.line].level < herbal.level)
                    {

                        applied[herbal.line].level = herbal.level;

                    }

                }
                else
                {

                    applied[herbal.line] = new();

                    applied[herbal.line].level = herbal.level;

                    applied[herbal.line].expires = Game1.timeOfDay;

                }

                for (int i = herbal.duration / 30; i > 0; i--)
                {

                    applied[herbal.line].expires += i * 30;

                    if (applied[herbal.line].expires % 100 >= 60)
                    {

                        applied[herbal.line].expires += 40;

                    }

                }

                if (applied[herbal.line].expires >= 2599)
                {

                    applied[herbal.line].expires = 2599;

                }

            }

            Mod.instance.save.herbalism[herbal.herbal] -= 1;

            HerbalBuff();

        }

        public void HerbalBuff()
        {

            if (Game1.player.buffs.IsApplied("184652"))
            {

                Game1.player.buffs.Remove("184652");

            }

            string description = "";

            int speed = 0;

            double lasts = 0;

            double latest = 0;

            for(int i = applied.Count - 1; i >= 0; i--)
            {

                KeyValuePair<herbals, HerbalBuff> herbBuff = applied.ElementAt(i);

                if (Game1.timeOfDay > herbBuff.Value.expires)
                {

                    applied.Remove(herbBuff.Key);

                    continue;

                }
                else if (herbBuff.Value.expires > latest)
                {

                    lasts = herbBuff.Value.expires - Game1.timeOfDay;

                    latest = herbBuff.Value.expires;

                }

                switch(herbBuff.Key)
                {

                    case herbals.ligna:

                        description += "Alignment " + herbBuff.Value.level.ToString() + ". ";

                        break;

                    case herbals.impes:

                        description += "Vigorous " + herbBuff.Value.level.ToString() + ". ";

                        break;

                    case herbals.celeri:

                        if (herbBuff.Value.level > 2)
                        {

                            speed = 2;

                        } 
                        else 
                        if (herbBuff.Value.level > 0)
                        {

                            speed = 1;

                        }

                        description += "Celerity "+herbBuff.Value.level.ToString() + ". ";

                        break;

                }

            }

            if (applied.Count == 0)
            {

                return;

            }

            if(lasts >= 100.00)
            {

                for (int l = (int)Math.Floor(lasts / 100); l > 0; l--)
                {

                    lasts -= 40.00;

                }

            }

            Buff herbalBuff = new(
                "184652",
                source: "Stardew Druid",
                displaySource: "Herbalism",
                duration: (int)(lasts * 1000),
                iconTexture: Mod.instance.iconData.displayTexture,
                iconSheetIndex: 5,
                displayName: "Druid Herbalism",
                description: description
                );

            if (speed > 0)
            {

                BuffEffects buffEffect = new();

                buffEffect.Speed.Set(0);

                herbalBuff.effects.Add(buffEffect);

            }

            Game1.player.buffs.Apply(herbalBuff);

        }


    }

    public class Herbal
    {

        // -----------------------------------------------
        // journal

        public HerbalData.herbals line = HerbalData.herbals.none;

        public HerbalData.herbals herbal = HerbalData.herbals.none;

        public IconData.schemes scheme = IconData.schemes.none;

        public IconData.relics container = IconData.relics.flask;

        public IconData.relics content = IconData.relics.flask1;

        public string title;

        public string description;

        public Dictionary<string, string> ingredients = new();

        public List<HerbalData.herbals> bases = new();

        public List<string> details = new();

        public int status;

        public Dictionary<string, int> amounts = new();

        public int level;

        public int duration;

        public int health;

        public int stamina;

    }

    public class HerbalBuff
    {

        // ----------------------------------------------------

        public HerbalData.herbals line = HerbalData.herbals.none;

        public double expires;

        public int level;

    }


}
