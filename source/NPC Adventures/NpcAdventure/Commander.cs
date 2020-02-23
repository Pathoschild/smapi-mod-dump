using NpcAdventure.Story;
using StardewModdingAPI;
using StardewValley;

namespace NpcAdventure
{
    internal class Commander
    {
        private readonly NpcAdventureMod npcAdventureMod;
        private readonly IMonitor monitor;

        private Commander(NpcAdventureMod npcAdventureMod)
        {
            this.npcAdventureMod = npcAdventureMod;
            this.monitor = npcAdventureMod.Monitor;
            this.SetupCommands(npcAdventureMod.Helper.ConsoleCommands);
        }

        internal void SetupCommands(ICommandHelper consoleCommands)
        {
            if (!this.npcAdventureMod.Config.EnableDebug)
                return;

            consoleCommands.Add("npcadventure_eligible", "Make player eligible to recruit a companion (server or singleplayer only)", this.Eligible);
            this.monitor.Log("Registered debug commands", LogLevel.Info);
        }

        private void Eligible(string command, string[] args)
        {
            if (Context.IsWorldReady && Context.IsMainPlayer && this.npcAdventureMod.GameMaster.Mode == GameMasterMode.MASTER)
            {
                this.npcAdventureMod.GameMaster.Data.GetPlayerState(Game1.player).isEligible = true;
                this.npcAdventureMod.GameMaster.SyncData();
                this.monitor.Log("Player is now eligible for recruit companion.", LogLevel.Info);
            } else
            {
                this.monitor.Log("Can't eligible player when game is not loaded, in non-adventure mode or not running on server!", LogLevel.Alert);
            }
        }

        public static Commander Register(NpcAdventureMod mod)
        {
            return new Commander(mod);
        }
    }
}
