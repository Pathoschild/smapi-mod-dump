using Igorious.StardewValley.DynamicApi2;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace Igorious.StardewValley.ShowcaseMod.Commands
{
    public sealed class PlayerUpgradeToolCommand : ConsoleCommand
    {
        public PlayerUpgradeToolCommand(IMonitor monitor) : base(monitor, "player_upgradetool", "Upgrade current tool.") { }

        public void Execute()
        {
            var tool = Game1.player.CurrentTool;
            if (!(tool is Axe || tool is Hoe || tool is Pickaxe || tool is WateringCan) || tool.UpgradeLevel == 4)
            {
                Error("Can't upgrade tool!");
                return;
            }

            ++tool.UpgradeLevel;
            Info($"Upgraded to {tool.Name}.");
        }
    }
}