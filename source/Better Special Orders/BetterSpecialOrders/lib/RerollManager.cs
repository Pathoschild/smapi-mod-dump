/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using BetterSpecialOrders.Messages;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Network;
using StardewValley.SpecialOrders;

namespace BetterSpecialOrders;

public class RerollManager
{

    private static RerollManager _instance = null;
    public ModConfig config;
    
    // Game State
    public IDictionary<string, BoardConfig> localBoardConfigs = new Dictionary<string, BoardConfig>();
    public IDictionary<string, int> localRerollsRemaining = new Dictionary<string, int>();
    public NetStringDictionary<BoardConfig, NetRef<BoardConfig>> boardConfigs = new NetStringDictionary<BoardConfig, NetRef<BoardConfig>>();
    public NetStringDictionary<int, NetInt> rerollsRemaining = new NetStringDictionary<int, NetInt>();
    public int rerollsToday = 0;
    
    // cache for overriding the Monday reroll if necessary
    public List<SpecialOrder> lastAvailableSpecialOrders;

    
    // Singleton Pattern
    public static RerollManager Get()
    {
        if (_instance == null)
        {
            _instance = new RerollManager();
        }

        return _instance;
    }

    private RerollManager()
    {
        // Load Settings from the Mod Config
        config = ModEntry.GHelper.ReadConfig<ModConfig>();
        //use the config to rebuild the state;
        localBoardConfigs = LoadBoardConfigs();
        localRerollsRemaining = LoadDefaultRerollsRemaining();
        
        RebuildConfig();
        
        lastAvailableSpecialOrders = new List<SpecialOrder>(); // create this to avoid null issue
    }
    
    #region REROLLS
    
