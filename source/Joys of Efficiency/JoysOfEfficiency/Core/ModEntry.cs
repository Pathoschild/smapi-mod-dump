using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.EventHandler;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.ModCheckers;
using JoysOfEfficiency.Patches;
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
        public static bool IsCcOn { get; private set; }
        public static bool IsCaOn { get; private set; }

        private static Config Conf => InstanceHolder.Config;

        public static bool HarmonyPatched { get; private set; }

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
            Logger.Init(this);

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

            if (ModChecker.IsCcLoaded(helper))
            {
                Logger.Log("Convenient Chests detected. JoE's CraftingFromChests feature will be disabled and won't patch the game.");
                Conf.CraftingFromChests = false;
                IsCcOn = true;
            }
            else if(!Conf.SafeMode)
            {
                Logger.Log("Start patching using Harmony...");
                HarmonyPatched = HarmonyPatcher.Init();
            }
            else
            {
                Logger.Log("SafeMode enabled, and won't patch the game.");
            }
            helper.WriteConfig(Conf);
            MineIcons.Init(helper);
        }

        private static void OnDebugCommand(string name, string[] args)
        {
            DebugMode = !DebugMode;
            string str = "";
            foreach (KeyValuePair<int, string> info in Game1.objectInformation)
            {
                string val = info.Value.Split('/').FirstOrDefault();
                if (val == null)
                {
                    continue;
                }

                Logger.Log($"{info.Key}: {val}\r\n");
            }

        }
    }
}
