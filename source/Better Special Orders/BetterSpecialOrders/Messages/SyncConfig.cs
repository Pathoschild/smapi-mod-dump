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

public class SyncConfig
{
    // This is a dictionary of all the boards we are watching
    public IDictionary<string, BoardConfig> boardConfigs;
    
    // This is a dictionary of the number of rerolls remaining
    public IDictionary<string, int> rerollsRemaining;

    public SyncConfig()
    {
        boardConfigs = new Dictionary<string, BoardConfig>();
        rerollsRemaining = new Dictionary<string, int>();
    }

    public SyncConfig(IDictionary<string, BoardConfig> boardConfigs, IDictionary<string, int> rerollsRemaining)
    {
        this.boardConfigs = boardConfigs;
        this.rerollsRemaining = rerollsRemaining;
    }
}