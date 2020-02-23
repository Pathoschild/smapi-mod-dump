using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class AllCrops : Items, ISpecificItems
    {
        public AllCrops() : base()
        {
            AddItems();
        }

        protected override void AddItems()
        {
            AddAllCropsAndSaplings();
        }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        private void AddAllCropsAndSaplings()
        {
            Dictionary<int, string> cropData = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            Constants constants = new Constants();
            foreach (KeyValuePair<int, string> data in cropData)
            {
                if (!constants.RANDOM_SEASON_SEEDS.Contains(data.Key))
                {
                    string[] crop = data.Value.Split('/');
                    AddOneCommonObject(int.Parse(crop[3]));
                }
            }

            Dictionary<int, string> fruitTreesData = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
            foreach (KeyValuePair<int, string> data in fruitTreesData)
            {
                string[] fruitTree = data.Value.Split('/');
                AddOneCommonObject(int.Parse(fruitTree[2]));
            }
        }
    }
}
