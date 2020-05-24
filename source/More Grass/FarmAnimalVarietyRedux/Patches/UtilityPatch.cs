using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Utility"/> class.</summary>
    internal class UtilityPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the GetPurchaseAnimalStock method.</summary>
        /// <param name="__result">The return value of the method.</param>
        internal static void GetPurchaseAnimalStockPostFix(List<StardewValley.Object> __result)
        {
            foreach (var animal in ModEntry.Instance.Api.GetAllBuyableAnimals())
            {
                if (animal.Data.AnimalShopInfo == null)
                    continue;

                Object animalObject = new Object(100, 1, false, animal.Data.AnimalShopInfo.BuyPrice, 0);
                animalObject.Name = animal.Name;
                animalObject.displayName = animal.Name;

                // construct buildings string and check if the animal has a valid home
                var buildingsString = "";

                bool hasValidHome = false;
                for (int i = 0; i < animal.Data.Buildings.Count; i++)
                {
                    var building = animal.Data.Buildings[i];
                    buildingsString += building;
                    if (i != animal.Data.Buildings.Count - 1)
                        buildingsString += ", ";

                    if (Game1.getFarm().isBuildingConstructed(building))
                    {
                        hasValidHome = true;
                        break;
                    }
                }

                if (!hasValidHome)
                    animalObject.Type = $"Requires construction of at least one of the following buildings: {buildingsString}";
                else
                    animalObject.Type = null;

                __result.Add(animalObject);
            }
        }
    }
}

