/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comics
{
    public class ComicsModAPI : IComicsModAPI
    {
        public static bool PlaceShop { get; set; } = true;

        public static string Shopkeeper { get; set; } = "Pierre";

        public static string ShopText { get; set; } = "";

        public void PreventShopPlacement()
        {
            PlaceShop = false;
        }

        public void SetShopKeeper(string name)
        {
            Shopkeeper = name;
        }

        public void SetShopText(string text)
        {
            ShopText = text;
        }
    }
}
