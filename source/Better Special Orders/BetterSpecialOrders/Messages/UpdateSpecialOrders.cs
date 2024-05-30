/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using StardewValley.SpecialOrders;

namespace BetterSpecialOrders.Messages;

public class UpdateSpecialOrders
{
    
    // if this is ALL then replace all orders 
    public string orderType;
    public List<SpecialOrder> orders;

}