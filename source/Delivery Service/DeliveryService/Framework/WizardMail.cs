using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace DeliveryService.Framework
{
    public class WizardMail : IAssetEditor
    {
        public WizardMail()
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;

            // "MyModMail1" is referred to as the mail Id.  It is how you will uniquely identify and reference your mail.
            // The @ will be replaced with the player's name.  Other items do not seem to work (i.e. %pet or %farm)
            // %item object 388 50 %%   - this adds 50 pieces of wood when added to the end of a letter.
            // %item money 250 601  %%  - this sends a random amount of gold from 250 to 601 inclusive.
            // %item cookingRecipe %%   - this is for recipes (did not try myself)  Not sure how it know which recipe. 
            data["DeliveryServiceWizardMail"] = "Hello @... ^I have noticed that the Junimos have been rummaging around in my chests.  I've been doing some experiments, and believe we can take advantage of this.  If you place these runes on your chests, the Junimos should deliver items between chests while you sleep.^^    - M";
        }
    }
}
