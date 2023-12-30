/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Bundles
{
    public abstract class Bundle
    {
        protected const string NUMBER_REQUIRED_KEY = "number_required";
        private const string BUNDLE_SUFFIX = " Bundle";
        private string _name;

        public string RoomName { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameWithoutBundle = _name.EndsWith(BUNDLE_SUFFIX) ? _name[..^BUNDLE_SUFFIX.Length] : _name;
            }
        }

        public string NameWithoutBundle { get; private set; }

        public int SpriteIndex => BundleIndexes.BundleSpriteIndexes[NameWithoutBundle];
        public int ColorIndex => BundleIndexes.BundleColorIndexes[NameWithoutBundle];

        public Bundle(string roomName, string bundleName)
        {
            RoomName = roomName;
            Name = bundleName;
        }

        public static Bundle Parse(StardewItemManager itemManager, string name, string bundleName, Dictionary<string, string> bundleContent)
        {
            if (bundleContent.Count() == 2 && bundleContent.Values.Any(x => CurrencyBundle.CurrencyIds.Keys.Contains(x.Split("|")[0])))
            {
                return new CurrencyBundle(name, bundleName, bundleContent);
            }

            return new ItemBundle(itemManager, name, bundleName, bundleContent);
        }

        public abstract string GetItemsString();
        public abstract string GetNumberRequiredItemsWithSeparator();
    }
}
