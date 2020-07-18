using StardewValley;
using System.Collections.Generic;

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
            __result.Clear();
            __result.AddRange(ModEntry.Instance.Api.GetAllBuyableAnimalObjects());
        }
    }
}