    /*
     * Checks to see whether a reroll is allowed
     *  this means checking to see if one of the
     *  options on the board is currently selected.
     * Additionally, we check to see if there are
     *  rerolls left or if we have infinite rerolls
     *  enabled
     */
    public bool CanReroll(string orderType)
    {
        // if we can't reroll then we can't reroll... duh
        if (!boardConfigs[orderType].canReroll.Get())
        {
            return false;
        }
        
        // if there are no rerolls left and we don't have unlimited
        if (rerollsRemaining[orderType] <= 0 && !boardConfigs[orderType].infiniteRerolls.Get())
        {
            return false;
        }

        foreach (SpecialOrder availableOrder in Game1.player.team.availableSpecialOrders)
        {
            foreach (SpecialOrder currentOrder in Game1.player.team.specialOrders)
            {
                if (currentOrder.questKey == availableOrder.questKey)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    /* Rep-Server
     * This is a modified copy of SpecialOrder.UpdateAvailableSpecialOrders
     *  which adds a counter for the number of times rerolled today and uses
     *  it to generate a seed that actually allows the rerolling to occur.
     * If we are not the host then we send a request to the host to replicate
     */
    public void Reroll(string orderType)
    {
        // if we aren't the main player then send a reroll request to the main player
        if (!Context.IsMainPlayer)
        {
            ModEntry.GHelper.Multiplayer.SendMessage(
                new RequestReroll(orderType), 
                Constants.REQUEST_REROLL, 
                modIDs: new []{ModEntry.ModID}, 
                playerIDs: new []{Game1.MasterPlayer.UniqueMultiplayerID}
            );
            return;
        }
        
        //handle actual reroll logic
        if (!CanReroll(orderType))
        {
            ModEntry.GMonitor.Log("Cannot Reroll Order Type");
            return;
        }
        
        ModEntry.GMonitor.Log($"Rerolling {orderType}");
        rerollsToday += 1; // do this first to avoid getting the same options on the first reroll of the day
        SpecialOrder.RemoveAllSpecialOrders(orderType);
        List<string> keyQueue = new List<string>();
        foreach (KeyValuePair<string, SpecialOrderData> pair in DataLoader.SpecialOrders(Game1.content))
        {
            if (pair.Value.OrderType == orderType && SpecialOrder.CanStartOrderNow(pair.Key, pair.Value))
            {
                keyQueue.Add(pair.Key);
            }
        }
        List<string> keysIncludingCompleted = new List<string>(keyQueue);
        if (orderType == "")
        {
            keyQueue.RemoveAll((string id) => Game1.player.team.completedSpecialOrders.Contains(id));
        }

        Random r = Utility.CreateRandom(Game1.uniqueIDForThisGame, (double)Game1.stats.DaysPlayed * 1.3, rerollsToday);
        
        // if the user wants true random then scrub and start over
        if (config.useTrueRandom)
        {
            r = new Random();
        }
        for (int i = 0; i < 2; i++)
        {
            if (keyQueue.Count == 0)
            {
                if (keysIncludingCompleted.Count == 0)
                {
                    break;
                }

                keyQueue = new List<string>(keysIncludingCompleted);
            }

            string key = r.ChooseFrom(keyQueue);
            SpecialOrder order = SpecialOrder.GetSpecialOrder(key, r.Next());
            order.SetHardOrderDuration(); // override the current duration
            Game1.player.team.availableSpecialOrders.Add(order);
            keyQueue.Remove(key);
            keysIncludingCompleted.Remove(key);
        }
        
        // after reroll complete subtract from available rerolls and notify clients of update to available orders
        rerollsRemaining[orderType] -= 1;
        
    }
    
    // Resets the rerolls for the specified type back to their max values;
    // HOST ONLY
    public void ResetRerolls(string orderType = Constants.ALL, bool resetDayTotal = false)
    {
        // Main player only
        if (!Context.IsMainPlayer)
        {
            return;
        }

        if (resetDayTotal)
        {
            rerollsToday = 0;
        }

        if (orderType == Constants.ALL)
        {
            // support custom board types
            foreach (BoardConfig board in boardConfigs.Values)
            {
                rerollsRemaining[orderType] = board.maxRerolls.Get();
            }
        }
        else
        {
            rerollsRemaining[orderType] = boardConfigs[orderType].maxRerolls.Get();
        }
    }
    
    #endregion

    #region CONFIG MANAGEMENT

    // Rebuild the config settings from the config file
    public void RebuildConfig()
    {
        boardConfigs.Set(LoadBoardConfigs());
        rerollsRemaining.Set(LoadDefaultRerollsRemaining());
    }
    
    private IDictionary<string, BoardConfig> LoadBoardConfigs()
    {
        return new Dictionary<string, BoardConfig>()
        {
            {
                Constants.SVBoardContext, new BoardConfig(
                    Constants.SVBoardContext,
                    config.sv_allowReroll,
                    config.sv_infiniteReroll,
                    config.sv_maxRerollCount,
                    config.sv_refresh_schedule
                )
            },
            {
                Constants.QiBoardContext, new BoardConfig(
                    Constants.QiBoardContext,
                    config.qi_allowReroll,
                    config.qi_infiniteReroll,
                    config.qi_maxRerollCount,
                    config.qi_refresh_schedule
                )
            },
            {
                Constants.DesertBoardContext, new BoardConfig(
                    Constants.DesertBoardContext,
                    config.de_allowReroll,
                    config.de_infiniteReroll,
                    config.de_maxRerollCount,
                    new bool[7] {true, true, true, true, true, true, true}
                )
            }
        };
    }

    private IDictionary<string, int> LoadDefaultRerollsRemaining()
    {
        return new Dictionary<string, int>()
        {
            { Constants.SVBoardContext, config.sv_maxRerollCount },
            { Constants.QiBoardContext, config.qi_maxRerollCount },
            { Constants.DesertBoardContext, config.de_maxRerollCount }
        };
    }
    
    #endregion

    #region CACHING

    // Clears the current cache and adds back the requested order type from the current available orders
    public void CacheCurrentAvailableSpecialOrders(string orderType = Constants.ALL)
    {
        // Clears only the requested type from the cache
        if (orderType == Constants.ALL)
        {
            lastAvailableSpecialOrders.Clear();
        }
        else
        {
            lastAvailableSpecialOrders.RemoveAll(order => order.orderType == orderType);
        }
        
        // Add the selected order type from the current list to the cache
        foreach (SpecialOrder order in Game1.player.team.availableSpecialOrders)
        {
            if (orderType == Constants.ALL || orderType == order.orderType.Get())
            {
                lastAvailableSpecialOrders.Add(order);
            }
        }
    }

    // Loads the requested orderType from the cache
    public void ReloadSpecialOrdersFromCache(string orderType = Constants.ALL)
    {
        ModEntry.GMonitor.Log("Running Reload");
        // remove the current orders that match the type
        if (orderType == Constants.ALL)
        {
            Game1.player.team.availableSpecialOrders.Clear();
        }
        else
        {
            Game1.player.team.availableSpecialOrders.RemoveWhere(order => order.orderType == orderType);
        }

        // reload from the cache
        foreach (SpecialOrder order in lastAvailableSpecialOrders)
        {
            if (orderType == Constants.ALL || orderType == order.orderType.Get())
            {
                order.SetHardOrderDuration();
                Game1.player.team.availableSpecialOrders.Add(order);
            }
        }
    }

    #endregion
}