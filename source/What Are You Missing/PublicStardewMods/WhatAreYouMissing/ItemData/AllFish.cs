/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

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
