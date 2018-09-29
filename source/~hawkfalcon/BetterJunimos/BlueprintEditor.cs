using System.Collections.Generic;
using BetterJunimos.Utils;
using StardewModdingAPI;

namespace BetterJunimos {
    public class BlueprintEditor : IAssetEditor {
        
        public bool CanEdit<T>(IAssetInfo asset) {
            if (Util.Config.JunimoHuts.ReducedCostToConstruct || Util.Config.JunimoHuts.FreeToConstruct) {
                return asset.AssetNameEquals(@"Data/Blueprints");
            }
            return false;
        }

        public void Edit<T>(IAssetData asset) {
            IDictionary<string, string> blueprints = asset.AsDictionary<string, string>().Data;

            if (Util.Config.JunimoHuts.FreeToConstruct) {
                blueprints["Junimo Hut"] = "/3/2/-1/-1/-2/-1/null/Junimo Hut/Junimos will harvest crops around the hut for you./Buildings/none/48/64/-1/null/Farm/0/true";
            }
            else if (Util.Config.JunimoHuts.ReducedCostToConstruct) {
                blueprints["Junimo Hut"] = "390 100 268 3 771 100/3/2/-1/-1/-2/-1/null/Junimo Hut/Junimos will harvest crops around the hut for you./Buildings/none/48/64/-1/null/Farm/10000/true";
            } 
        }
    }
}
