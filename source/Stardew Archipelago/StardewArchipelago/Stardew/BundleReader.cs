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
        private Dictionary<int, Bundle> _bundleDictionary;
        private Dictionary<Area, List<Bundle>> _areaToBundleDictionary;
        private Dictionary<Bundle, Area> _bundleToAreaDictionary;

        public BundleReader()
        {
            InitAreaBundleConversions();
        }

        public List<StateBundle> ReadCurrentBundleStates()
        {
            var communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            var stateBundles = new List<StateBundle>();
            foreach (var (bundleId, bundle) in _bundleDictionary)
            {
                var isCompleted = IsBundleComplete(communityCenter, bundleId, bundle.BundleName);
                var stateBundle = new StateBundle(bundle, isCompleted);
                stateBundles.Add(stateBundle);
            }

            return stateBundles;
        }

        private bool IsBundleComplete(CommunityCenter communityCenter, int bundleId, string bundleName)
        {
            if (bundleName.EndsWith("00g"))
            {
                return communityCenter.bundles[bundleId][0];
            }

            return communityCenter.isBundleComplete(bundleId);
        }

        public Area GetAreaOfBundle(int bundleId)
        {
            return _bundleDictionary.ContainsKey(bundleId) ? GetAreaOfBundle(_bundleDictionary[bundleId]) : Area.None;
        }

        public Area GetAreaOfBundle(Bundle bundle)
        {
            return _bundleToAreaDictionary.ContainsKey(bundle) ? _bundleToAreaDictionary[bundle] : Area.None;
        }

        public List<Bundle> GetBundlesInArea(Area area)
        {
            return _areaToBundleDictionary.ContainsKey(area) ? _areaToBundleDictionary[area] : new List<Bundle>();
        }

        private void InitAreaBundleConversions()
        {
            _bundleDictionary = new Dictionary<int, Bundle>();
            _areaToBundleDictionary = new Dictionary<Area, List<Bundle>>();
            _bundleToAreaDictionary = new Dictionary<Bundle, Area>();
            foreach (var area in Enum.GetValues<Area>())
            {
                _areaToBundleDictionary.Add(area, new List<Bundle>());
            }

            foreach (var keyValuePair in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = keyValuePair.Key.Split('/');
                var splitValue = keyValuePair.Value.Split('/');

                var areaName = splitKey[0];
                var area = (Area)CommunityCenter.getAreaNumberFromName(areaName);
                var bundleId = Convert.ToInt32(splitKey[1]);

                var bundleName = splitValue[0];
                var bundleReward = splitValue[1];
                var bundleItemsRequired = splitValue[2];
                var bundleColor = splitValue[2];

                var bundle = new Bundle(bundleId, bundleName, bundleReward, bundleItemsRequired, bundleColor);

                _bundleDictionary.Add(bundleId, bundle);
                _areaToBundleDictionary[area].Add(bundle);
                _bundleToAreaDictionary.Add(bundle, area);
            }
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