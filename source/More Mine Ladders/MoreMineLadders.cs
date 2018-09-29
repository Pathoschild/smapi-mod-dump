using System;
using System.IO;
using StardewModdingAPI;
using StardewValley.Locations;
using Harmony;
using StardewValley;

namespace MoreMineLadders
{
    public class MoreMineLadders : Mod
    {
        public override void Entry(IModHelper mod)
        {
            instance = this;
            InitConfig();

            harmony = HarmonyInstance.Create("JadeTheavas.StardewValley.MoreMineLadders");

            harmony.Patch(AccessTools.Method(typeof(MineShaft), "createLadderDown"), 
                new HarmonyMethod(typeof(createLadderDown_Patch), "Prefix"), null);
            harmony.Patch(AccessTools.Method(typeof(MineShaft), "checkStoneForItems"), 
                new HarmonyMethod(typeof(checkStoneForItems_Patch), "Prefix"), 
                new HarmonyMethod(typeof(checkStoneForItems_Patch), "Postfix"));

            mod.ConsoleCommands.Add("makeladder", "Creates a ladder at the player's position.", new Action<string, string[]>(this.MakeLadder));
            mod.ConsoleCommands.Add("mml_reloadconfig", "Reloads your MoreMineLadders settings from config.", new Action<string, string[]>(this.InitConfig));
        }

        void InitConfig(string cmd = null, string[] args = null)
        {
            string cfg_path = Path.Combine(base.Helper.DirectoryPath, "config.json");
            if (!File.Exists(cfg_path))
            {
                base.Monitor.Log("No config file found. Creating a new one.");
                this.config = new MoreMineLadders_Config();
                base.Helper.WriteJsonFile<MoreMineLadders_Config>(cfg_path, this.config);
                return;
            }
            this.config = base.Helper.ReadJsonFile<MoreMineLadders_Config>(cfg_path);
        }

        void MakeLadder(string cmd, string[] args)
        {
            if (!Game1.inMine)
            {
                base.Monitor.Log("You need to be inside the mine to use this command.", LogLevel.Alert);
                return;
            }

            Game1.mine.createLadderDown(Game1.player.getTileX(), Game1.player.getTileY());
            base.Monitor.Log("Created a ladder at the player's position." + Game1.player.getTileLocation().ToString(), LogLevel.Alert);
        }

        public static MoreMineLadders instance;

        public HarmonyInstance harmony;

        public MoreMineLadders_Config config;
    }

    public class MoreMineLadders_Config
    {
        public bool Enabled = true;

        public bool affectedByLuck = false;

        public float dropLadderChance = 0.1f;

    }
}
