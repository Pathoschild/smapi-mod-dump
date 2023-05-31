/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    /// <summary>
    /// Utilities that deals with the player's farm.
    /// </summary>
    public static class FarmUtilities
    {

        /// <summary>
        /// Gets the capacity from the silos for how many pieces of hay can be stored on the farm.
        /// </summary>
        /// <returns></returns>
        public static int GetSiloCapacity()
        {
            return Utility.numSilos() * 240;
        }

        /// <summary>
        /// Gets how much more hay the silos on the farm can store.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfHayPiecesUntilFullSilos()
        {
            return GetSiloCapacity() - (int)Game1.getFarm().piecesOfHay.Value;
        }

        /// <summary>
        /// Returns if the silos are at max capacity.
        /// </summary>
        /// <returns></returns>
        public static bool AreSilosAtMaxCapacity()
        {
            return GetNumberOfHayPiecesUntilFullSilos() == 0;
        }

        /// <summary>
        /// Gets the maximum number of hay pieces that can be bought to fill the silos.
        /// </summary>
        /// <param name="PricePerHayPiece"></param>
        /// <returns></returns>
        public static int GetNumberOfHayPiecesUntilFullSilosLimitByPlayersMoney(int PricePerHayPiece)
        {
            int maxHay = GetNumberOfHayPiecesUntilFullSilos();
            int money = Game1.player.Money;

            int maxTotalHayPrice = maxHay * PricePerHayPiece;
            if (maxTotalHayPrice > money)
            {
                maxHay = money / PricePerHayPiece;
            }

            return maxHay;
        }

        /// <summary>
        /// Actually refils the silos on the farm from the silo refil item.
        /// </summary>
        /// <param name="PricePerHayPiece"></param>
        public static void FillSilosFromSiloReillItem(int PricePerHayPiece)
        {
            Game1.getFarm().tryToAddHay(GetNumberOfHayPiecesUntilFullSilosLimitByPlayersMoney(PricePerHayPiece));
        }

    }
}
