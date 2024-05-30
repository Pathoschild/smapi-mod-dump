/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace BetterSpecialOrders;

public class ModConfig
{
    /** General Settings **/
    public KeybindList resetRerollsKeybind = new KeybindList();

    public bool useTrueRandom = false;
    
    /** STARDEW VALLEY SPECIAL ORDERS **/
    
    // Flags for which days the board should auto refresh
    public bool[] sv_refresh_schedule = new bool[7] {true, false, false, false, false, false, false};

    // If true then the player can reroll sv_rerollCount times
    public bool sv_allowReroll = false;
    public bool sv_infiniteReroll = false;

    public int sv_maxRerollCount = 1;
    

    /** QI QUESTS **/
    // Flags for which days the board should auto refresh
    public bool[] qi_refresh_schedule = new bool[7] {true, false, false, false, false, false, false};

    // If true then the player can reroll qi_rerollCount times
    public bool qi_allowReroll = false;
    public bool qi_infiniteReroll = false;

    public int qi_maxRerollCount = 1;
    

    /** DESERT EVENT QUESTS **/
    public bool de_allowReroll = false;
    public bool de_infiniteReroll = false;

    public int de_maxRerollCount = 1;
    

    
    
}