/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseGatherers.GreenhouseGatherers.Objects
{
    public class RecipeMail : IAssetEditor
    {
        public RecipeMail()
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;

            data["WizardHarvestStatueRecipe"] = "Enclosed you'll find blueprints for a statue imbued with forest magic.^ ^If placed indoors, it allows Junimos to enter buildings and harvest crops.^ ^Use it well...^ ^-M. Rasmodius, Wizard%item craftingRecipe Harvest_Statue %%";
        }
    }
}
