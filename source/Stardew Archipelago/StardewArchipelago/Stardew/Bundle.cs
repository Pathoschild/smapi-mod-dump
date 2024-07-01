/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Stardew
{
    public class Bundle
    {
        public int BundleId { get; }
        public string BundleName { get; }
        public string BundleReward { get; }
        public string BundleItemsRequired { get; }
        public string BundleColor { get; }

        public Bundle(int bundleId, string bundleName, string bundleReward, string bundleItemsRequired, string bundleColor)
        {
            BundleId = bundleId;
            BundleName = bundleName;
            BundleReward = bundleReward;
            BundleItemsRequired = bundleItemsRequired;
            BundleColor = bundleColor;
        }
    }
}
