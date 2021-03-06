/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/


namespace StardewTaxes.Framework.Data
{
    internal class PlayerData
    {
        /*
         *
         * Totals
         *
         */
        
        //The total amount of items the player has shipped
        private int TotalItemsShipped { get; set; } = 0;

        //The total amount of items sold to shops
        private int TotalItemsSoldToShops { get; set; } = 0;

        //The total amount of items bought from shops
        private int TotalItemsBoughtFromShops { get; set; } = 0;

        //private int TotalItemsSoldToPierre { get; set; } = 0;

        //private int TotalItemsSoldToClint { get; set; } = 0;


        /*
         *
         * Stuff used for commodity. Will be used as a system similar to a stock market.
         *
         */

        //Crops that are needed at any time.
        private int[] CropsInNeeded { get; set; } = { 0};

        //Crops that are sold over sold.
        private int[] CropsNotInNeed { get; set; } = {0};

        /*
         *
         * Stuff for taxes
         *
         */

        //The current amount of taxes owed.
        private int CurrentTaxesOwed { get; set; } = 0;
        //The current amount of taxes paid back during a tax season. Will be used to keep track of how much is still owed.
        private int CurrentlyPaidTaxes { get; set; } = 0;

        //Current Tax percentage. Example 5 would equal 5%
        private int TaxPercentage { get; set; } = 0;

        //Amount paid in sales tax, calculated each time an item is sold or bought.
        private int SalesTaxPaid { get; set; } = 0;
    }
}
