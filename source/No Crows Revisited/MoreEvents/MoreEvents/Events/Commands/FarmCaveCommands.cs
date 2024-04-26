/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/StardewMods
**
*************************************************/

using MoreEvents.Events.Precondition;
using StardewValley;
using StardewValley.Locations;

namespace MoreEvents.Events.Commands;
using static Logger;

public class FarmCaveCommands
{
    ///  <summary> 洞穴升级.
    /// 见 <see cref="FarmCavePreconditions.FarmCaveFirstComplete"/></summary>
    public static void FarmCaveUpgrade(Event @event, string[] args, EventContext context)
    {
        var wrapper = Game1.MasterPlayer.caveChoice;
        var lastCave = wrapper?.Value;
        switch (lastCave)
        {
            // 水果 + 蘑菇
            case 1:
                Game1.RequireLocation<FarmCave>("FarmCave").setUpMushroomHouse();
                break;
            // 蘑菇 + 水果
            case 2:
                wrapper!.Set(1);
                break;
            // 错误状态
            default:
                context.LogErrorAndSkip("必须在洞穴事件后");
                break;
        }
        Debug("FarmCaveUpgrade success!");
        ++@event.CurrentCommand;
    }
}