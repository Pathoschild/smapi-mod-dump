/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using Netcode;

namespace BetterSpecialOrders;

public class BoardConfig : INetObject<NetFields>
{
    public NetString boardContext = new NetString("");
    public NetBool  canReroll = new NetBool(false);
    public NetBool infiniteRerolls = new NetBool(false);
    public NetInt maxRerolls = new NetInt(0);
    private NetArray<bool, NetBool> refreshSchedule = new NetArray<bool, NetBool>(7);

    public NetFields NetFields { get; } = new NetFields("BetterSpecialOrders.BoardConfig");

    public BoardConfig()
    {
        InitializeNetFields();
    }

    public BoardConfig(string ctx, bool canReroll, bool infiniteRerolls, int maxRerolls, bool[] refreshSchedule)
    {
        boardContext.Set(ctx);
        this.canReroll.Set(canReroll);
        this.infiniteRerolls.Set(infiniteRerolls);
        this.maxRerolls.Set(maxRerolls);
        this.refreshSchedule.Set(refreshSchedule);
    }

    public bool shouldRefreshToday(int dayOfTheWeek)
    {
        return refreshSchedule[dayOfTheWeek];
    }

    private void InitializeNetFields()
    {
        NetFields.SetOwner(this)
            .AddField(boardContext, "boardContext")
            .AddField(canReroll, "canReroll")
            .AddField(infiniteRerolls, "infiniteRerolls")
            .AddField(maxRerolls, "maxRerolls")
            .AddField(refreshSchedule, "refreshSchedule");
    }
}