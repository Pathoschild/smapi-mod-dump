/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class ItemCheatHandler : BaseHandler
{
    public ItemCheatHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.ConsoleCommands.Add("inventory", "", this.AccessInventory);
    }

    private void AccessInventory(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var farmer = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);
        if (farmer is null)
        {
            Log.Info($"{args[0]}不存在，无法访问该玩家的背包。");
        }
        else
        {
            Log.Alert($"{farmer.Name}的背包中有：");
            foreach (var item in farmer.Items)
            {
                if (item is null) continue;
                Log.Info($"{item.Stack}\t{item.DisplayName}");
            }
        }
    }
}