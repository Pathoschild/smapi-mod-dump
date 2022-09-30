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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Omegasis.Revitalize.Framework.Configs.ShopConfigs
{
    /// <summary>
    /// A config file used for adjusting the price and location of the hay maker shop used for buying Hay from Marnies ranch whenever.
    /// </summary>
    public class HayMakerShopConfig
    {

        /// <summary>
        /// Is the hay maker for buying hay displayed outside of marnies ranch? If false, then the hay maker shop will not appear in the game.
        /// </summary>
        public bool IsHayMakerShopSetUpOutsideOfMarniesRanch = true;
        /// <summary>
        /// The tile location where the permanent hay maker shop should be set up in the forest. Used on the off chance another mod maker changes the Forest map.
        /// </summary>
        public Vector2 HayMakerTileLocation = new Vector2(81, 14);


        /// <summary>
        /// The amount of gold it costs to buy a single piece of hay.
        /// </summary>
        public int HayMakerShopHaySellPrice = 60;

        /// <summary>
        /// This value should be true if the top of the hay maker is up against a wall, such as when it is in teh default position outside of marnie's silo. This bool effectively shortens the bounding box on it to ensure proper collision handling. Disable this to reenable the collision handling.
        /// </summary>
        public bool IsHayMakerShopUpAgainstAWall = true;

        public HayMakerShopConfig()
        {

        }

    }
}
