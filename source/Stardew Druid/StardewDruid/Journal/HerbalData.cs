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

        }

        public Dictionary<string, Herbal> herbalism = new();

        public Dictionary<herbals, HerbalBuff> applied = new();

        public double consumeBuffer;

        public HerbalData()
        {

            herbalism = HerbalList();

        }

        public List<List<string>> OrganiseHerbals()
        {

            List<List<string>> source = new();

            foreach (KeyValuePair<string, Herbal> pair in Mod.instance.herbalData.herbalism)
            {

                Mod.instance.herbalData.CheckHerbal(pair.Key);

                if (source.Count == 0 || source.Last().Count() == 15)
                {
                    source.Add(new List<string>());
                }

                source.Last().Add(pair.Key);

            }

            return source;

        }

        public static Dictionary<string,Herbal> HerbalList()
        {
            /*
 
                roughageItems = new()
                {
                    92, // Sap
                    766, // Slime
                    311, // PineCone
                    310, // MapleSeed
                    309, // Acorn
                    292, // Mahogany
                    767, // BatWings
                    420, // RedMushroom
                    831, // Taro Tuber
                };

                lunchItems = new()
                {
                    399, // SpringOnion
                    403, // Snackbar
                    404, // FieldMushroom
                    257, // Morel
                    281, // Chanterelle
                    152, // Seaweed
                    153, // Algae
                    157, // white Algae
                    78, // Carrot
                    227, // Sashimi
                    296, // Salmonberry
                    410, // Blackberry
                    424, // Cheese
                    24, // Parsnip
                    851, // Magma Cap
                    196, // Salad
                    349, // Tonic

                };            

             */

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

                description = "Nature, liquified and sticky",
                
                ingredients = new(){ ["(O)92"] = "Sap", ["(O)766"] = "Slime", ["Moss"] = "Moss", },

                bases = new() { },

                health = 10,

                stamina = 15,

                details = new()
                {
                    "Restores: 5 Health, 10 Stamina",
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

                description = "Like bark-root tea, with more bark, and root to boot",

                ingredients = new() { ["(O)311"] = "Acorn", ["(O)310"] = "MapleSeed", ["(O)309"] = "Pinecorn", ["(O)292"] = "MahoganySeed" },

                bases = new() { herbals.ligna, },

                health = 15,

                stamina = 30,

                details = new()
                {
                    "Restores: 15 Health, 30 Stamina",
                    "Effect: Alignment Level 1 (Increased rite power)",
                }

            };


            potions[herbals.satius_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.satius_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask3,

                title = "Satius Ligna",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.melius_ligna, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };


            potions[herbals.magnus_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.magnus_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask4,

                title = "Magnus Ligna",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.satius_ligna, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            potions[herbals.optimus_ligna.ToString()] = new()
            {

                line = HerbalData.herbals.ligna,

                herbal = HerbalData.herbals.optimus_ligna,

                scheme = IconData.schemes.Emerald,

                container = IconData.relics.flask,

                content = IconData.relics.flask5,

                title = "Optimus Ligna",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.magnus_ligna, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            // ====================================================================
            // Impes series

            potions[herbals.impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle1,

                title = "Impes",

                description = "The spiciness of the earth in a bottle",

                ingredients = new() { ["(O)399"] = "Spring Onion", ["(O)78"] = "Cave Carrot", ["(O)24"] = "Parsnip", ["(O)831"] = "Taro Tubers", ["(O)829"] = "Ginger"},

                bases = new() { },

                health = 10,

                stamina = 20,

                details = new()
                {
                    "Restores: 10 Health, 20 Stamina",
                }

            };


            potions[herbals.melius_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.melius_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle2,

                level = 1,

                duration = 60,

                title = "Melius Impes",

                description = "As close a thing to berserker mushrooms as you can get",

                ingredients = new() {  ["(O)420"] = "Field Mushrooms", ["(O)404"] = "Red Mushrooms", ["(O)257"] = "Morel", ["(O)767"] = "Batwings", },

                bases = new() { herbals.impes, },

                health = 25,

                stamina = 50,

                details = new()
                {
                    "Restores: 25 Health, 50 Stamina",
                    "Effect: Impetus Level 1 (Increased criticals, chargeups)",
                }

            };


            potions[herbals.satius_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.satius_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle3,

                title = "Satius Impes",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.melius_impes, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };


            potions[herbals.magnus_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.magnus_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle4,

                title = "Magnus Impes",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.satius_impes, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            potions[herbals.optimus_impes.ToString()] = new()
            {

                line = HerbalData.herbals.impes,

                herbal = HerbalData.herbals.optimus_impes,

                scheme = IconData.schemes.Ruby,

                container = IconData.relics.bottle,

                content = IconData.relics.bottle5,

                title = "Optimus Impes",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.magnus_impes, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            // ====================================================================
            // Celeri series

            potions[herbals.celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.celeri,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.vial,

                content = IconData.relics.vial1,

                title = "Celeri",

                description = "Good for your joints and memory retention.",

                ingredients = new() {  ["(O)129"] = "Sardine", ["(O)131"] = "Anchovy", ["(O)147"] = "Smallmouth Bass", ["(O)137"] = "Sunfish", ["(O)142"] = "Bream", ["(O)132"] = "Herring", },

                bases = new() { },

                health = 15,

                stamina = 25,

                details = new()
                {
                    "Restores: 15 Health, 25 Stamina",
                }

            };

            potions[herbals.melius_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.melius_celeri,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.vial,

                content = IconData.relics.vial2,

                level = 1,

                duration = 60,

                title = "Melius Celeri",

                description = "Now with extra fibre for good gut health",

                ingredients = new() { ["(O)152"] = "Algae", ["(O)153"] = "Seaweed", ["(O)157"] = "White Algae", ["(O)815"] = "Tea Leaves", ["(O)433"] = "Coffee Bean" },

                health = 20,

                stamina = 40,

                bases = new() { herbals.celeri, },

                details = new()
                {
                    "Restores: 30 Health, 40 Stamina",
                    "Effect: Celerity Level 1 (Movement and Casting Speed)",
                }

            };


            potions[herbals.satius_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.satius_celeri,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.vial,

                content = IconData.relics.vial3,

                title = "Satius Celeri",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },


                bases = new() { herbals.melius_celeri, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };


            potions[herbals.magnus_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.magnus_celeri,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.vial,

                content = IconData.relics.vial4,

                title = "Magnus Celeri",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.satius_celeri, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            potions[herbals.optimus_celeri.ToString()] = new()
            {

                line = HerbalData.herbals.celeri,

                herbal = HerbalData.herbals.optimus_celeri,

                scheme = IconData.schemes.Amethyst,

                container = IconData.relics.vial,

                content = IconData.relics.vial5,

                title = "Optimus Celeri",

                description = "TBC",

                ingredients = new() { ["(O)9999"] = "Nothing", },

                bases = new() { herbals.magnus_celeri, },

                details = new()
                {
                    "Restores: TBC",
                    "Ingredients: TBC",
                }

            };

            return potions;

        }

        public void CheckHerbal(string id)
        {

            herbalism[id].status = CheckInventory(id);

        }

        public int CheckInventory(string id)
        {

            Herbal herbal = herbalism[id];

            herbalism[id].amounts.Clear();

            if (Mod.instance.save.herbalism.ContainsKey(herbal.herbal))
            {

                if(Mod.instance.save.herbalism[herbal.herbal] == 99)
                {

                    return 3;

                }

            }

            if (herbal.bases.Count > 0)
            {

                foreach(herbals required in herbal.bases)
                {

                    if(!Mod.instance.save.herbalism.ContainsKey(required))
                    {

                        return 2;

                    }

                    if(Mod.instance.save.herbalism[required] == 0)
                    {

                        return 2;

                    }

                }

            }

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

                draught = Math.Min(99 - Mod.instance.save.herbalism[herbal.herbal], draught);

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

            if(herbal.level < 4)
            {

                CheckHerbal((herbal.herbal + 1).ToString());

            }

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

                        description += "Alignment " + (herbBuff.Value.level + 1).ToString() + ". ";

                        break;

                    case herbals.impes:

                        description += "Impetus " + (herbBuff.Value.level + 1).ToString() + ". ";

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

                        description += "Celerity "+(herbBuff.Value.level).ToString() + ". ";

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
