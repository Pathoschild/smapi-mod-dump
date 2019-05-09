using StardewModdingAPI;
using System;

namespace SadisticBundles
{
    class StringInjector : IAssetEditor
    {
        private IModHelper helper;
        private IMonitor monitor;

        public StringInjector(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (GameState.Current?.Activated != true) return false;

            if (asset.AssetNameEquals("Strings/UI") && !GameState.Current.LookingAtVanillaRewards) return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/UI"))
            {
                Func<string, Translation> t = helper.Translation.Get;
                var dict = asset.AsDictionary<string, string>().Data;
                foreach(var room in new string[] { "Boiler", "Crafts", "Pantry", "Vault", "FishTank" })
                {
                    var key = "JunimoNote_Reward" + room;
                    dict[key] = t(key);
                }

            }
        }
    }
}
