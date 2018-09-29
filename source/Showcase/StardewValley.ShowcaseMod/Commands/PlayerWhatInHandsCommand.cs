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