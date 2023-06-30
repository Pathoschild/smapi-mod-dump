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
using System.Text;
using System.Threading.Tasks;
using StardewArchipelago.Constants;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Archipelago
{
    public class BundlesManager
    {
        private static Dictionary<string, string> _vanillaBundleData;
        private Dictionary<string, string> _modifiedBundlesData;

        public BundlesManager(Dictionary<string, string> modifiedBundlesData)
        {
            _vanillaBundleData = null;
            _modifiedBundlesData = modifiedBundlesData;
        }

        public void ReplaceAllBundles()
        {
            if (_vanillaBundleData == null)
            {
                _vanillaBundleData = Game1.content.LoadBase<Dictionary<string, string>>("Data\\Bundles");
            }

            Game1.netWorldState.Value.SetBundleData(_vanillaBundleData);
            foreach (var (bundleKey, newBundleData) in _modifiedBundlesData)
            {
                var oldBundle = Game1.netWorldState.Value.BundleData[bundleKey];
                var oldBundleName = oldBundle.Split("/")[0];
                var newBundleName = newBundleData.Split("/")[0];
                CommunityCenterInjections.BundleNames.Add(newBundleName, oldBundleName);
                Game1.netWorldState.Value.BundleData[bundleKey] = newBundleData;
            }
        }
    }
}
