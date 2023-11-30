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
        private Dictionary<int, string> _bundleIdToName;
        private Dictionary<string, int> _bundleNameToId;
        private Dictionary<Area, List<int>> _areaToBundles;
        private Dictionary<int, Area> _bundleToArea;

        public BundleReader()
        {
            InitAreaBundleConversions();
        }

        public List<string> GetAllCompletedBundles()
        {
            var communityCenter = Game1.locations.OfType<CommunityCenter>().First();
            var completedBundles = new List<string>();
            foreach (var (bundleId, bundleName) in _bundleIdToName)
            {
                var isCompleted = IsBundleComplete(communityCenter, bundleId);
                if (isCompleted)
                {
                    completedBundles.Add(bundleName);
                }
            }

            return completedBundles;
        }

        private bool IsBundleComplete(CommunityCenter communityCenter, int bundleId)
        {
            if (_bundleToArea[bundleId] == Area.Vault)
            {
                return communityCenter.bundles[bundleId][0];
            }

            return communityCenter.isBundleComplete(bundleId);
        }

        public Area GetAreaOfBundle(int bundleId)
        {
            return _bundleToArea.ContainsKey(bundleId) ? _bundleToArea[bundleId] : Area.None;
        }

        public List<int> GetBundlesInArea(Area area)
        {
            return _areaToBundles.ContainsKey(area) ? _areaToBundles[area] : new List<int>();
        }

        private void InitAreaBundleConversions()
        {
            _bundleIdToName = new Dictionary<int, string>();
            _bundleNameToId = new Dictionary<string, int>();
            _areaToBundles = new Dictionary<Area, List<int>>();
            _bundleToArea = new Dictionary<int, Area>();
            foreach (var area in Enum.GetValues<Area>())
            {
                _areaToBundles.Add(area, new List<int>());
            }

            foreach (var keyValuePair in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = keyValuePair.Key.Split('/');
                var splitValue = keyValuePair.Value.Split('/');

                var areaName = splitKey[0];
                var area = (Area)CommunityCenter.getAreaNumberFromName(areaName);
                var bundleId = Convert.ToInt32(splitKey[1]);

                var bundleName = splitValue[0];

                _bundleIdToName.Add(bundleId, bundleName);
                _bundleNameToId.Add(bundleName, bundleId);
                _areaToBundles[area].Add(bundleId);
                _bundleToArea.Add(bundleId, area);
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