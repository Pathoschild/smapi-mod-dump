/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace weizinai.StardewValleyMod.Common;

public class ItemRepository
{
    /// <summary>农场普通石头的QualifiedItemId</summary>
    public static readonly HashSet<string> FarmStone = new() { "(O)343", "(O)450" };

    /// <summary>其他普通石头的QualifiedItemId</summary>
    public static readonly HashSet<string> OtherStone = new()
    {
        "(O)32", "(O)34", "(O)36", "(O)38", "(O)40", "(O)42", "(O)48", "(O)50", "(O)52", "(O)54", "(O)56", "(O)58", "(O)668", "(O)670", "(O)760", "(O)762",
        "(O)845", "(O)846", "(O)847"
    };

    /// <summary>姜岛石头的QualifiedItemId</summary>
    public static readonly HashSet<string> IslandStone = new() { "(O)25", "(O)816", "(O)817", "(O)818" };

    /// <summary>矿石石头的QualifiedItemId</summary>
    public static readonly HashSet<string> OreStone = new()
    {
        "(O)95", "(O)290", "(O)751", "(O)764", "(O)765", "(O)843", "(O)844", "(O)849", "(O)850", "(O)BasicCoalNode0", "(O)BasicCoalNode1",
        "(O)VolcanoCoalNode0", "(O)VolcanoCoalNode1", "(O)VolcanoGoldNode"
    };

    /// <summary>宝石石头的QualifiedItemId</summary>
    public static readonly HashSet<string> GemStone = new() { "(O)2", "(O)4", "(O)6", "(O)8", "(O)10", "(O)12", "(O)14", "(O)44", "(O)46" };

    /// <summary>晶球石头的QualifiedItemId</summary>
    public static readonly HashSet<string> GeodeStone = new() { "(O)75", "(O)76", "(O)77", "(O)819" };

    /// <summary>卡利三花蛋的QualifiedItemId</summary>
    public static readonly HashSet<string> CalicoEggStone = new() { "(O)CalicoEggStone_0", "(O)CalicoEggStone_1", "(O)CalicoEggStone_2" };
}