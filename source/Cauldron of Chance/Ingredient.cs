/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WizardsLizards/CauldronOfChance
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CauldronOfChance
{
    public class Ingredient
    {
        public Item item;

        #region special events
        public double butterflies { get; set; } = 0;
        public double boom { get; set; } = 0;
        public int cooking { get; set; } = 0;
        public double cauldronLuck { get; set; } = 0;
        public double duration { get; set; } = 1;
        #endregion special events

        public List<int> buffList;
        public List<int> debuffList;

        public Ingredient (Item item, int butterfliesChance = 0, int boomChance = 0, int garlicOil = 0, int monsterMusk = 0, int debuffImmunity = 0,
            int farming = 0, int mining = 0, int fishing = 0, int foraging = 0, int attack = 0, int defense = 0,
            int maxEnergy = 0, int luck = 0, int magneticRadius = 0, int speed = 0, int cauldronLuck = 0, int duration = 0)
        {
            buffList = new List<int>();
            foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
            {
                buffList.Add(0);
            }
            debuffList = new List<int>();
            foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
            {
                debuffList.Add(0);
            }

            this.item = item;

            if (butterfliesChance > 0)
            {
                this.butterflies += butterfliesChance * Cauldron.butterfliesConst;
            }
            if (boomChance > 0)
            {
                this.boom += boomChance * Cauldron.boomConst;
            }

            this.cauldronLuck += cauldronLuck * Cauldron.cauldronLuckConst;
            this.duration += duration * Cauldron.durationConst;

            using (Cauldron Cauldron = new Cauldron(this))
            {
                Cauldron.addToCauldron(nameof(garlicOil), garlicOil);
                Cauldron.addToCauldron(nameof(monsterMusk), monsterMusk);
                Cauldron.addToCauldron(nameof(debuffImmunity), debuffImmunity);
                Cauldron.addToCauldron(nameof(farming), farming);
                Cauldron.addToCauldron(nameof(mining), mining);
                Cauldron.addToCauldron(nameof(fishing), fishing);
                Cauldron.addToCauldron(nameof(foraging), foraging);
                Cauldron.addToCauldron(nameof(attack), attack);
                Cauldron.addToCauldron(nameof(defense), defense);
                Cauldron.addToCauldron(nameof(maxEnergy), maxEnergy);
                Cauldron.addToCauldron(nameof(luck), luck);
                Cauldron.addToCauldron(nameof(magneticRadius), magneticRadius);
                Cauldron.addToCauldron(nameof(speed), speed);

                addForCategory(Cauldron);
                addForFoodBuffs(Cauldron);
                addForQuality();
                addForEdibility();
                addForMoneyValue();
            }
        }

        public void addForCategory(Cauldron Cauldron)
        {
            switch (item.Category)
            {
                case StardewValley.Object.artisanGoodsCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.baitCategory:
                    Cauldron.addToCauldron("fishing", 1);
                    break;
                case StardewValley.Object.CookingCategory:
                    Cauldron.addToCauldron("maxEnergy", 1);
                    break;
                case StardewValley.Object.CraftingCategory:
                    Cauldron.addToCauldron("defense", 1);
                    break;
                case StardewValley.Object.EggCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.fertilizerCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.FishCategory:
                    Cauldron.addToCauldron("fishing", 1);
                    break;
                case StardewValley.Object.flowersCategory:
                    Cauldron.addToCauldron("foraging", 1);
                    break;
                case StardewValley.Object.FruitsCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.GemCategory:
                    Cauldron.addToCauldron("mining", 1);
                    break;
                case StardewValley.Object.GreensCategory:
                    Cauldron.addToCauldron("foraging", 1);
                    break;
                case StardewValley.Object.ingredientsCategory:
                    Cauldron.addToCauldron("maxEnergy", 1);
                    break;
                case StardewValley.Object.junkCategory:
                    this.boom += 0.2;
                    break;
                case StardewValley.Object.MilkCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.mineralsCategory:
                    Cauldron.addToCauldron("mining", 1);
                    break;
                case StardewValley.Object.monsterLootCategory:
                    Cauldron.addToCauldron("attack", 1);
                    break;
                case StardewValley.Object.SeedsCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.syrupCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
                case StardewValley.Object.tackleCategory:
                    Cauldron.addToCauldron("fishing", 1);
                    break;
                case StardewValley.Object.VegetableCategory:
                    Cauldron.addToCauldron("farming", 1);
                    break;
            }
        }

        public void addForFoodBuffs(Cauldron Cauldron)
        {
            if(item is StardewValley.Object)
            {
                StardewValley.Object csObject = item as StardewValley.Object;

                string[] objectDescription = Game1.objectInformation[csObject.ParentSheetIndex].Split('/');

                if (Convert.ToInt32(objectDescription[2]) > 0)
                {
                    string[] whatToBuff = (string[])((objectDescription.Length > 7) ? ((object)objectDescription[7].Split(' ')) : ((object)new string[12]
                    {
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                        "0", "0"
                    }));

                    csObject.ModifyItemBuffs(whatToBuff);

                    //Buff buff = new Buff(Convert.ToInt32(whatToBuff[0]), Convert.ToInt32(whatToBuff[1]), Convert.ToInt32(whatToBuff[2]), Convert.ToInt32(whatToBuff[3]), Convert.ToInt32(whatToBuff[4]), Convert.ToInt32(whatToBuff[5]), Convert.ToInt32(whatToBuff[6]), Convert.ToInt32(whatToBuff[7]), Convert.ToInt32(whatToBuff[8]), Convert.ToInt32(whatToBuff[9]), Convert.ToInt32(whatToBuff[10]), (whatToBuff.Length > 11) ? Convert.ToInt32(whatToBuff[11]) : 0, duration, objectDescription[0], objectDescription[4]);

                    Cauldron.addToCauldron("farming", Convert.ToInt32(whatToBuff[0]));
                    Cauldron.addToCauldron("mining", Convert.ToInt32(whatToBuff[2]));
                    Cauldron.addToCauldron("fishing", Convert.ToInt32(whatToBuff[1]));
                    Cauldron.addToCauldron("foraging", Convert.ToInt32(whatToBuff[5]));
                    Cauldron.addToCauldron("attack", (whatToBuff.Length > 11) ? Convert.ToInt32(whatToBuff[11]) : 0);
                    Cauldron.addToCauldron("defense", Convert.ToInt32(whatToBuff[10]));
                    Cauldron.addToCauldron("maxEnergy", Convert.ToInt32(whatToBuff[7]) / 10);
                    Cauldron.addToCauldron("luck", Convert.ToInt32(whatToBuff[4]));
                    Cauldron.addToCauldron("magneticRadius", Convert.ToInt32(whatToBuff[8]) / 32);
                    Cauldron.addToCauldron("speed", Convert.ToInt32(whatToBuff[9]));
                }
            }
        }

        public void addForQuality()
        {
            if (item is StardewValley.Object)
            {
                StardewValley.Object csObject = item as StardewValley.Object;

                cauldronLuck += csObject.Quality * 0.05;
            }
        }

        public void addForEdibility()
        {
            if (item is StardewValley.Object)
            {
                StardewValley.Object csObject = item as StardewValley.Object;

                if(csObject.Edibility > -300)
                {
                    cauldronLuck += (csObject.Edibility / 100) * 0.05;
                }
            }
        }
        
        public void addForMoneyValue()
        {
            cauldronLuck += (item.salePrice() / 1000) * 0.02;
        }


        public int getBuffChance()
        {
            int buffChance = 0;

            foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
            {
                buffChance += buffList[buffIndex];
            }

            return buffChance;
        }

        public int getDebuffChance()
        {
            int debuffChance = 0;

            foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
            {
                debuffChance += debuffList[debuffIndex];
            }

            return debuffChance;
        }
    }
}