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
using Netcode;
using Newtonsoft.Json;
using StardewArchipelago.Locations.CodeInjections.Vanilla.CC;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace StardewArchipelago.Bundles
{
    public class BundlesManager
    {
        private IModHelper _modHelper;
        private static Dictionary<string, string> _vanillaBundleData;
        private Dictionary<string, string> _currentBundlesData;
        private BundleRooms BundleRooms { get; }

        public BundlesManager(IModHelper modHelper, StardewItemManager itemManager, string bundlesJson)
        {
            _modHelper = modHelper;
            var bundlesDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(bundlesJson);
            BundleRooms = new BundleRooms(itemManager, bundlesDictionary);
            _vanillaBundleData = Game1.content.LoadBase<Dictionary<string, string>>("Data\\Bundles");
            _currentBundlesData = BundleRooms.ToStardewStrings();
        }

        public void ReplaceAllBundles()
        {
            if (Game1.netWorldState.Value is not NetWorldState worldState)
            {
                throw new InvalidCastException($"World State was unexpected type: {Game1.netWorldState.GetType()}");
            }

            // private readonly NetStringDictionary<string, NetString> netBundleData = new NetStringDictionary<string, NetString>();
            var netBundleDataField = _modHelper.Reflection.GetField<NetStringDictionary<string, NetString>>(worldState, "netBundleData");
            var netBundleData = netBundleDataField.GetValue();
            netBundleData.Clear();
            worldState.Bundles.Clear();
            worldState.BundleRewards.Clear();
            worldState.BundleData.Clear();
            worldState.SetBundleData(_currentBundlesData);
        }
    }
}
