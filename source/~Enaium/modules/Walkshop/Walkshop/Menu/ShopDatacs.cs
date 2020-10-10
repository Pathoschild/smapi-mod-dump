/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using StardewValley.Menus;

namespace Walkshop
{
    public class ShopData
    {

        public string name { get; }
        public IClickableMenu iClickableMenu { get; }

        public ShopData(string name, IClickableMenu iClickableMenu)
        {
            this.name = name;
            this.iClickableMenu = iClickableMenu;
        }
    }
}