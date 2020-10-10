/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using Igorious.StardewValley.DynamicApi2;
using Igorious.StardewValley.DynamicApi2.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace Igorious.StardewValley.ShowcaseMod.Commands
{
    public sealed class PlayerWhatInHandsCommand : ConsoleCommand
    {
        public PlayerWhatInHandsCommand(IMonitor monitor) : base(monitor, "player_whatinhands", "Get info about item in hands.") { }

        public void Execute()
        {
            var item = Game1.player.ActiveObject ?? (Item)Game1.player.CurrentTool;
            Info(item.GetInfo());
        }
    }
}