using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class BundlesService
    {
        #region Singleton Access

        private BundlesService()
        {
            GameEvents.LoadContent += OnLoadContent;
        }

        private static BundlesService _instance;

        public static BundlesService Instance => _instance ?? (_instance = new BundlesService());

        #endregion

        #region Private Data

        private readonly List<BundleInformation> _bundleInformations = new List<BundleInformation>();
        private readonly List<OverridedBundleInformation> _addedBundleItems = new List<OverridedBundleInformation>();
        private readonly List<OverridedBundleInformation> _removedBundleItems = new List<OverridedBundleInformation>();

        #endregion

        #region	Public Methods

        public void AddBundleItems(OverridedBundleInformation bundle)
        {
            _addedBundleItems.Add(bundle);
        }

        public void RemoveBundleItems(OverridedBundleInformation bundle)
        {
            _removedBundleItems.Add(bundle);
        }

        #endregion

        #region	Auxiliary Methods

        private Dictionary<string, string> LoadGameBundles()
        {
            Game1.content.Load<Dictionary<string, string>>(@"Data\Bundles");
            var loadedAssets = Game1.content.GetField<Dictionary<string, object>>("loadedAssets");
            var bundles = (Dictionary<string, string>)loadedAssets[@"Data\Bundles"];

            foreach (var kv in bundles)
            {
                _bundleInformations.Add(BundleInformation.Parse(kv.Value, kv.Key));
            }

            return bundles;
        }

        private void RemoveBundleItems()
        {
            foreach (var removedBundle in _removedBundleItems)
            {
                var bundle = _bundleInformations.First(b => b.Key == removedBundle.Key);

                foreach (var bundleItem in removedBundle.Items)
                {
                    var index = bundle.Items.FindIndex(bi => bi.ID == bundleItem.ID);
                    if (index != -1)
                    {
                        bundle.Items[index] = null;
                    }
                }
            }
        }

        private void AddBundleItems()
        {
            foreach (var addedBundle in _addedBundleItems)
            {
                var bundle = _bundleInformations.First(b => b.Key == addedBundle.Key);

                foreach (var bundleItem in addedBundle.Items)
                {
                    var index = bundle.Items.FindIndex(bi => bi == null);
                    if (index != -1)
                    {
                        bundle.Items[index] = bundleItem;
                    }
                    else
                    {
                        bundle.Items.Add(bundleItem);
                    }
                }    
            }
        }

        private void ClearEmptyValues()
        {
            var bundles = _bundleInformations.ToList();

            foreach (var bundle in bundles)
            {
                var items = bundle.Items.Where(i => i != null).ToList();
                bundle.Items.Clear();
                bundle.Items.AddRange(items);
            }
        }

        private void ApplyValues(IDictionary<string, string> bundles)
        {
            foreach (var bundle in _bundleInformations)
            {
                bundles[bundle.Key] = bundle.ToString();
            }
        }

        private void OnLoadContent(object sender, EventArgs e)
        {
            var bundles = LoadGameBundles();
            RemoveBundleItems();
            AddBundleItems();
            ClearEmptyValues();
            ApplyValues(bundles);          
        }

        #endregion
    }
}
