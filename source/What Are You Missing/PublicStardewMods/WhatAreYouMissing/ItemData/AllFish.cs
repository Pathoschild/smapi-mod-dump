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


            //add option to only show uncaught fish
            AddFishBasedOnConfig(Constants.CRIMSONFISH);
            AddFishBasedOnConfig(Constants.ANGLER);
            AddFishBasedOnConfig(Constants.GLACIERFISH);
            AddFishBasedOnConfig(Constants.MUTANT_CARP);

            AddFishBasedOnConfig(Constants.MIDNIGHT_SQUID);
            AddFishBasedOnConfig(Constants.SPOOK_FISH);
            AddFishBasedOnConfig(Constants.BLOBFISH);

            AddFishBasedOnConfig(Constants.STONEFISH);
            AddFishBasedOnConfig(Constants.ICE_PIP);
            AddFishBasedOnConfig(Constants.LAVA_EEL);
        }

        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        private void AddAllFish()
        {
            Dictionary<string, string> LocationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            foreach (KeyValuePair<string, string> data in LocationData)
            {
                for (int season = (int)SeasonIndex.Spring; !Utilities.IsTempOrFishingGameOrBackwoodsLocation(data.Key) && season < (int)SeasonIndex.Winter + 1; ++season)
                {
                    string[] seasonalFish = data.Value.Split('/')[season].Split(' ');
                    for (int i = 0; i < seasonalFish.Length; ++i)
                    {
                        if (i % 2 == 0)
                        {
                            //Its a parent sheet index
                            bool successful = int.TryParse(seasonalFish[i], out int parentSheetIndex);
                            if (!successful)
                            {
                                ModEntry.Logger.LogFishIndexError(data.Value.Split('/')[season], seasonalFish[i], i);
                                continue;
                            }

                            //I want to add them manually, -1 means no fish at this location
                            if (IsAFish(parentSheetIndex))
                            {
                                AddFishBasedOnConfig(parentSheetIndex);
                            }
                        }
                    }
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
