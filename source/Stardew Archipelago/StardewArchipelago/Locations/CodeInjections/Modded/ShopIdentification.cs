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
            if (!shop.ShopId.Equals(Context, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(NpcFilter))
            {
                return true;
            }

            // I have no idea how to identify who is tending a shop in 1.6
            throw new Exception($"{nameof(IsCorrectShop)} is not ready for 1.6");
            // return shop.portraitPerson.Name.Contains(NpcFilter, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Context.GetHashCode() * 21 + NpcFilter.GetHashCode();
        }
    }
}
