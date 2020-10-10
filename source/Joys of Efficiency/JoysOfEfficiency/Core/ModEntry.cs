/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using JoysOfEfficiency.EventHandler;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.ModCheckers;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;

using StardewValley;

namespace JoysOfEfficiency.Core
{
    using Player = Farmer;


    /// <summary>
    /// This class is a representation of the mod itself.
    /// </summary>
    internal class ModEntry : Mod
    {
        public static bool IsCoGOn { get; private set; }
        public static bool IsCaOn { get; private set; }
        private static Config Conf => InstanceHolder.Config;
        public static bool DebugMode { get; private set; }

        private static readonly Logger Logger = new Logger("Main");

        /// <summary>
        /// Called firstly when SMAPI finished loading of the mod.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            // Loads configuration from file.
            Config conf = Helper.ReadConfig<Config>();

            // Initialize InstanceHolder.
            InstanceHolder.Init(this, conf);

            // Initialize Logger
            Logger.Init(Monitor);

            // Register events.
            EventHolder.RegisterEvents(Helper.Events);
            

            // Registration commands.
            Helper.ConsoleCommands.Add("joedebug", "Debug command for JoE", OnDebugCommand);

            // Limit config values.
            ConfigLimitation.LimitConfigValues();

            // Check mod compatibilities.
            if(ModChecker.IsCoGLoaded(helper))
            {
                Logger.Log("CasksOnGround detected.");
                IsCoGOn = true;
            }

            if (ModChecker.IsCaLoaded(helper))
            {
                Logger.Log("CasksAnywhere detected.");
                IsCaOn = true;
            }

            helper.WriteConfig(Conf);
            MineIcons.Init(helper);
        }

        private static void OnDebugCommand(string name, string[] args)
        {
            DebugMode = !DebugMode;
            Farmer player = Game1.player;
            Logger.Log($"Facing:{player.FacingDirection}");

        }
    }
}
