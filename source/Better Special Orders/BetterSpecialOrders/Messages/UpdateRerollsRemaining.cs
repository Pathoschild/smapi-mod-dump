/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

namespace BetterSpecialOrders.Messages;

// this is sent whenever the reroll count needs to updated
public class UpdateRerollsRemaining
{
    // The context of the board that was rerolled
    public string orderType;
    
    // The number of rerolls remaining for the context's board
    public int rerollsRemaining;

    public UpdateRerollsRemaining()
    {
        orderType = "";
        rerollsRemaining = 0;
    }

    public UpdateRerollsRemaining(string orderType, int rerollsRemaining)
    {
        this.orderType = orderType;
        this.rerollsRemaining = rerollsRemaining;
    }
}