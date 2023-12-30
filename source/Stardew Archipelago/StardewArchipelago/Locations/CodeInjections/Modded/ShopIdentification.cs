/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public class ShopIdentification
    {
        public string Context { get; }
        public string NpcFilter { get; }

        public ShopIdentification(string context) : this(context, "")
        {
        }

        public ShopIdentification(string context, string npcFilter)
        {
            Context = context;
            NpcFilter = npcFilter;
        }

        public bool IsCorrectShop(ShopMenu shop)
        {
            if (!shop.storeContext.Equals(Context, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(NpcFilter))
            {
                return true;
            }

            return shop.portraitPerson.Name.Contains(NpcFilter, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Context.GetHashCode() * 21 + NpcFilter.GetHashCode();
        }
    }
}
