/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;
using BetterJunimos.Utils;
using StardewModdingAPI;

namespace BetterJunimos {
    public class BlueprintEditor : IAssetEditor {
        
        public bool CanEdit<T>(IAssetInfo asset) {
            if (Util.Progression.ReducedCostToConstruct || Util.Config.JunimoHuts.FreeToConstruct) {
                return asset.AssetNameEquals(@"Data/Blueprints");
            }
            return false;
        }

        public void Edit<T>(IAssetData asset) {
            IDictionary<string, string> blueprints = asset.AsDictionary<string, string>().Data;

            if (Util.Config.JunimoHuts.FreeToConstruct) {
                blueprints["Junimo Hut"] = "/3/2/-1/-1/-2/-1/null/Junimo Hut/Junimos will harvest crops around the hut for you./Buildings/none/48/64/-1/null/Farm/0/true";
            }
            else if (Util.Progression.ReducedCostToConstruct) {
                blueprints["Junimo Hut"] = "390 100 268 3 771 100/3/2/-1/-1/-2/-1/null/Junimo Hut/Junimos will harvest crops around the hut for you./Buildings/none/48/64/-1/null/Farm/10000/true";
            } 
        }
    }
}
