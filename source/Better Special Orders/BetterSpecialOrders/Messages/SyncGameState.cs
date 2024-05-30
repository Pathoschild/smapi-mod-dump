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

// Sent whenever we need to sync the game state.
public class SyncGameState
{
    // This is a dictionary of all the boards we are watching
    public IDictionary<string, BoardConfig> boardConfigs;
    
    // This is a dictionary of the number of rerolls remaining
    public IDictionary<string, int> rerollsRemaining;
}