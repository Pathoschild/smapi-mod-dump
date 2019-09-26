using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class HighestQuality
    {
        private List<int> IridiumQualityFlowers;
        private List<int> GoldQualityFruits;

        public HighestQuality()
        {
            IridiumQualityFlowers = new List<int>();
            GoldQualityFruits = new List<int>();

            Initialize();
        }

        private void Initialize()
        {
            AddIridiumFlowers();
            AddGoldFruits();
        }

        private void AddIridiumFlowers()
        {
            IridiumQualityFlowers.Add(Constants.SWEET_PEA);
            IridiumQualityFlowers.Add(Constants.CROCUS);
        }

        private void AddGoldFruits()
        {
            GoldQualityFruits.Add(Constants.ANCIENT_FRUIT);
            GoldQualityFruits.Add(Constants.BLUEBERRY);
            GoldQualityFruits.Add(Constants.CRANBERRIES);
            GoldQualityFruits.Add(Constants.HOT_PEPPER);
            GoldQualityFruits.Add(Constants.MELON);
            GoldQualityFruits.Add(Constants.RHUBARB);
            GoldQualityFruits.Add(Constants.STARFRUIT);
            GoldQualityFruits.Add(Constants.STRAWBERRY);
        }

        public int GetHighestQualityForItem(int parentSheetIndex)
        {
            int category = Convert.ToInt32(Game1.objectInformation[parentSheetIndex].Split('/')[3].Split(' ')[1]);

            switch (category)
            {
                case SObject.flowersCategory:
                    return IridiumQualityFlowers.Contains(parentSheetIndex) ? Constants.IRIDIUM_QUALITY : Constants.GOLD_QUALITY;
                case SObject.FruitsCategory:
                    return !GoldQualityFruits.Contains(parentSheetIndex) ? Constants.IRIDIUM_QUALITY : Constants.GOLD_QUALITY;
                case SObject.VegetableCategory:
                    return parentSheetIndex == Constants.FIDDLEHEAD_FERN ? Constants.IRIDIUM_QUALITY : Constants.GOLD_QUALITY;
                case SObject.FishCategory:
                    return Constants.GOLD_QUALITY;
                case SObject.GreensCategory:
                    //This is foragables
                    return parentSheetIndex != Constants.CAVE_CARROT ? Constants.IRIDIUM_QUALITY : Constants.COMMON_QUALITY;
                case SObject.SeedsCategory:
                    //For the sake of this mod the only seed that will be passed in here
                    //is the coffee bean and its highest quality is gold
                    return Constants.GOLD_QUALITY;
                default:
                    //Catches the Truffle
                    return Constants.IRIDIUM_QUALITY;
            }
        }
    }
}
