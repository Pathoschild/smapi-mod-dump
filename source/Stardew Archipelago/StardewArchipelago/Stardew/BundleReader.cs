/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Stardew
{
    public class BundleReader
    {
        //private Dictionary<int, string> _bundleIdToName;
        //private Dictionary<string, int> _bundleNameToId;
        //private Dictionary<Area, List<int>> _areaToBundles;
        //private Dictionary<int, Area> _bundleToArea;

        public BundleReader()
        {
        }

        public bool IsCommunityCenterComplete()
        {
            var communityCenter = GetCommunityCenter();
            return communityCenter.areAllAreasComplete();
        }

        public List<string> GetAllCompletedBundles()
        {
            var communityCenter = GetCommunityCenter();
            var completedBundles = new List<string>();
            foreach (var (key, bundleData) in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = key.Split('/');
                var bundleId = Convert.ToInt32(splitKey[1]);
                var isCompleted = IsBundleComplete(communityCenter, bundleId);
                if (isCompleted)
                {
                    var bundleName = bundleData.Split("/").First();
                    completedBundles.Add(bundleName);
                }
            }

            return completedBundles;
        }

        private Area GetAreaNumberFromId(int desiredBundleId)
        {
            foreach (var (key, bundleName) in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = key.Split('/');
                var bundleId = Convert.ToInt32(splitKey[1]);
                if (bundleId == desiredBundleId)
                {
                    var areaName = splitKey[0];
                    return (Area)CommunityCenter.getAreaNumberFromName(areaName);
                }
            }

            throw new ArgumentException($"Failed in {nameof(GetAreaNumberFromId)}: Could not find a bundle with id {desiredBundleId}");
        }

        private CommunityCenter GetCommunityCenter()
        {
            return Game1.locations.OfType<CommunityCenter>().First();
        }

        private bool IsBundleComplete(CommunityCenter communityCenter, int bundleId)
        {
            if (GetAreaNumberFromId(bundleId) == Area.Vault)
            {
                return communityCenter.bundles[bundleId][0];
            }

            return communityCenter.isBundleComplete(bundleId);
        }
    }

    public enum Area
    {
        None = -1,

        // Community Center
        Pantry = 0,
        CraftsRoom = 1,
        FishTank = 2,
        BoilerRoom = 3,
        Vault = 4,
        Bulletin = 5,

        // Other
        AbandonedJojaMart = 6,
        Bulletin2 = 7,
        JunimoHut = 8,
    }
}
