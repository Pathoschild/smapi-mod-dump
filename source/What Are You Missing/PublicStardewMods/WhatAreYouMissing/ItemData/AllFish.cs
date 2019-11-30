using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
using StardewValley;

namespace WhatAreYouMissing
{
    public class AllFish : Items, ISpecificItems
    {
        public AllFish() : base() { }

        protected override void AddItems()
        {
            AddAllFish();
        }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        private void AddAllFish()
        {
            Dictionary<int, string> FishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");

            foreach (KeyValuePair<int, string> data in FishData)
            {
                if (IsAFish(data.Key))
                {
                    AddFishBasedOnConfig(data.Key);
                }
            }
        }

        private void AddFishBasedOnConfig(int id)
        {
            if (!Config.DoNotShowCaughtFish)
            {
                //Add the fish regardless of if its been caught or not
                AddOneCommonObject(id);
            }
            else if (!IsFishAlreadyCaught(id))
            {
                //only add it if it hasn't been caught yet
                AddOneCommonObject(id);
            }
        }
    }
}
